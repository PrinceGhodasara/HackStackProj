using ClosedXML.Excel;
using HackStack___Gemini.Models;
using HackStack___Gemini.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[Route("api/[controller]")]
[ApiController]
public class RecommendationsController : ControllerBase
{
    private readonly GeminiService _geminiService;

    public RecommendationsController(GeminiService geminiService)
    {
        _geminiService = geminiService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File not found.");

        var excelData = new ExcelData
        {
            Employees = new List<Dictionary<string, string>>(),
            Departments = new List<Dictionary<string, string>>(),
            Projects = new List<Dictionary<string, string>>()
        };

        var employees = new List<Employee>();
        var projects = new List<Project>();
        var departments = new List<Departments>();
        var timesheets = new List<TimeSheet>();

        using var stream = file.OpenReadStream();
        using var workbook = new XLWorkbook(stream);

        foreach (var sheet in workbook.Worksheets)
        {
            var table = new List<Dictionary<string, string>>();
            var headers = sheet.Row(1).Cells().Select(c => c.Value.ToString()).ToList();

            foreach (var row in sheet.RowsUsed().Skip(1))
            {
                var rowData = new Dictionary<string, string>();
                for (int i = 0; i < headers.Count; i++)
                {
                    rowData[headers[i]] = row.Cell(i + 1).GetString();
                }
                table.Add(rowData);
            }
            int projectid = 0;
            if (sheet.Name.ToLower().Contains("employee"))
            {
                excelData.Employees = table;
                employees = table.Select(e => new Employee
                {
                    Id = e.ContainsKey("Id") ? e["Id"] : string.Empty,
                    Name = e.ContainsKey("Name") ? e["Name"] : string.Empty,
                    Email = e.ContainsKey("email") ? e["email"] : string.Empty,
                    DepatmentId = e.ContainsKey("depatmentId") ? e["depatmentId"] : string.Empty,
                    DateOfJoining = e.ContainsKey("date_of_joining") ? DateTime.Parse(e["date_of_joining"], null, System.Globalization.DateTimeStyles.None) : DateTime.MinValue,
                    Salary = e.ContainsKey("Salary") ? double.Parse(e["Salary"]) : 0,
                    IsBillable = e.ContainsKey("isBillable") ? e["isBillable"] == "TRUE" ? true : false : false,
                    ProjectId = e.ContainsKey("projectId") ? int.TryParse(e["projectId"], out projectid) ? projectid : 0 : 0
                }).ToList();
            }
            else if (sheet.Name.ToLower().Contains("project"))
            {
                excelData.Projects = table;
                projects = table.Select(p => new Project
                {
                    Id = p.ContainsKey("Id") ? p["Id"] : string.Empty,
                    Name = p.ContainsKey("name") ? p["name"] : string.Empty,
                    state = p.ContainsKey("state") ? p["state"] : string.Empty,
                    IsFixedCost = p.ContainsKey("isFixedCost") ? p["isFixedCost"] == "TRUE" ? true : false : false,
                    TotalHours = p.ContainsKey("total_hours") ? int.TryParse(p["total_hours"], out var totalHours) ? totalHours : 0 : 0,
                    Rate = p.ContainsKey("rate") ? double.TryParse(p["rate"], out var rate) ? rate : 0 : 0
                }).ToList();
            }
            else if (sheet.Name.ToLower().Contains("department"))
            {
                excelData.Departments = table;
                departments = table.Select(d => new Departments
                {
                    Id = d.ContainsKey("id") ? d["id"] : string.Empty,
                    Name = d.ContainsKey("name") ? d["name"] : string.Empty
                }).ToList();
            }
            else if (sheet.Name.ToLower().Contains("timesheet"))
            {
                excelData.TimeSheets = table;
                timesheets = table.Select(t => new TimeSheet
                {
                    Id = t.ContainsKey("id") ? t["id"] : string.Empty,
                    EmployeeId = t.ContainsKey("EmployeeId") ? t["EmployeeId"] : "0",
                    Hours = t.ContainsKey("Hours") ? double.TryParse(t["Hours"], out var hours) ? hours : 0 : 0
                }).ToList();
            }
        }

        if (excelData.Employees.Count == 0)
            return BadRequest("No employee data found in the file.");

        CompanyData.ExcelData = excelData;
        return Ok(new
        {
            CompanyData.ExcelData,
            Employees = employees,
            Projects = projects,
            Department = departments,
            timesheet = timesheets,
            NoOfEmployees = employees.Count(),
            NoOfBillableEmployees = employees.Where(x => x.IsBillable).Count(),
            NoOfNonBillableEmployees = employees.Where(x => !x.IsBillable).Count(),
            NoOfProjects = projects.Count(),
            NoOfFixedCostProjects = projects.Count(x => x.IsFixedCost),
            NoOfDedicateProjects = projects.Count(x => !x.IsFixedCost),
            TotalHours = timesheets.Sum(x => x.Hours),
            BillableHours = timesheets.Where(t => employees.Where(e => e.IsBillable).Select(e => e.Id).ToList().Contains(t.EmployeeId)).Sum(x => x.Hours),
            NonBillableHours = timesheets.Where(t => employees.Where(e => !e.IsBillable).Select(e => e.Id).ToList().Contains(t.EmployeeId)).Sum(x => x.Hours),
            NoOfEmployeeJoinedInLastMonth = employees.Where(x => x.DateOfJoining != DateTime.MinValue && x.DateOfJoining >= DateTime.Now.AddMonths(-1)).Count(),
        });
    }
    [HttpGet("recommendations")]
    public async Task<IActionResult> Recommendations([FromBody] RecommendationRequest req) 
    {
        if(CompanyData.ExcelData == null)
            return BadRequest("Upload data using excel first.");
        return Ok(await _geminiService.GetRecommendationsAsync(CompanyData.ExcelData,req?.query));
    }
}
