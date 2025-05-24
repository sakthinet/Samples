using static OcrDataClassificationWebApp.Services.MlClassifier;

namespace OcrDataClassificationWebApp.Services
{
    public static class SampleTrainingData
    {
        public static List<OCRData> Get() => new List<OCRData>
        {
            new OCRData { Metadata = "BANK_CHECK", JsonOutput = "contains account number, amount, signature", Category = "Financial" },
            new OCRData { Metadata = "ACCOUNT_STATEMENT", JsonOutput = "contains transactions, balance, account details", Category = "Financial" },
            new OCRData { Metadata = "CREDIT_CARD_STATEMENT", JsonOutput = "contains credit card number, transactions, payment due", Category = "Financial" },
            new OCRData { Metadata = "INVOICE_PAYMENT", JsonOutput = "contains payment amount, invoice number, due date", Category = "Financial" },
            new OCRData { Metadata = "BANK_RECEIPT", JsonOutput = "contains transaction details, account info, timestamp", Category = "Financial" },

	//Sales
	        new OCRData { Metadata = "PAY_IN_SHEET", JsonOutput = "contains payment details, legal references, verification", Category = "Sales" },
	
	        // Transport category
	        new OCRData { Metadata = "SHIPPING_INVOICE", JsonOutput = "contains tracking number, delivery address, shipping cost", Category = "Transport" },
            new OCRData { Metadata = "DELIVERY_NOTE", JsonOutput = "contains package details, delivery instructions, recipient", Category = "Transport" },
            new OCRData { Metadata = "BILL_OF_LADING", JsonOutput = "contains cargo details, shipping routes, vessel information", Category = "Transport" },
            new OCRData { Metadata = "FREIGHT_RECEIPT", JsonOutput = "contains freight charges, shipping details, carrier info", Category = "Transport" },
            new OCRData { Metadata = "TRANSPORT_MANIFEST", JsonOutput = "contains vehicle details, route information, cargo list", Category = "Transport" },
	
	// Healthcare category
	
	        new OCRData { Metadata = "EQUIPMENT_INSPECTION", JsonOutput = "contains medical equipment details, inspection results, compliance", Category = "Healthcare" },
            new OCRData { Metadata = "PATIENT_INTAKE", JsonOutput = "contains patient details, medical history, symptoms", Category = "Healthcare" },
            new OCRData { Metadata = "MEDICAL_REPORT", JsonOutput = "contains diagnosis, treatment plan, doctor recommendations", Category = "Healthcare" },
            new OCRData { Metadata = "PRESCRIPTION", JsonOutput = "contains medication details, dosage, patient name", Category = "Healthcare" },
	
	// Legal category
	        new OCRData { Metadata = "FORM_1040", JsonOutput = "contains taxpayer information, income details, deductions", Category = "Legal" },
            new OCRData { Metadata = "PROXY_VOTING", JsonOutput = "contains shareholder details, voting instructions, meeting date", Category = "Legal" },
            new OCRData { Metadata = "COMMERCIAL_LEASE_AGREEMENT", JsonOutput = "contains property details, lease terms, signatures", Category = "Legal" },
            new OCRData { Metadata = "CONTRACT", JsonOutput = "contains terms and conditions, party details, effective date", Category = "Legal" },
            new OCRData { Metadata = "PETITION_FORM", JsonOutput = "contains terms and conditions, party details, effective date", Category = "Legal" },
            new OCRData { Metadata = "PATENT", JsonOutput = "contains medical device specifications, diagrams, claims", Category = "Legal" },
	
	// General category
	        new OCRData { Metadata = "PHOTO_DOC", JsonOutput = "contains image of document, text extraction results", Category = "General" },
            new OCRData { Metadata = "PHOTO_TABLE", JsonOutput = "contains image of table, structured data extraction", Category = "General" },
            new OCRData { Metadata = "PHOTO_RECEIPT", JsonOutput = "contains image of receipt, item list, total amount", Category = "General" },
            new OCRData { Metadata = "GENERAL_LETTER", JsonOutput = "contains correspondence details, date, signature", Category = "General" },
            new OCRData { Metadata = "MEMO", JsonOutput = "contains brief message, sender details, date", Category = "General" },
	
	// Ranking category
	        new OCRData { Metadata = "TABLE", JsonOutput = "contains rows, columns, structured data", Category = "Ranking" },
            new OCRData { Metadata = "CHART", JsonOutput = "contains visual representation, data points, labels", Category = "Ranking" },
            new OCRData { Metadata = "RANKING", JsonOutput = "contains ordered list, scores, positions", Category = "Ranking" },
            new OCRData { Metadata = "LEADERBOARD", JsonOutput = "contains rankings, names, performance metrics", Category = "Ranking" },
            new OCRData { Metadata = "PERFORMANCE_METRICS", JsonOutput = "contains KPIs, comparative analysis, benchmarks", Category = "Ranking" },
	
	        // Education category
	        new OCRData { Metadata = "GLOSSARY", JsonOutput = "contains terms, definitions, references", Category = "Education" },
            new OCRData { Metadata = "SYLLABUS", JsonOutput = "contains course details, schedule, requirements", Category = "Education" },
            new OCRData { Metadata = "ACADEMIC_TRANSCRIPT", JsonOutput = "contains grades, courses, student information", Category = "Education" },
            new OCRData { Metadata = "LESSON_PLAN", JsonOutput = "contains learning objectives, activities, materials", Category = "Education" },
            new OCRData { Metadata = "EDUCATIONAL_CERTIFICATE", JsonOutput = "contains qualification details, institution, date", Category = "Education" },
	
	// Health & Food category
	        new OCRData { Metadata = "NUTRITION", JsonOutput = "contains calorie information, ingredients, serving size", Category = "Health & Food" },
            new OCRData { Metadata = "FOOD_LABEL", JsonOutput = "contains nutritional facts, allergens, ingredients", Category = "Health & Food" },
            new OCRData { Metadata = "MENU", JsonOutput = "contains food items, prices, descriptions", Category = "Health & Food" },
            new OCRData { Metadata = "DIET_PLAN", JsonOutput = "contains meal schedule, calorie targets, food recommendations", Category = "Health & Food" },
            new OCRData { Metadata = "RECIPE", JsonOutput = "contains ingredients, instructions, cooking time", Category = "Health & Food" },
	
	// Property category
	        new OCRData { Metadata = "REAL_ESTATE", JsonOutput = "contains property details, price, location", Category = "Property" },
            new OCRData { Metadata = "PROPERTY_DEED", JsonOutput = "contains ownership details, property description, signatures", Category = "Property" },
            new OCRData { Metadata = "MORTGAGE_DOCUMENT", JsonOutput = "contains loan terms, property details, payment schedule", Category = "Property" },
            new OCRData { Metadata = "PROPERTY_LISTING", JsonOutput = "contains property features, price, contact information", Category = "Property" },
            new OCRData { Metadata = "PROPERTY_ASSESSMENT", JsonOutput = "contains valuation details, tax information, property attributes", Category = "Property" },
	
	// Workforce category
	        new OCRData { Metadata = "SHIFT_SCHEDULE", JsonOutput = "contains work hours, employee assignments, dates", Category = "Workforce" },
            new OCRData { Metadata = "EMPLOYEE_RECORD", JsonOutput = "contains personnel details, employment history, skills", Category = "Workforce" },
            new OCRData { Metadata = "PAYROLL", JsonOutput = "contains salary details, deductions, employee information", Category = "Workforce" },
            new OCRData { Metadata = "JOB_DESCRIPTION", JsonOutput = "contains role details, responsibilities, requirements", Category = "Workforce" },
            new OCRData { Metadata = "PERFORMANCE_REVIEW", JsonOutput = "contains evaluation metrics, feedback, goals", Category = "Workforce" }
        };
    }
}
