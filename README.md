# GradeSheetApp
ASP.NET Core Web API, CsvHelper, LINQ, C#, JSON, Dependency Injection Designed and developed a RESTful Web API that allows faculty to upload student score sheets in CSV format and automatically calculates per-student average percentages, assigns letter grades (A–F), and computes class-level statistics such as average and standard deviation.

	• Grade Sheet Web APP
	This ASP.NET Core Web API application allows lecturers to upload a CSV file containing student scores and automatically calculates:
		• Each student's average percentage
		• Their corresponding letter grade (A–F, based on configurable thresholds)
		• The overall class average and standard deviation
	✅ Supports optional logic to exclude the lowest score for each student via query string.
	
	•  Features
		• ✅ Upload CSV file with dynamic column headers
		• ✅ Calculate average percentage per student
		• ✅ Assign letter grade based on grade boundaries from appsettings.json
		• ✅ Compute class average and standard deviation
		• ✅ Optional flag to exclude each student’s lowest mark
		• ✅ JSON result sorted by highest percentage
		• ✅ Testable via Swagger UI

	•  Technologies Used
		• ASP.NET Core 8 Web API
		• CsvHelper for CSV parsing
		• LINQ for calculations
		• Swagger (Swashbuckle) for interactive API UI
	
	• CSV Input Format
		• 1st column: Student ID (e.g., S001)
		• Remaining columns: Numeric scores (any number of assessments)
	Example:
	StudentId,Quiz1,Quiz2,Final
S001,70,60,64
S002,55,68,61.5
S003,78,72,76
	
	• Grade Boundaries Configuration
	In appsettings.json:
	
	"LetterGrades": {
  "A": 90,
  "B": 80,
  "C": 70,
  "D": 60,
  "E": 50
  // Below 50 → F
}

	You can change these values as a teacher/admin without touching any code.

	• Application Flow
		1. Lecturer opens Swagger UI.
		2. Uploads a .csv file via POST /api/gradesheet/calculate.
		3. Chooses whether to ?exclude-lowest=true.
		4. API:
			○ Parses and validates file
			○ Computes each student’s average and grade
			○ Calculates class stats (average, std. deviation)
		5. Returns a sorted JSON response with all results.
	
	• How to Run the App
	🔨 1. Clone the Repository
	
	bash
	CopyEdit
	git clone https://github.com/your-repo/GradeSheetApp.git
cd GradeSheetApp
	
	📦 2. Install Dependencies
	Make sure you have .NET SDK 8.0+ installed.
	
	bash
	CopyEdit
	dotnet restore
	
	🔍 3. Verify or Install Swagger Support
	Ensure your Program.cs includes:
	
	csharp
	CopyEdit
	builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
	...
	if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
	
	▶️ 4. Run the App
	
	bash
	CopyEdit
	dotnet run
	
	🌐 5. Open Swagger UI
	Visit:
	
	bash
	CopyEdit
	https://localhost:{PORT}/swagger
	Usually https://localhost:7281/swagger.
	
	🧪 6. Test the API
		1. Expand POST /api/gradesheet/calculate
		2. Click Try it out
		3. Upload your .csv file
		4. Set exclude-lowest to true or false
		5. Click Execute
	✅ Response: JSON result with students' percentages and grades
	
	• Sample Response
	
	{
  "classAverage": 67.38,
  "standardDeviation": 5.14,
  "students": [
   {
      "studentId": "S003",
      "percentage": 75.25,
      "letter": "C"
    },
    ...
  ]
}
	• Future Enhancements
		• Save data to database
		• Role-based login for teachers/admins
		• Export results to CSV or PDF
		• Web frontend with React/Blazor
		• Email/print report cards
	


![image](https://github.com/user-attachments/assets/f0845353-4f07-46a8-a2cf-c6ccb2852cc2)

