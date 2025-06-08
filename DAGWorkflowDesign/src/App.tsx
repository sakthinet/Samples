import React, { useState, useCallback, useRef, CSSProperties } from 'react';
import {
  ReactFlow,
  MiniMap,
  Controls,
  Background,
  useNodesState,
  useEdgesState,
  addEdge,
  Panel,
  Node,
  Edge,
  Connection,
  ReactFlowInstance,
  NodeTypes,
  BackgroundVariant,
  MarkerType,
  Handle,
  Position,
  EdgeProps,
  getBezierPath,
} from 'reactflow';
import 'reactflow/dist/style.css';
import { 
  Database, 
  Play, 
  Download,
  Upload,
  Shuffle,
  Filter,
  Code,
  Globe,
  LucideIcon,
  Moon,
  Sun,
  Trash2,
  Undo2,
  Redo2,
  X
} from 'lucide-react';

// Type definitions
interface NodeData {
  label: string;
  processId: number;
  [key: string]: any;
}

interface ToolboxItem {
  type: string;
  label: string;
  icon: LucideIcon;
  color: string;
}

interface WorkflowDefinition {
  nodes: Array<{
    id: string;
    type: string;
    position: { x: number; y: number };
    data: NodeData;
  }>;
  edges: Array<{
    source: string;
    target: string;
  }>;
}

interface HistoryState {
  nodes: Node[];
  edges: Edge[];
  timestamp: number;
}

// Theme definitions
const lightTheme = {
  background: '#f9fafb',
  sidebarBg: '#f5f5f5',
  cardBg: 'white',
  border: '#d1d5db',
  text: '#374151',
  textSecondary: '#6b7280',
  primary: '#2563eb'
};

const darkTheme = {
  background: '#1f2937',
  sidebarBg: '#111827',
  cardBg: '#374151',
  border: '#4b5563',
  text: '#f9fafb',
  textSecondary: '#d1d5db',
  primary: '#3b82f6'
};

// Inline styles with theme support
const createStyles = (theme: typeof lightTheme): { [key: string]: CSSProperties } => ({
  container: {
    height: '100vh',
    display: 'flex',
    fontFamily: 'Arial, sans-serif',
    backgroundColor: theme.background,
    color: theme.text
  },
  sidebar: {
    width: '256px',
    backgroundColor: theme.sidebarBg,
    borderRight: `1px solid ${theme.border}`,
    padding: '16px'
  },
  toolboxItem: {
    display: 'flex',
    alignItems: 'center',
    gap: '12px',
    padding: '12px',
    backgroundColor: theme.cardBg,
    borderRadius: '8px',
    border: `1px solid ${theme.border}`,
    cursor: 'grab',
    marginBottom: '8px',
    transition: 'all 0.2s',
    color: theme.text
  },
  configPanel: {
    width: '320px',
    backgroundColor: theme.cardBg,
    borderLeft: `1px solid ${theme.border}`,
    padding: '16px'
  },
  input: {
    width: '100%',
    padding: '8px',
    border: `1px solid ${theme.border}`,
    borderRadius: '6px',
    fontSize: '14px',
    backgroundColor: theme.cardBg,
    color: theme.text
  },
  textarea: {
    width: '100%',
    padding: '8px',
    border: `1px solid ${theme.border}`,
    borderRadius: '6px',
    fontSize: '14px',
    fontFamily: 'monospace',
    resize: 'vertical' as const,
    backgroundColor: theme.cardBg,
    color: theme.text
  },
  button: {
    display: 'flex',
    alignItems: 'center',
    gap: '8px',
    width: '100%',
    padding: '12px',
    backgroundColor: theme.primary,
    color: 'white',
    border: 'none',
    borderRadius: '8px',
    cursor: 'pointer',
    fontSize: '14px',
    fontWeight: '500'
  },
  themeButton: {
    display: 'flex',
    alignItems: 'center',
    gap: '8px',
    width: '100%',
    padding: '8px',
    backgroundColor: 'transparent',
    color: theme.text,
    border: `1px solid ${theme.border}`,
    borderRadius: '6px',
    cursor: 'pointer',
    fontSize: '12px',
    marginBottom: '12px'
  },
  label: {
    display: 'block',
    fontSize: '14px',
    fontWeight: '500',
    color: theme.text,
    marginBottom: '8px'
  },
  undoRedoButton: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    padding: '8px',
    backgroundColor: theme.cardBg,
    color: theme.text,
    border: `1px solid ${theme.border}`,
    borderRadius: '6px',
    cursor: 'pointer',
    fontSize: '12px',
    marginRight: '8px',
    minWidth: '36px',
    height: '36px'
  }
});

// Custom Deletable Edge Component
const DeletableEdge = ({ 
  id, 
  sourceX, 
  sourceY, 
  targetX, 
  targetY, 
  sourcePosition, 
  targetPosition, 
  style = {}, 
  markerEnd,
  data
}: EdgeProps) => {
  const [edgePath, labelX, labelY] = getBezierPath({
    sourceX,
    sourceY,
    sourcePosition,
    targetX,
    targetY,
    targetPosition,
  });

  const onEdgeClick = (evt: React.MouseEvent, edgeId: string) => {
    evt.stopPropagation();
    if (data?.onDelete) {
      data.onDelete(edgeId);
    }
  };

  return (
    <>
      <path
        id={id}
        style={style}
        className="react-flow__edge-path"
        d={edgePath}
        markerEnd={markerEnd}
      />
      <g transform={`translate(${labelX}, ${labelY})`}>
        <circle
          r={10}
          fill="#dc2626"
          stroke="white"
          strokeWidth={2}
          style={{ cursor: 'pointer' }}
          onClick={(evt) => onEdgeClick(evt, id)}
        />
        <X
          size={12}
          color="white"
          x={-6}
          y={-6}
          style={{ pointerEvents: 'none' }}
        />
      </g>
    </>
  );
};

// Custom Node Components with theme, process ID, and connection handles
const createNodeComponent = (bgColor: string, borderColor: string, iconColor: string, nodeType: string) => 
  ({ data, selected }: { data: NodeData; selected?: boolean }) => (
    <div style={{
      padding: '8px 16px',
      boxShadow: selected ? '0 0 0 2px #3b82f6' : '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
      borderRadius: '8px',
      backgroundColor: bgColor,
      border: `2px solid ${selected ? '#3b82f6' : borderColor}`,
      minWidth: '140px',
      position: 'relative'
    }}>
      {/* Connection Handles */}
      <Handle
        type="target"
        position={Position.Left}
        style={{
          background: iconColor,
          width: '8px',
          height: '8px',
          border: '2px solid white',
          left: '-4px'
        }}
      />
      <Handle
        type="source"
        position={Position.Right}
        style={{
          background: iconColor,
          width: '8px',
          height: '8px',
          border: '2px solid white',
          right: '-4px'
        }}
      />
      
      {/* Process ID Badge */}
      <div style={{
        position: 'absolute',
        top: '-8px',
        right: '-8px',
        backgroundColor: iconColor,
        color: 'white',
        borderRadius: '50%',
        width: '20px',
        height: '20px',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        fontSize: '10px',
        fontWeight: 'bold',
        zIndex: 10
      }}>
        {data.processId}
      </div>
      
      {/* Delete Button (shows when selected) */}
      {selected && (
        <div style={{
          position: 'absolute',
          top: '-8px',
          left: '-8px',
          backgroundColor: '#dc2626',
          color: 'white',
          borderRadius: '50%',
          width: '20px',
          height: '20px',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          cursor: 'pointer',
          zIndex: 10
        }}
        onClick={(e) => {
          e.stopPropagation();
          if (data.onDelete) {
            data.onDelete(data.nodeId);
          }
        }}
        >
          <Trash2 size={10} />
        </div>
      )}
      
      <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
        {nodeType === 'fileInput' && <Upload size={16} color={iconColor} />}
        {nodeType === 'fileOutput' && <Download size={16} color={iconColor} />}
        {nodeType === 'database' && <Database size={16} color={iconColor} />}
        {nodeType === 'transform' && <Shuffle size={16} color={iconColor} />}
        {nodeType === 'filter' && <Filter size={16} color={iconColor} />}
        {nodeType === 'pythonScript' && <Code size={16} color={iconColor} />}
        {nodeType === 'restApi' && <Globe size={16} color={iconColor} />}
        <span style={{ fontSize: '12px', fontWeight: '500', color: iconColor }}>
          {nodeType === 'fileInput' && 'File Input'}
          {nodeType === 'fileOutput' && 'File Output'}
          {nodeType === 'database' && 'Database'}
          {nodeType === 'transform' && 'Transform'}
          {nodeType === 'filter' && 'Filter'}
          {nodeType === 'pythonScript' && 'Python Script'}
          {nodeType === 'restApi' && 'REST API'}
        </span>
      </div>
      <div style={{ fontSize: '11px', color: iconColor, marginTop: '4px' }}>
        {data.label || 'Node'}
      </div>
    </div>
  );

// Node component instances
const FileInputNode = createNodeComponent('#dbeafe', '#93c5fd', '#2563eb', 'fileInput');
const FileOutputNode = createNodeComponent('#dcfce7', '#86efac', '#16a34a', 'fileOutput');
const DatabaseNode = createNodeComponent('#f3e8ff', '#c084fc', '#9333ea', 'database');
const TransformNode = createNodeComponent('#fed7aa', '#fdba74', '#ea580c', 'transform');
const FilterNode = createNodeComponent('#fef3c7', '#fde047', '#ca8a04', 'filter');
const PythonScriptNode = createNodeComponent('#f9fafb', '#d1d5db', '#6b7280', 'pythonScript');
const RestApiNode = createNodeComponent('#fef2f2', '#fca5a5', '#dc2626', 'restApi');

// Node Types
const nodeTypes: NodeTypes = {
  fileInput: FileInputNode,
  fileOutput: FileOutputNode,
  database: DatabaseNode,
  transform: TransformNode,
  filter: FilterNode,
  pythonScript: PythonScriptNode,
  restApi: RestApiNode,
};

// Edge Types
const edgeTypes = {
  deletable: DeletableEdge,
};

// Empty initial state
const initialNodes: Node[] = [];
const initialEdges: Edge[] = [];

let processIdCounter = 0;

const WorkflowDesigner: React.FC = () => {
  const [nodes, setNodes, onNodesChange] = useNodesState(initialNodes);
  const [edges, setEdges, onEdgesChange] = useEdgesState(initialEdges);
  const [selectedNode, setSelectedNode] = useState<Node | null>(null);
  const [selectedNodes, setSelectedNodes] = useState<Node[]>([]);
  const [selectedEdges, setSelectedEdges] = useState<Edge[]>([]);
  const [showConfig, setShowConfig] = useState<boolean>(false);
  const [isDarkMode, setIsDarkMode] = useState<boolean>(false);
  const reactFlowWrapper = useRef<HTMLDivElement>(null);
  const [reactFlowInstance, setReactFlowInstance] = useState<ReactFlowInstance | null>(null);

  // History for undo/redo
  const [history, setHistory] = useState<HistoryState[]>([{ nodes: [], edges: [], timestamp: Date.now() }]);
  const [historyIndex, setHistoryIndex] = useState<number>(0);

  const currentTheme = isDarkMode ? darkTheme : lightTheme;
  const styles = createStyles(currentTheme);

  // Save state to history
  const saveToHistory = useCallback((newNodes: Node[], newEdges: Edge[]) => {
    const newState: HistoryState = {
      nodes: JSON.parse(JSON.stringify(newNodes)),
      edges: JSON.parse(JSON.stringify(newEdges)),
      timestamp: Date.now()
    };
    
    // Remove any history after current index (when user makes new changes after undo)
    const newHistory = history.slice(0, historyIndex + 1);
    newHistory.push(newState);
    
    // Limit history to 50 states
    if (newHistory.length > 50) {
      newHistory.shift();
    } else {
      setHistoryIndex(historyIndex + 1);
    }
    
    setHistory(newHistory);
  }, [history, historyIndex]);

  // Undo function
  const undo = useCallback(() => {
    if (historyIndex > 0) {
      const newIndex = historyIndex - 1;
      const previousState = history[newIndex];
      setNodes(previousState.nodes);
      setEdges(previousState.edges);
      setHistoryIndex(newIndex);
      setSelectedNode(null);
      setShowConfig(false);
    }
  }, [historyIndex, history, setNodes, setEdges]);

  // Redo function
  const redo = useCallback(() => {
    if (historyIndex < history.length - 1) {
      const newIndex = historyIndex + 1;
      const nextState = history[newIndex];
      setNodes(nextState.nodes);
      setEdges(nextState.edges);
      setHistoryIndex(newIndex);
      setSelectedNode(null);
      setShowConfig(false);
    }
  }, [historyIndex, history, setNodes, setEdges]);

  // Handle keyboard shortcuts
  const onKeyDown = useCallback((event: KeyboardEvent) => {
    // Undo: Ctrl+Z (Cmd+Z on Mac)
    if ((event.ctrlKey || event.metaKey) && event.key === 'z' && !event.shiftKey) {
      event.preventDefault();
      undo();
      return;
    }

    // Redo: Ctrl+Y or Ctrl+Shift+Z (Cmd+Y or Cmd+Shift+Z on Mac)
    if ((event.ctrlKey || event.metaKey) && (event.key === 'y' || (event.key === 'z' && event.shiftKey))) {
      event.preventDefault();
      redo();
      return;
    }

    // Delete selected items
    if ((event.key === 'Delete' || event.key === 'Backspace') && (selectedNodes.length > 0 || selectedEdges.length > 0)) {
      event.preventDefault();
      
      const newNodes = nodes.filter(node => !selectedNodes.find(selectedNode => selectedNode.id === node.id));
      const newEdges = edges.filter(edge => 
        !selectedEdges.find(selectedEdge => selectedEdge.id === edge.id) &&
        !selectedNodes.find(selectedNode => selectedNode.id === edge.source || selectedNode.id === edge.target)
      );
      
      setNodes(newNodes);
      setEdges(newEdges);
      saveToHistory(newNodes, newEdges);
      setSelectedNode(null);
      setShowConfig(false);
    }
  }, [selectedNodes, selectedEdges, nodes, edges, setNodes, setEdges, saveToHistory, undo, redo]);

  // Add keyboard event listener
  React.useEffect(() => {
    document.addEventListener('keydown', onKeyDown);
    return () => {
      document.removeEventListener('keydown', onKeyDown);
    };
  }, [onKeyDown]);

  const deleteNode = useCallback((nodeId: string) => {
    const newNodes = nodes.filter((node) => node.id !== nodeId);
    const newEdges = edges.filter((edge) => edge.source !== nodeId && edge.target !== nodeId);
    
    setNodes(newNodes);
    setEdges(newEdges);
    saveToHistory(newNodes, newEdges);
    setSelectedNode(null);
    setShowConfig(false);
  }, [nodes, edges, setNodes, setEdges, saveToHistory]);

  const deleteEdge = useCallback((edgeId: string) => {
    const newEdges = edges.filter((edge) => edge.id !== edgeId);
    setEdges(newEdges);
    saveToHistory(nodes, newEdges);
  }, [edges, nodes, setEdges, saveToHistory]);

  const onConnect = useCallback(
    (params: Connection) => {
      // Add arrow markers to connections with delete functionality
      const newEdge = {
        ...params,
        id: `${params.source}-${params.target}-${Date.now()}`,
        type: 'deletable',
        animated: true,
        markerEnd: {
          type: MarkerType.ArrowClosed,
          width: 20,
          height: 20,
          color: currentTheme.primary,
        },
        style: {
          stroke: currentTheme.primary,
          strokeWidth: 2,
        },
        data: {
          onDelete: deleteEdge
        }
      };
      
      const newEdges = addEdge(newEdge, edges);
      setEdges(newEdges);
      saveToHistory(nodes, newEdges);
    },
    [edges, nodes, setEdges, saveToHistory, currentTheme.primary, deleteEdge]
  );

  const onSelectionChange = useCallback(({ nodes, edges }: { nodes: Node[]; edges: Edge[] }) => {
    setSelectedNodes(nodes);
    setSelectedEdges(edges);
  }, []);

  const onDragOver = useCallback((event: React.DragEvent) => {
    event.preventDefault();
    event.dataTransfer.dropEffect = 'move';
  }, []);

  const onDrop = useCallback(
    (event: React.DragEvent) => {
      event.preventDefault();

      const type = event.dataTransfer.getData('application/reactflow');
      if (typeof type === 'undefined' || !type) {
        return;
      }

      if (!reactFlowInstance) return;

      const position = reactFlowInstance.screenToFlowPosition({
        x: event.clientX,
        y: event.clientY,
      });

      const newNode: Node = {
        id: `${Date.now()}`,
        type,
        position,
        data: { 
          label: `${type} node`,
          processId: processIdCounter++,
          nodeId: `${Date.now()}`,
          onDelete: deleteNode
        },
      };

      const newNodes = nodes.concat(newNode);
      setNodes(newNodes);
      saveToHistory(newNodes, edges);
    },
    [reactFlowInstance, nodes, edges, setNodes, saveToHistory, deleteNode]
  );

  const onNodeClick = useCallback((event: React.MouseEvent, node: Node) => {
    setSelectedNode(node);
    setShowConfig(true);
  }, []);

  const nodeToolbox: ToolboxItem[] = [
    { type: 'fileInput', label: 'File Input', icon: Upload, color: '#2563eb' },
    { type: 'fileOutput', label: 'File Output', icon: Download, color: '#16a34a' },
    { type: 'database', label: 'Database', icon: Database, color: '#9333ea' },
    { type: 'restApi', label: 'REST API', icon: Globe, color: '#dc2626' },
    { type: 'transform', label: 'Transform', icon: Shuffle, color: '#ea580c' },
    { type: 'filter', label: 'Filter', icon: Filter, color: '#ca8a04' },
    { type: 'pythonScript', label: 'Python Script', icon: Code, color: '#6b7280' },
  ];

  const onDragStart = (event: React.DragEvent, nodeType: string) => {
    event.dataTransfer.setData('application/reactflow', nodeType);
    event.dataTransfer.effectAllowed = 'move';
  };

  const generateDAG = (): void => {
    if (nodes.length === 0) {
      alert('Please add some nodes to generate a DAG!');
      return;
    }

    // Sort nodes by process ID for proper execution order
    const sortedNodes = [...nodes].sort((a, b) => 
      (a.data as NodeData).processId - (b.data as NodeData).processId
    );

    const workflowDef: WorkflowDefinition = {
      nodes: sortedNodes.map(node => ({
        id: node.id,
        type: node.type || 'default',
        position: node.position,
        data: node.data as NodeData
      })),
      edges: edges.map(edge => ({
        source: edge.source,
        target: edge.target
      }))
    };

    let dagCode = `from airflow import DAG
from airflow.operators.python_operator import PythonOperator
from airflow.operators.bash_operator import BashOperator
from datetime import datetime, timedelta

default_args = {
    'owner': 'workflow-designer',
    'depends_on_past': False,
    'start_date': datetime(2025, 1, 1),
    'retries': 1,
    'retry_delay': timedelta(minutes=5),
}

dag = DAG(
    'generated_workflow',
    default_args=default_args,
    description='Generated from visual workflow',
    schedule_interval='@daily',
    catchup=False
)

`;

    sortedNodes.forEach(node => {
      const nodeData = node.data as NodeData;
      const taskId = `process_${nodeData.processId}_${node.type}`;
      
      dagCode += `
# Task ${nodeData.processId}: ${nodeData.label}
${taskId} = PythonOperator(
    task_id='${taskId}',
    python_callable=lambda: print('Executing Process ${nodeData.processId}: ${nodeData.label}'),
    dag=dag
)
`;
    });

    // Add dependencies based on process ID order
    if (sortedNodes.length > 1) {
      dagCode += '\n# Task Dependencies (based on process order)\n';
      for (let i = 0; i < sortedNodes.length - 1; i++) {
        const currentNode = sortedNodes[i];
        const nextNode = sortedNodes[i + 1];
        const currentTaskId = `process_${(currentNode.data as NodeData).processId}_${currentNode.type}`;
        const nextTaskId = `process_${(nextNode.data as NodeData).processId}_${nextNode.type}`;
        dagCode += `${currentTaskId} >> ${nextTaskId}\n`;
      }
    }

    alert(`Generated DAG Code:\n\n${dagCode}`);
  };

  const updateNodeLabel = (value: string): void => {
    if (!selectedNode) return;
    
    const updatedNodes = nodes.map(node =>
      node.id === selectedNode.id
        ? { ...node, data: { ...node.data, label: value } }
        : node
    );
    setNodes(updatedNodes);
    setSelectedNode({ ...selectedNode, data: { ...selectedNode.data, label: value } });
    saveToHistory(updatedNodes, edges);
  };

  const updateProcessId = (value: number): void => {
    if (!selectedNode) return;
    
    const updatedNodes = nodes.map(node =>
      node.id === selectedNode.id
        ? { ...node, data: { ...node.data, processId: value } }
        : node
    );
    setNodes(updatedNodes);
    setSelectedNode({ ...selectedNode, data: { ...selectedNode.data, processId: value } });
    saveToHistory(updatedNodes, edges);
  };

  const clearWorkflow = (): void => {
    const newNodes: Node[] = [];
    const newEdges: Edge[] = [];
    
    setNodes(newNodes);
    setEdges(newEdges);
    setSelectedNode(null);
    setShowConfig(false);
    processIdCounter = 0;
    
    // Save clear state to history
    saveToHistory(newNodes, newEdges);
  };

  return (
    <div style={styles.container}>
      {/* Sidebar */}
      <div style={styles.sidebar}>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '16px' }}>
          <h2 style={{ fontSize: '18px', fontWeight: '600', color: currentTheme.text, margin: 0 }}>
            Node Toolbox
          </h2>
          <button 
            style={styles.themeButton}
            onClick={() => setIsDarkMode(!isDarkMode)}
          >
            {isDarkMode ? <Sun size={14} /> : <Moon size={14} />}
            {isDarkMode ? 'Light' : 'Dark'}
          </button>
        </div>
        
        {/* Undo/Redo Controls */}
        <div style={{ display: 'flex', marginBottom: '16px', gap: '4px' }}>
          <button
            style={{
              ...styles.undoRedoButton,
              opacity: historyIndex > 0 ? 1 : 0.5,
              cursor: historyIndex > 0 ? 'pointer' : 'not-allowed'
            }}
            onClick={undo}
            disabled={historyIndex <= 0}
            title="Undo (Ctrl+Z)"
          >
            <Undo2 size={16} />
          </button>
          <button
            style={{
              ...styles.undoRedoButton,
              opacity: historyIndex < history.length - 1 ? 1 : 0.5,
              cursor: historyIndex < history.length - 1 ? 'pointer' : 'not-allowed'
            }}
            onClick={redo}
            disabled={historyIndex >= history.length - 1}
            title="Redo (Ctrl+Y)"
          >
            <Redo2 size={16} />
          </button>
          <div style={{ 
            fontSize: '12px', 
            color: currentTheme.textSecondary, 
            alignSelf: 'center',
            marginLeft: '8px',
            fontFamily: 'monospace'
          }}>
            {historyIndex + 1}/{history.length}
          </div>
        </div>
        
        <div>
          {nodeToolbox.map((tool) => (
            <div
              key={tool.type}
              style={styles.toolboxItem}
              draggable
              onDragStart={(event) => onDragStart(event, tool.type)}
            >
              <tool.icon size={20} color={tool.color} />
              <span style={{ fontSize: '14px', fontWeight: '500' }}>
                {tool.label}
              </span>
            </div>
          ))}
        </div>

        <div style={{ marginTop: '32px' }}>
          <h3 style={{ fontSize: '16px', fontWeight: '600', marginBottom: '12px', color: currentTheme.text }}>
            Actions
          </h3>
          <button style={styles.button} onClick={generateDAG}>
            <Play size={16} />
            Generate DAG
          </button>
          <button 
            style={{ ...styles.button, backgroundColor: '#dc2626', marginTop: '8px' }} 
            onClick={clearWorkflow}
          >
            Clear All
          </button>
          
          {selectedNodes.length > 0 && (
            <button 
              style={{ ...styles.button, backgroundColor: '#f59e0b', marginTop: '8px' }} 
              onClick={() => {
                const newNodes = nodes.filter(node => !selectedNodes.find(selectedNode => selectedNode.id === node.id));
                const newEdges = edges.filter(edge => 
                  !selectedNodes.find(selectedNode => selectedNode.id === edge.source || selectedNode.id === edge.target)
                );
                setNodes(newNodes);
                setEdges(newEdges);
                saveToHistory(newNodes, newEdges);
              }}
            >
              <Trash2 size={16} />
              Delete Selected ({selectedNodes.length})
            </button>
          )}
        </div>
      </div>

      {/* Main Canvas */}
      <div style={{ flex: 1, position: 'relative' }} ref={reactFlowWrapper}>
        <ReactFlow
          nodes={nodes}
          edges={edges}
          onNodesChange={onNodesChange}
          onEdgesChange={onEdgesChange}
          onConnect={onConnect}
          onInit={setReactFlowInstance}
          onDrop={onDrop}
          onDragOver={onDragOver}
          onNodeClick={onNodeClick}
          onSelectionChange={onSelectionChange}
          nodeTypes={nodeTypes}
          edgeTypes={edgeTypes}
          fitView
          style={{ backgroundColor: currentTheme.background }}
          selectNodesOnDrag={true}
          multiSelectionKeyCode="Control"
          deleteKeyCode="Delete"
        >
          <Controls style={{ background: currentTheme.cardBg }} />
          <MiniMap 
            nodeStrokeColor={currentTheme.border}
            nodeColor={currentTheme.cardBg}
            nodeBorderRadius={8}
            style={{ background: currentTheme.cardBg }}
          />
          <Background 
            variant={BackgroundVariant.Dots} 
            gap={12} 
            size={1}
            color={currentTheme.border}
          />
          
          <Panel position="top-left">
            <div style={{
              backgroundColor: currentTheme.cardBg,
              padding: '16px',
              borderRadius: '8px',
              boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
              border: `1px solid ${currentTheme.border}`
            }}>
              <h1 style={{ fontSize: '20px', fontWeight: 'bold', color: currentTheme.text, marginBottom: '8px' }}>
                Workflow Designer
              </h1>
              <p style={{ fontSize: '14px', color: currentTheme.textSecondary, margin: 0 }}>
                Drag nodes from the sidebar • Click edges to delete • Ctrl+Z/Y for undo/redo • Delete key to remove
              </p>
            </div>
          </Panel>
        </ReactFlow>
      </div>

      {/* Configuration Panel */}
      {showConfig && selectedNode && (
        <div style={styles.configPanel}>
          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '16px' }}>
            <h3 style={{ fontSize: '18px', fontWeight: '600', color: currentTheme.text }}>Node Configuration</h3>
            <button
              onClick={() => setShowConfig(false)}
              style={{ background: 'none', border: 'none', fontSize: '24px', cursor: 'pointer', color: currentTheme.textSecondary }}
            >
              ×
            </button>
          </div>
          
          <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
            <div>
              <label style={styles.label}>Process ID</label>
              <input
                type="number"
                value={(selectedNode.data as NodeData).processId || 0}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => updateProcessId(parseInt(e.target.value) || 0)}
                style={styles.input}
                min="0"
              />
              <small style={{ fontSize: '12px', color: currentTheme.textSecondary, display: 'block', marginTop: '4px' }}>
                Controls execution order in the workflow
              </small>
            </div>

            <div>
              <label style={styles.label}>Node Type</label>
              <input
                type="text"
                value={selectedNode.type || ''}
                disabled
                style={{ ...styles.input, opacity: 0.6 }}
              />
            </div>
            
            <div>
              <label style={styles.label}>Label</label>
              <input
                type="text"
                value={(selectedNode.data as NodeData).label || ''}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => updateNodeLabel(e.target.value)}
                style={styles.input}
                placeholder="Enter node label"
              />
            </div>

            {selectedNode.type === 'fileInput' && (
              <div>
                <label style={styles.label}>File Path</label>
                <input
                  type="text"
                  style={styles.input}
                  placeholder="/path/to/input/file"
                />
              </div>
            )}

            {selectedNode.type === 'restApi' && (
              <>
                <div>
                  <label style={styles.label}>API Endpoint</label>
                  <input
                    type="text"
                    style={styles.input}
                    placeholder="https://api.example.com/endpoint"
                  />
                </div>
                <div>
                  <label style={styles.label}>HTTP Method</label>
                  <select style={styles.input}>
                    <option value="GET">GET</option>
                    <option value="POST">POST</option>
                    <option value="PUT">PUT</option>
                    <option value="DELETE">DELETE</option>
                  </select>
                </div>
              </>
            )}

            {selectedNode.type === 'database' && (
              <>
                <div>
                  <label style={styles.label}>Connection String</label>
                  <input
                    type="text"
                    style={styles.input}
                    placeholder="postgresql://user:pass@host:port/db"
                  />
                </div>
                <div>
                  <label style={styles.label}>SQL Query</label>
                  <textarea
                    style={{ ...styles.textarea, height: '80px' }}
                    placeholder="SELECT * FROM table_name"
                  />
                </div>
              </>
            )}

            {selectedNode.type === 'pythonScript' && (
              <div>
                <label style={styles.label}>Python Code</label>
                <textarea
                  style={{ ...styles.textarea, height: '120px' }}
                  placeholder="def process_data(data):&#10;    # Your Python code here&#10;    return data"
                />
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default WorkflowDesigner;