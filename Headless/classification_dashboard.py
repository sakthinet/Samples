import streamlit as st
import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestClassifier
from sklearn.linear_model import LogisticRegression
from sklearn.svm import SVC
from sklearn.preprocessing import StandardScaler, LabelEncoder
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.metrics import accuracy_score, precision_score, recall_score, f1_score, confusion_matrix, classification_report
import plotly.express as px
import plotly.graph_objects as go
from plotly.subplots import make_subplots
import seaborn as sns
import matplotlib.pyplot as plt
import io
import base64
from typing import Dict, List, Tuple, Any
import warnings
warnings.filterwarnings('ignore')

class DataClassificationSystem:
    def __init__(self):
        self.data = None
        self.processed_data = None
        self.models = {}
        self.scalers = {}
        self.encoders = {}
        self.vectorizers = {}
        self.predictions = {}
        self.metrics = {}
        
    def load_data(self, file) -> pd.DataFrame:
        """Load data from CSV or Excel file"""
        try:
            if file.name.endswith('.csv'):
                df = pd.read_csv(file)
            elif file.name.endswith(('.xlsx', '.xls')):
                df = pd.read_excel(file)
            else:
                st.error("Unsupported file format. Please upload CSV or Excel file.")
                return None
            return df
        except Exception as e:
            st.error(f"Error loading file: {str(e)}")
            return None
    
    def check_data_suitability(self, df: pd.DataFrame, target_column: str) -> bool:
        """Check if data is suitable for classification"""
        if df is None or target_column not in df.columns:
            return False
        
        # Check target column
        target_values = df[target_column].dropna()
        unique_values = target_values.unique()
        total_rows = len(df)
        unique_count = len(unique_values)
        
        # Check for ID columns (likely not suitable as target)
        id_indicators = ['id', 'employee_id', 'user_id', 'customer_id', 'employeeid', 'record_id']
        if any(indicator in target_column.lower() for indicator in id_indicators):
            st.error(f"❌ '{target_column}' appears to be an ID column and cannot be used as a target.")
            st.info("💡 ID columns are not suitable for classification. Please select a categorical column like STATUS, department_name, or BUSINESS_UNIT.")
            return False
        
        # Check if target has too many unique values (likely continuous or ID)
        uniqueness_ratio = unique_count / total_rows
        if uniqueness_ratio > 0.1:  # More than 10% unique values
            st.error(f"❌ Target column '{target_column}' has {unique_count:,} unique values out of {total_rows:,} rows ({uniqueness_ratio:.1%} uniqueness).")
            st.info("💡 This appears to be an identifier or continuous variable. For classification, choose a column with categories (2-50 distinct values).")
            return False
        
        # Check for reasonable number of classes for classification
        if unique_count > 50:
            st.warning(f"⚠️ Target column '{target_column}' has {unique_count} classes. This might be too many for effective classification.")
            st.info("💡 Consider selecting a column with fewer categories (2-20 is ideal).")
            return False
        
        # Check class distribution
        value_counts = target_values.value_counts()
        min_class_size = value_counts.min()
        
        if min_class_size < 2:
            st.warning(f"⚠️ Some classes in '{target_column}' have less than 2 samples. Minimum class size: {min_class_size}")
            return False
        
        return True
    
    def suggest_target_columns(self, df: pd.DataFrame) -> List[str]:
        """Suggest suitable target columns for classification"""
        suggestions = []
        
        for col in df.columns:
            # Skip obvious ID columns
            if any(indicator in col.lower() for indicator in ['id', '_key', 'key_']):
                continue
                
            unique_count = df[col].nunique()
            total_rows = len(df)
            uniqueness_ratio = unique_count / total_rows
            
            # Good targets: 2-50 unique values, less than 10% uniqueness
            if 2 <= unique_count <= 50 and uniqueness_ratio <= 0.1:
                # Check if classes have sufficient samples
                value_counts = df[col].value_counts()
                min_class_size = value_counts.min()
                
                if min_class_size >= 2:
                    suggestions.append({
                        'column': col,
                        'unique_count': unique_count,
                        'min_class_size': min_class_size,
                        'data_type': str(df[col].dtype)
                    })
        
        return suggestions
    
    def preprocess_data(self, df: pd.DataFrame, target_column: str) -> Tuple[np.ndarray, np.ndarray]:
        """Preprocess structured and unstructured data with better error handling"""
        if not self.check_data_suitability(df, target_column):
            return None, None
            
        try:
            # Separate features and target
            X = df.drop(columns=[target_column])
            y = df[target_column].dropna()  # Remove NaN values from target
            
            # Remove rows where target is NaN
            valid_indices = y.index
            X = X.loc[valid_indices]
            
            # Handle missing values in features
            X = X.fillna('')
            
            # Remove ID-like columns from features
            id_columns = [col for col in X.columns if col.lower().endswith('id') or col.lower() == 'id']
            if id_columns:
                st.info(f"🗑️ Removing ID columns from features: {id_columns}")
                X = X.drop(columns=id_columns)
            
            # Identify column types
            numeric_cols = X.select_dtypes(include=[np.number]).columns.tolist()
            text_cols = X.select_dtypes(include=['object']).columns.tolist()
            
            # Remove constant columns
            constant_cols = []
            for col in numeric_cols:
                if X[col].nunique() <= 1:
                    constant_cols.append(col)
            
            for col in text_cols:
                if X[col].nunique() <= 1:
                    constant_cols.append(col)
            
            if constant_cols:
                st.info(f"🗑️ Removing constant columns: {constant_cols}")
                X = X.drop(columns=constant_cols)
                numeric_cols = [col for col in numeric_cols if col not in constant_cols]
                text_cols = [col for col in text_cols if col not in constant_cols]
            
            processed_features = []
            
            # Process numeric columns
            if numeric_cols:
                numeric_data = X[numeric_cols].fillna(0)
                if len(numeric_cols) > 0:
                    scaler = StandardScaler()
                    numeric_scaled = scaler.fit_transform(numeric_data)
                    processed_features.append(numeric_scaled)
                    self.scalers['numeric'] = scaler
                    st.info(f"✅ Processed {len(numeric_cols)} numeric features")
            
            # Process text columns
            if text_cols:
                # Combine all text columns
                text_data = X[text_cols].astype(str).agg(' '.join, axis=1)
                # Remove empty strings
                text_data = text_data.replace('', 'unknown')
                
                vectorizer = TfidfVectorizer(
                    max_features=min(1000, len(text_data)), 
                    stop_words='english', 
                    ngram_range=(1, 2),
                    min_df=1,
                    max_df=0.95
                )
                text_features = vectorizer.fit_transform(text_data).toarray()
                processed_features.append(text_features)
                self.vectorizers['text'] = vectorizer
                st.info(f"✅ Processed {len(text_cols)} text features → {text_features.shape[1]} TF-IDF features")
            
            # Combine all features
            if processed_features:
                X_processed = np.hstack(processed_features)
            else:
                st.error("❌ No valid features found for processing")
                return None, None
            
            # Encode target variable
            label_encoder = LabelEncoder()
            y_encoded = label_encoder.fit_transform(y)
            self.encoders['target'] = label_encoder
            
            st.success(f"✅ Preprocessing completed: {X_processed.shape[0]} samples, {X_processed.shape[1]} features")
            st.info(f"🎯 Target classes: {list(label_encoder.classes_)}")
            
            return X_processed, y_encoded
            
        except Exception as e:
            st.error(f"❌ Preprocessing error: {str(e)}")
            return None, None
    
    def get_optimal_model_config(self, X: np.ndarray, y: np.ndarray) -> Dict:
        """Get optimal model configuration based on dataset characteristics"""
        n_samples = len(X)
        n_features = X.shape[1]
        n_classes = len(np.unique(y))
        
        # Calculate complexity factors
        samples_per_class = n_samples / n_classes
        feature_to_sample_ratio = n_features / n_samples
        
        config = {
            'test_size': 0.2,
            'use_stratify': True,
            'models': {},
            'complexity_level': 'medium'
        }
        
        # Determine complexity level and configuration
        if n_samples < 50:
            # Very small dataset
            config.update({
                'complexity_level': 'very_low',
                'test_size': 0.3,  # Larger test set for better evaluation
                'use_stratify': samples_per_class >= 2,
                'models': {
                    'Logistic Regression': {
                        'class': LogisticRegression,
                        'params': {'random_state': 42, 'max_iter': 500, 'C': 1.0}
                    },
                    'Simple Random Forest': {
                        'class': RandomForestClassifier,
                        'params': {
                            'n_estimators': 20,
                            'max_depth': 3,
                            'min_samples_split': 5,
                            'min_samples_leaf': 2,
                            'random_state': 42
                        }
                    }
                }
            })
            
        elif n_samples < 200:
            # Small dataset
            config.update({
                'complexity_level': 'low',
                'test_size': 0.25,
                'use_stratify': samples_per_class >= 2,
                'models': {
                    'Logistic Regression': {
                        'class': LogisticRegression,
                        'params': {'random_state': 42, 'max_iter': 1000, 'C': 1.0}
                    },
                    'Random Forest': {
                        'class': RandomForestClassifier,
                        'params': {
                            'n_estimators': 50,
                            'max_depth': 5,
                            'min_samples_split': 4,
                            'min_samples_leaf': 2,
                            'random_state': 42
                        }
                    }
                }
            })
            
        elif n_samples < 1000:
            # Medium dataset
            config.update({
                'complexity_level': 'medium',
                'test_size': 0.2,
                'use_stratify': True,
                'models': {
                    'Logistic Regression': {
                        'class': LogisticRegression,
                        'params': {'random_state': 42, 'max_iter': 1000}
                    },
                    'Random Forest': {
                        'class': RandomForestClassifier,
                        'params': {
                            'n_estimators': 100,
                            'max_depth': 8,
                            'min_samples_split': 3,
                            'min_samples_leaf': 1,
                            'random_state': 42
                        }
                    },
                    'SVM': {
                        'class': SVC,
                        'params': {
                            'random_state': 42,
                            'probability': True,
                            'C': 1.0,
                            'gamma': 'scale'
                        }
                    }
                }
            })
            
        elif n_samples < 10000:
            # Large dataset
            config.update({
                'complexity_level': 'high',
                'test_size': 0.2,
                'use_stratify': True,
                'models': {
                    'Random Forest': {
                        'class': RandomForestClassifier,
                        'params': {
                            'n_estimators': 200,
                            'max_depth': 12,
                            'min_samples_split': 2,
                            'min_samples_leaf': 1,
                            'random_state': 42,
                            'n_jobs': -1  # Use all CPU cores
                        }
                    },
                    'Logistic Regression': {
                        'class': LogisticRegression,
                        'params': {
                            'random_state': 42,
                            'max_iter': 2000,
                            'solver': 'lbfgs'
                        }
                    },
                    'SVM (RBF)': {
                        'class': SVC,
                        'params': {
                            'random_state': 42,
                            'probability': True,
                            'C': 1.0,
                            'gamma': 'scale',
                            'kernel': 'rbf'
                        }
                    }
                }
            })
            
        else:
            # Very large dataset (like your 50K employee dataset)
            config.update({
                'complexity_level': 'very_high',
                'test_size': 0.15,  # Smaller test set is fine with large data
                'use_stratify': True,
                'models': {
                    'Random Forest (Optimized)': {
                        'class': RandomForestClassifier,
                        'params': {
                            'n_estimators': 300,
                            'max_depth': 15,
                            'min_samples_split': 2,
                            'min_samples_leaf': 1,
                            'max_features': 'sqrt',
                            'random_state': 42,
                            'n_jobs': -1,
                            'bootstrap': True
                        }
                    },
                    'Logistic Regression (L2)': {
                        'class': LogisticRegression,
                        'params': {
                            'random_state': 42,
                            'max_iter': 3000,
                            'solver': 'lbfgs',
                            'penalty': 'l2',
                            'C': 1.0
                        }
                    },
                    'Logistic Regression (L1)': {
                        'class': LogisticRegression,
                        'params': {
                            'random_state': 42,
                            'max_iter': 3000,
                            'solver': 'liblinear',
                            'penalty': 'l1',
                            'C': 1.0
                        }
                    },
                    'SVM (Linear)': {
                        'class': SVC,
                        'params': {
                            'random_state': 42,
                            'probability': True,
                            'C': 1.0,
                            'kernel': 'linear'
                        }
                    }
                }
            })
        
        # Adjust for high-dimensional data
        if feature_to_sample_ratio > 0.1:
            st.info(f"📊 High-dimensional data detected ({n_features} features, {n_samples} samples)")
            # Reduce Random Forest complexity for high-dimensional data
            for model_name, model_config in config['models'].items():
                if 'Random Forest' in model_name:
                    model_config['params']['max_features'] = 'sqrt'
                    model_config['params']['n_estimators'] = min(model_config['params'].get('n_estimators', 100), 150)
        
        return config
    def train_models(self, X: np.ndarray, y: np.ndarray, task_type: str = 'auto'):
        """Train multiple classification models with sophisticated adaptive configuration"""
        if X is None or y is None:
            st.error("❌ No data available for training")
            return
        
        try:
            # Get optimal configuration based on dataset characteristics
            config = self.get_optimal_model_config(X, y)
            
            # Determine task type
            unique_classes, class_counts = np.unique(y, return_counts=True)
            n_classes = len(unique_classes)
            
            if task_type == 'auto':
                task_type = 'binary' if n_classes == 2 else 'multiclass'
            
            # Display dataset analysis
            st.info(f"🎯 **Dataset Analysis:**")
            st.info(f"• Task: {task_type.upper()} classification with {n_classes} classes")
            st.info(f"• Samples: {len(X):,} | Features: {X.shape[1]:,}")
            st.info(f"• Complexity Level: {config['complexity_level'].upper()}")
            st.info(f"• Models to train: {len(config['models'])}")
            
            # Show class distribution
            min_class_count = np.min(class_counts)
            max_class_count = np.max(class_counts)
            avg_class_count = np.mean(class_counts)
            
            st.info(f"• Class distribution: Min={min_class_count}, Max={max_class_count}, Avg={avg_class_count:.0f}")
            
            # Perform train-test split with adaptive parameters
            if config['use_stratify'] and min_class_count >= 2:
                X_train, X_test, y_train, y_test = train_test_split(
                    X, y, 
                    test_size=config['test_size'], 
                    random_state=42, 
                    stratify=y
                )
                st.success(f"✅ Using stratified split (test_size={config['test_size']})")
            else:
                X_train, X_test, y_train, y_test = train_test_split(
                    X, y, 
                    test_size=config['test_size'], 
                    random_state=42
                )
                st.warning(f"⚠️ Using regular split due to small class sizes (test_size={config['test_size']})")
            
            st.info(f"📊 **Data Split:** Training: {X_train.shape[0]:,} samples | Testing: {X_test.shape[0]:,} samples")
            
            # Train models with progress tracking
            successful_models = 0
            total_models = len(config['models'])
            
            # Create progress bar
            progress_bar = st.progress(0)
            status_text = st.empty()
            
            for i, (model_name, model_config) in enumerate(config['models'].items()):
                try:
                    # Update progress
                    progress = (i + 1) / total_models
                    progress_bar.progress(progress)
                    status_text.text(f"🔄 Training {model_name}... ({i+1}/{total_models})")
                    
                    # Initialize model with optimized parameters
                    model_class = model_config['class']
                    model_params = model_config['params']
                    model = model_class(**model_params)
                    
                    # Train model
                    model.fit(X_train, y_train)
                    
                    # Make predictions
                    y_pred = model.predict(X_test)
                    y_pred_proba = model.predict_proba(X_test) if hasattr(model, 'predict_proba') else None
                    
                    # Store model and predictions
                    self.models[model_name] = model
                    self.predictions[model_name] = {
                        'y_test': y_test,
                        'y_pred': y_pred,
                        'y_pred_proba': y_pred_proba,
                        'config': model_config
                    }
                    
                    # Calculate metrics
                    self.metrics[model_name] = self.calculate_metrics(y_test, y_pred, task_type)
                    
                    # Add model-specific info to metrics
                    self.metrics[model_name]['model_params'] = model_params
                    self.metrics[model_name]['complexity_level'] = config['complexity_level']
                    
                    successful_models += 1
                    
                    # Show quick performance preview
                    accuracy = self.metrics[model_name]['accuracy']
                    f1 = self.metrics[model_name]['f1_score']
                    st.success(f"✅ {model_name}: Accuracy={accuracy:.3f}, F1={f1:.3f}")
                    
                except Exception as e:
                    st.warning(f"⚠️ Could not train {model_name}: {str(e)}")
                    continue
            
            # Clean up progress indicators
            progress_bar.empty()
            status_text.empty()
            
            if successful_models == 0:
                st.error("❌ No models could be trained successfully")
            else:
                st.success(f"🎉 **Training completed!** {successful_models}/{total_models} models trained successfully")
                
                # Show best model preview
                if self.metrics:
                    best_model = max(self.metrics.keys(), key=lambda x: self.metrics[x]['f1_score'])
                    best_f1 = self.metrics[best_model]['f1_score']
                    st.info(f"🏆 **Best Model:** {best_model} (F1-Score: {best_f1:.4f})")
                
                # Show configuration summary
                with st.expander("🔧 Model Configuration Details"):
                    for model_name, model_config in config['models'].items():
                        if model_name in self.models:
                            st.write(f"**{model_name}:**")
                            for param, value in model_config['params'].items():
                                st.write(f"  • {param}: {value}")
                
        except Exception as e:
            st.error(f"❌ Training error: {str(e)}")
            st.info("💡 This might be due to data incompatibility or insufficient memory.")
            
            # Provide specific suggestions based on error
            error_str = str(e).lower()
            if 'memory' in error_str:
                st.info("🔧 **Memory Issue Solutions:**")
                st.info("• Try reducing the number of features")
                st.info("• Use a smaller dataset for testing")
                st.info("• Consider using simpler models")
            elif 'stratify' in error_str:
                st.info("🔧 **Stratification Issue Solutions:**")
                st.info("• Some classes might have too few samples")
                st.info("• Try using a different target column")
                st.info("• Consider combining similar classes")
    
    def calculate_metrics(self, y_true: np.ndarray, y_pred: np.ndarray, task_type: str) -> Dict:
        """Calculate classification metrics with robust error handling"""
        try:
            # Determine averaging method
            unique_classes = len(np.unique(y_true))
            if unique_classes == 2:
                avg_method = 'binary'
            else:
                avg_method = 'weighted'
            
            metrics = {
                'accuracy': accuracy_score(y_true, y_pred),
                'precision': precision_score(y_true, y_pred, average=avg_method, zero_division=0),
                'recall': recall_score(y_true, y_pred, average=avg_method, zero_division=0),
                'f1_score': f1_score(y_true, y_pred, average=avg_method, zero_division=0),
                'confusion_matrix': confusion_matrix(y_true, y_pred),
                'classification_report': classification_report(y_true, y_pred, zero_division=0)
            }
            
            return metrics
            
        except Exception as e:
            st.error(f"Error calculating metrics: {str(e)}")
            return {
                'accuracy': 0.0,
                'precision': 0.0,
                'recall': 0.0,
                'f1_score': 0.0,
                'confusion_matrix': np.array([[0]]),
                'classification_report': "Error generating report"
            }
    
    def predict_new_data(self, model_name: str) -> pd.DataFrame:
        """Make predictions on the entire dataset"""
        if model_name not in self.models or self.processed_data is None:
            return None
        
        try:
            model = self.models[model_name]
            predictions = model.predict(self.processed_data[0])
            
            # Decode predictions
            if 'target' in self.encoders:
                predictions_decoded = self.encoders['target'].inverse_transform(predictions)
            else:
                predictions_decoded = predictions
            
            # Add predictions to original data
            result_df = self.data.copy()
            result_df['Predicted_Class'] = predictions_decoded
            
            # Add prediction probabilities
            if hasattr(model, 'predict_proba'):
                probabilities = model.predict_proba(self.processed_data[0])
                result_df['Prediction_Confidence'] = np.max(probabilities, axis=1)
            
            return result_df
            
        except Exception as e:
            st.error(f"Error making predictions: {str(e)}")
            return None

def create_metrics_dashboard(classification_system: DataClassificationSystem):
    """Create enhanced metrics visualization dashboard"""
    if not classification_system.metrics:
        st.warning("No metrics available. Please classify data first.")
        return
    
    st.subheader("📊 Classification Metrics")
    
    # Get complexity level from first model
    complexity_level = list(classification_system.metrics.values())[0].get('complexity_level', 'unknown')
    st.info(f"🎚️ **Model Complexity Level:** {complexity_level.upper()}")
    
    # Metrics comparison table
    metrics_df = pd.DataFrame({
        model: {
            'Accuracy': metrics['accuracy'],
            'Precision': metrics['precision'],
            'Recall': metrics['recall'],
            'F1-Score': metrics['f1_score']
        }
        for model, metrics in classification_system.metrics.items()
    }).round(4)
    
    st.write("**Model Performance Comparison**")
    
    # Enhanced table with performance indicators
    styled_df = metrics_df.style.highlight_max(axis=1, color='lightgreen')
    st.dataframe(styled_df, use_container_width=True)
    
    # Performance insights
    best_accuracy = metrics_df.loc['Accuracy'].max()
    best_f1 = metrics_df.loc['F1-Score'].max()
    
    col1, col2, col3 = st.columns(3)
    with col1:
        st.metric("Best Accuracy", f"{best_accuracy:.4f}")
    with col2:
        st.metric("Best F1-Score", f"{best_f1:.4f}")
    with col3:
        performance_level = "Excellent" if best_f1 > 0.9 else "Good" if best_f1 > 0.8 else "Fair" if best_f1 > 0.7 else "Needs Improvement"
        st.metric("Performance Level", performance_level)
    
    # Visualizations
    col1, col2 = st.columns(2)
    
    with col1:
        # Enhanced performance comparison chart
        fig = px.bar(
            metrics_df.T.reset_index(),
            x='index',
            y=['Accuracy', 'Precision', 'Recall', 'F1-Score'],
            title="Model Performance Comparison",
            barmode='group',
            color_discrete_sequence=['#FF6B6B', '#4ECDC4', '#45B7D1', '#96CEB4']
        )
        fig.update_layout(
            xaxis_title="Models", 
            yaxis_title="Score",
            legend_title="Metrics",
            hovermode='x unified'
        )
        fig.update_traces(texttemplate='%{y:.3f}', textposition='outside')
        st.plotly_chart(fig, use_container_width=True)
    
    with col2:
        # Best model identification
        best_model = metrics_df.loc['F1-Score'].idxmax()
        st.metric("🏆 Best Performing Model", best_model, f"F1-Score: {metrics_df.loc['F1-Score', best_model]:.4f}")
        
        # Model configuration details
        if best_model in classification_system.metrics and 'model_params' in classification_system.metrics[best_model]:
            with st.expander(f"🔧 {best_model} Configuration"):
                params = classification_system.metrics[best_model]['model_params']
                for param, value in params.items():
                    if param != 'random_state':  # Skip random_state for clarity
                        st.write(f"• **{param}**: {value}")
        
        # Confusion matrix for best model
        if best_model in classification_system.metrics:
            cm = classification_system.metrics[best_model]['confusion_matrix']
            
            # Create more informative confusion matrix
            if 'target' in classification_system.encoders:
                class_names = classification_system.encoders['target'].classes_
            else:
                class_names = [f"Class {i}" for i in range(cm.shape[0])]
            
            fig = px.imshow(
                cm, 
                text_auto=True, 
                aspect="auto", 
                title=f"Confusion Matrix - {best_model}",
                labels=dict(x="Predicted", y="Actual"),
                x=class_names,
                y=class_names,
                color_continuous_scale='Blues'
            )
            fig.update_layout(width=400, height=400)
            st.plotly_chart(fig, use_container_width=True)
    
    # Detailed performance analysis
    with st.expander("📈 Detailed Performance Analysis"):
        
        # Model comparison radar chart
        if len(classification_system.metrics) >= 2:
            st.write("**Multi-Model Performance Radar Chart:**")
            
            categories = ['Accuracy', 'Precision', 'Recall', 'F1-Score']
            fig = go.Figure()
            
            colors = ['#FF6B6B', '#4ECDC4', '#45B7D1', '#96CEB4', '#FECA57']
            
            for i, (model_name, metrics) in enumerate(classification_system.metrics.items()):
                values = [metrics['accuracy'], metrics['precision'], metrics['recall'], metrics['f1_score']]
                
                fig.add_trace(go.Scatterpolar(
                    r=values,
                    theta=categories,
                    fill='toself',
                    name=model_name,
                    line_color=colors[i % len(colors)],
                    opacity=0.7
                ))
            
            fig.update_layout(
                polar=dict(
                    radialaxis=dict(
                        visible=True,
                        range=[0, 1]
                    )),
                showlegend=True,
                title="Model Performance Comparison (Radar Chart)"
            )
            st.plotly_chart(fig, use_container_width=True)
        
        # Performance recommendations
        st.write("**🎯 Performance Recommendations:**")
        
        if best_f1 > 0.95:
            st.success("🌟 **Excellent Performance!** Your model is performing exceptionally well.")
            st.info("• Consider using this model for production")
            st.info("• Monitor for overfitting with new data")
        elif best_f1 > 0.85:
            st.success("✅ **Good Performance!** Your model is performing well.")
            st.info("• Model is ready for most practical applications")
            st.info("• Consider fine-tuning hyperparameters for marginal improvements")
        elif best_f1 > 0.70:
            st.warning("⚠️ **Fair Performance.** There's room for improvement.")
            st.info("• Try feature engineering or data preprocessing")
            st.info("• Consider collecting more training data")
            st.info("• Experiment with different algorithms")
        else:
            st.error("🔄 **Needs Improvement.** Consider data quality and feature selection.")
            st.info("• Check data quality and target column selection")
            st.info("• Verify if the problem is suitable for classification")
            st.info("• Consider domain expertise for feature engineering")
    
    # Classification reports
    with st.expander("📋 Detailed Classification Reports"):
        for model, metrics in classification_system.metrics.items():
            st.write(f"**{model} - Detailed Report:**")
            st.text(metrics['classification_report'])
            st.write("---")

def main():
    st.set_page_config(page_title="Data Classification Dashboard", layout="wide")
    
    st.title("🤖 Data Classification Dashboard")
    st.markdown("**Upload your CSV/Excel file and perform binary or multi-class classification with comprehensive metrics**")
    
    # Initialize session state
    if 'classification_system' not in st.session_state:
        st.session_state.classification_system = DataClassificationSystem()
    
    classification_system = st.session_state.classification_system
    
    # Sidebar for file upload and configuration
    with st.sidebar:
        st.header("⚙️ Configuration")
        
        # File upload
        uploaded_file = st.file_uploader(
            "Upload CSV or Excel file",
            type=['csv', 'xlsx', 'xls'],
            help="Upload your dataset for classification"
        )
        
        if uploaded_file:
            # Load data
            data = classification_system.load_data(uploaded_file)
            if data is not None:
                classification_system.data = data
                st.success(f"✅ File loaded: {data.shape[0]} rows, {data.shape[1]} columns")
                
                # Show column info with recommendations
                st.write("**Available Columns:**")
                
                # Get target suggestions
                suggestions = classification_system.suggest_target_columns(data)
                suggested_cols = [s['column'] for s in suggestions]
                
                for col in data.columns:
                    unique_count = data[col].nunique()
                    col_type = str(data[col].dtype)
                    
                    # Color code based on suitability
                    if col in suggested_cols:
                        st.write(f"✅ **{col}** ({col_type}) - {unique_count:,} unique values - *Recommended for classification*")
                    elif any(indicator in col.lower() for indicator in ['id', '_key', 'key_']):
                        st.write(f"🔴 {col} ({col_type}) - {unique_count:,} unique values - *ID column (not suitable)*")
                    elif unique_count > data.shape[0] * 0.1:
                        st.write(f"🟡 {col} ({col_type}) - {unique_count:,} unique values - *Too many categories*")
                    else:
                        st.write(f"• {col} ({col_type}) - {unique_count:,} unique values")
                
                # Show recommendations prominently
                if suggestions:
                    st.success("🎯 **Recommended Target Columns:**")
                    for suggestion in suggestions[:5]:  # Show top 5
                        st.write(f"• **{suggestion['column']}** - {suggestion['unique_count']} classes, min class size: {suggestion['min_class_size']}")
                else:
                    st.warning("⚠️ No ideal target columns found. Look for columns with 2-50 distinct categories.")
                
                # Target column selection with better filtering
                st.write("**Select Target Column:**")
                
                # Filter out obvious non-targets for the dropdown
                suitable_columns = []
                problematic_columns = []
                
                for col in data.columns:
                    if any(indicator in col.lower() for indicator in ['id', '_key', 'key_']):
                        problematic_columns.append(col)
                    elif data[col].nunique() > data.shape[0] * 0.1:
                        problematic_columns.append(col)
                    else:
                        suitable_columns.append(col)
                
                # Show suitable columns first, then problematic ones
                all_columns = suitable_columns + ['---PROBLEMATIC COLUMNS---'] + problematic_columns
                
                target_column = st.selectbox(
                    "Choose the column you want to predict",
                    options=all_columns,
                    help="Green ✅ columns are recommended. Red 🔴 columns are typically not suitable for classification."
                )
                
                # Validate selection
                if target_column == '---PROBLEMATIC COLUMNS---':
                    st.error("Please select a valid column (not the separator)")
                    target_column = None
                elif target_column in problematic_columns:
                    st.warning(f"⚠️ '{target_column}' may not be suitable for classification. Consider using a recommended column instead.")
                
                # Show target column info
                if target_column and target_column != '---PROBLEMATIC COLUMNS---':
                    target_info = data[target_column].value_counts()
                    st.write(f"**Target '{target_column}' distribution:**")
                    
                    # Show distribution
                    col1, col2 = st.columns(2)
                    with col1:
                        for value, count in target_info.head(10).items():
                            percentage = (count / len(data)) * 100
                            st.write(f"• {value}: {count:,} samples ({percentage:.1f}%)")
                    
                    with col2:
                        if len(target_info) <= 10:
                            fig = px.pie(values=target_info.values, names=target_info.index, 
                                       title=f"Distribution of {target_column}")
                            st.plotly_chart(fig, use_container_width=True)
                    
                    if len(target_info) > 10:
                        st.write(f"... and {len(target_info) - 10} more classes")
                    
                    # Show suitability assessment
                    uniqueness = target_info.nunique() / len(data)
                    min_class_size = target_info.min()
                    
                    if uniqueness <= 0.01 and min_class_size >= 10:
                        st.success(f"✅ Excellent target column: {target_info.nunique()} classes, min class size: {min_class_size}")
                    elif uniqueness <= 0.05 and min_class_size >= 5:
                        st.info(f"ℹ️ Good target column: {target_info.nunique()} classes, min class size: {min_class_size}")
                    elif uniqueness <= 0.1 and min_class_size >= 2:
                        st.warning(f"⚠️ Acceptable target column: {target_info.nunique()} classes, min class size: {min_class_size}")
                    else:
                        st.error(f"❌ Poor target column: {target_info.nunique()} classes, min class size: {min_class_size}")
                        st.info("Consider selecting a different column for better results.")
                
                # Task type selection
                task_type = st.radio(
                    "Classification Type",
                    options=['auto', 'binary', 'multiclass'],
                    help="Choose classification type or let system auto-detect"
                )
                
                # Store configuration
                if target_column and target_column != '---PROBLEMATIC COLUMNS---':
                    st.session_state.target_column = target_column
                    st.session_state.task_type = task_type
                else:
                    if 'target_column' in st.session_state:
                        del st.session_state.target_column
    
    # Main dashboard
    if classification_system.data is not None:
        # Tabs for different views
        tab1, tab2, tab3, tab4 = st.tabs(["📋 Data Grid", "🎯 Classification", "📊 Metrics", "🔍 Analysis"])
        
        with tab1:
            st.subheader("📋 Dataset Overview")
            
            # Search functionality
            col1, col2, col3 = st.columns([3, 1, 1])
            with col1:
                search_term = st.text_input("🔍 Search in data", placeholder="Enter search term...")
            with col2:
                search_column = st.selectbox("Search in column", options=['All'] + classification_system.data.columns.tolist())
            with col3:
                if st.button("🔍 Search", type="primary"):
                    if search_term:
                        if search_column == 'All':
                            mask = classification_system.data.astype(str).apply(lambda x: x.str.contains(search_term, case=False, na=False)).any(axis=1)
                        else:
                            mask = classification_system.data[search_column].astype(str).str.contains(search_term, case=False, na=False)
                        filtered_data = classification_system.data[mask]
                        st.write(f"Found {len(filtered_data)} matching records")
                        st.dataframe(filtered_data, use_container_width=True)
                    else:
                        st.dataframe(classification_system.data, use_container_width=True)
            
            if not search_term:
                # Display full dataset
                st.dataframe(classification_system.data, use_container_width=True)
                
                # Dataset statistics
                col1, col2, col3, col4 = st.columns(4)
                with col1:
                    st.metric("Total Records", len(classification_system.data))
                with col2:
                    st.metric("Total Columns", len(classification_system.data.columns))
                with col3:
                    st.metric("Missing Values", classification_system.data.isnull().sum().sum())
                with col4:
                    st.metric("Memory Usage", f"{classification_system.data.memory_usage(deep=True).sum() / 1024:.1f} KB")
        
        with tab2:
            st.subheader("🎯 Data Classification")
            
            col1, col2, col3 = st.columns([2, 2, 2])
            
            with col1:
                if st.button("🚀 Classify Data", type="primary", use_container_width=True):
                    if hasattr(st.session_state, 'target_column'):
                        with st.spinner("Processing and training models..."):
                            try:
                                # Preprocess data
                                X, y = classification_system.preprocess_data(
                                    classification_system.data, 
                                    st.session_state.target_column
                                )
                                
                                if X is not None and y is not None:
                                    classification_system.processed_data = (X, y)
                                    
                                    # Train models
                                    classification_system.train_models(X, y, st.session_state.task_type)
                                    
                                    if classification_system.metrics:
                                        st.rerun()
                                else:
                                    st.error("❌ Error in data preprocessing. Please check your target column selection.")
                            except Exception as e:
                                st.error(f"❌ Classification error: {str(e)}")
                                st.info("💡 Suggestions:")
                                st.info("• Try selecting a different target column")
                                st.info("• Check if your target column has meaningful categories")
                                st.info("• Ensure your data has enough samples per class")
                    else:
                        st.error("❌ Please select a target column first")
            
            with col2:
                if st.button("👥 Group Data", use_container_width=True):
                    if hasattr(st.session_state, 'target_column'):
                        target_col = st.session_state.target_column
                        if target_col in classification_system.data.columns:
                            grouped = classification_system.data[target_col].value_counts().reset_index()
                            grouped.columns = [target_col, 'Count']
                            st.write(f"**Data Distribution by '{target_col}':**")
                            st.dataframe(grouped, use_container_width=True)
                            
                            # Visualization
                            if len(grouped) <= 20:  # Only show pie chart if not too many categories
                                fig = px.pie(grouped, values='Count', names=target_col, title=f"Distribution of {target_col}")
                                st.plotly_chart(fig, use_container_width=True)
                            else:
                                fig = px.bar(grouped.head(20), x=target_col, y='Count', title=f"Top 20 Categories in {target_col}")
                                st.plotly_chart(fig, use_container_width=True)
                                st.info(f"Showing top 20 out of {len(grouped)} categories")
                        else:
                            st.error(f"Column '{target_col}' not found")
                    else:
                        st.error("Please select a target column first")
            
            with col3:
                # Model selection for prediction
                if classification_system.models:
                    selected_model = st.selectbox("Select Model for Prediction", options=list(classification_system.models.keys()))
                    
                    if st.button("🔮 Generate Predictions", use_container_width=True):
                        predictions_df = classification_system.predict_new_data(selected_model)
                        if predictions_df is not None:
                            st.write(f"**Predictions using {selected_model}:**")
                            st.dataframe(predictions_df, use_container_width=True)
                            
                            # Download predictions
                            csv = predictions_df.to_csv(index=False)
                            st.download_button(
                                label="📥 Download Predictions",
                                data=csv,
                                file_name="predictions.csv",
                                mime="text/csv"
                            )
                        else:
                            st.error("Failed to generate predictions")
                else:
                    st.info("Train models first to generate predictions")
        
        with tab3:
            if classification_system.metrics:
                create_metrics_dashboard(classification_system)
            else:
                st.info("📊 No metrics available yet. Please run classification first.")
                
                # Show sample of what metrics will look like
                st.write("**Metrics will include:**")
                st.write("• Model performance comparison (Accuracy, Precision, Recall, F1-Score)")
                st.write("• Confusion matrices")
                st.write("• Detailed classification reports")
                st.write("• Performance visualizations")
        
        with tab4:
            st.subheader("🔍 Data Analysis")
            
            if classification_system.data is not None:
                # Data profiling
                col1, col2 = st.columns(2)
                
                with col1:
                    st.write("**Data Types:**")
                    dtype_df = pd.DataFrame({
                        'Column': classification_system.data.dtypes.index,
                        'Data Type': classification_system.data.dtypes.values,
                        'Unique Values': [classification_system.data[col].nunique() for col in classification_system.data.columns],
                        'Non-Null Count': [classification_system.data[col].count() for col in classification_system.data.columns]
                    })
                    st.dataframe(dtype_df, use_container_width=True)
                
                with col2:
                    st.write("**Missing Values Analysis:**")
                    missing_df = pd.DataFrame({
                        'Column': classification_system.data.columns,
                        'Missing Count': classification_system.data.isnull().sum().values,
                        'Missing %': (classification_system.data.isnull().sum() / len(classification_system.data) * 100).round(2).values
                    })
                    missing_df = missing_df[missing_df['Missing Count'] > 0]
                    if len(missing_df) > 0:
                        st.dataframe(missing_df, use_container_width=True)
                    else:
                        st.success("✅ No missing values found!")
                
                # Correlation analysis for numeric columns
                numeric_cols = classification_system.data.select_dtypes(include=[np.number]).columns
                if len(numeric_cols) > 1:
                    st.write("**Correlation Matrix:**")
                    corr_matrix = classification_system.data[numeric_cols].corr()
                    fig = px.imshow(corr_matrix, text_auto=True, aspect="auto", title="Feature Correlation Matrix")
                    st.plotly_chart(fig, use_container_width=True)
                
                # Data quality insights
                st.write("**Data Quality Insights:**")
                insights = []
                
                # Check for potential ID columns
                potential_ids = [col for col in classification_system.data.columns 
                               if col.lower().endswith('id') or 
                               classification_system.data[col].nunique() == len(classification_system.data)]
                if potential_ids:
                    insights.append(f"🆔 Potential ID columns detected: {potential_ids}")
                
                # Check for high cardinality columns
                high_card_cols = [col for col in classification_system.data.columns 
                                if classification_system.data[col].nunique() > len(classification_system.data) * 0.5]
                if high_card_cols:
                    insights.append(f"📊 High cardinality columns (may not be suitable as targets): {high_card_cols}")
                
                # Check for constant columns
                constant_cols = [col for col in classification_system.data.columns 
                               if classification_system.data[col].nunique() <= 1]
                if constant_cols:
                    insights.append(f"⚠️ Constant columns (no variation): {constant_cols}")
                
                # Check for recommended target columns
                good_targets = []
                for col in classification_system.data.columns:
                    unique_count = classification_system.data[col].nunique()
                    if 2 <= unique_count <= 20 and not col.lower().endswith('id'):
                        good_targets.append(f"{col} ({unique_count} classes)")
                
                if good_targets:
                    insights.append(f"🎯 Recommended target columns: {good_targets}")
                
                for insight in insights:
                    st.info(insight)
    
    else:
        st.info("👆 Please upload a CSV or Excel file to get started")
        
        # Instructions and sample data format
        st.subheader("📝 How to Use This Dashboard")
        
        with st.expander("📋 Step-by-Step Instructions", expanded=True):
            st.write("""
            **1. Upload Your Data**
            - Click "Browse files" in the sidebar
            - Upload a CSV or Excel file (max 200MB)
            - Supported formats: .csv, .xlsx, .xls
            
            **2. Select Target Column**
            - Choose the column you want to predict
            - Avoid ID columns or columns with too many unique values
            - Best targets have 2-20 distinct categories
            
            **3. Configure Classification**
            - Select binary, multiclass, or auto-detect
            - Review data distribution in the sidebar
            
            **4. Run Classification**
            - Click "🚀 Classify Data" to train models
            - View results in the Metrics tab
            - Generate predictions with trained models
            
            **5. Analyze Results**
            - Compare model performance
            - View confusion matrices
            - Download predictions as CSV
            """)
        
        with st.expander("📊 Your Employee Dataset - Recommended Targets"):
            st.write("**Based on your employee dataset, here are the best target columns for classification:**")
            
            recommended_targets = [
                ("STATUS", "Employee status (Active/Inactive) - Perfect for binary classification"),
                ("department_name", "Department classification - Predict employee department"),
                ("BUSINESS_UNIT", "Business unit classification - Predict business unit"),
                ("gender_short", "Gender classification - Binary classification"),
                ("termtype_desc", "Termination type - Predict termination category"),
                ("termreason_desc", "Termination reason - Predict why employees leave")
            ]
            
            for target, description in recommended_targets:
                st.write(f"✅ **{target}**: {description}")
            
            st.write("**❌ Avoid these columns as targets:**")
            avoid_targets = [
                ("EmployeeID", "Unique identifier - each employee has a different ID"),
                ("recorddate_key", "Date field - too many unique values"),
                ("birthdate_key", "Date field - too many unique values"),
                ("orighiredate_key", "Date field - too many unique values"),
                ("terminationdate_key", "Date field - too many unique values"),
                ("age", "Continuous variable - better as a feature"),
                ("length_of_service", "Continuous variable - better as a feature"),
                ("store_name", "If it's a store ID number, likely too many unique values")
            ]
            
            for target, reason in avoid_targets:
                st.write(f"🔴 **{target}**: {reason}")
        
        with st.expander("⚠️ Common Issues and Solutions"):
            st.write("""
            **Issue: "Target column has too many unique values"**
            - Solution: Choose a column with categories, not continuous values or IDs
            
            **Issue: "Some classes have too few samples"**
            - Solution: Ensure each category has at least 2-3 samples
            
            **Issue: "ValueError during classification"**
            - Solution: Check for missing values and data quality issues
            
            **Issue: "No valid features found"**
            - Solution: Ensure your data has numeric or text columns besides the target
            """)

if __name__ == "__main__":
    main()