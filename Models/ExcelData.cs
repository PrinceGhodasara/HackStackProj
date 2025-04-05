namespace HackStack___Gemini.Models
{
    public static class CompanyData
    {
        public static ExcelData ExcelData { get; set; }
    }


    public class ExcelData
    {
        public List<Dictionary<string, string>> Employees { get; set; }
        public List<Dictionary<string, string>> Departments { get; set; }
        public List<Dictionary<string, string>> Projects { get; set; }
        public List<Dictionary<string, string>> TimeSheets { get; set; }

    }

    public class Employee
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string DepatmentId { get; set; }
        public double Salary { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public bool IsBillable { get; set; }
        public int ProjectId { get; set; }
    }

    public class Departments
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string state { get; set; }
        public bool IsFixedCost { get; set; }
        public int TotalHours { get; set; }
        public double Rate { get; set; }
    }

    public class TimeSheet
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public double Hours { get; set; }
    }

    public class RecommendationRequest
    {
        public string? query { get; set; }
    }
}
