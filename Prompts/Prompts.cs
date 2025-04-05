using HackStack___Gemini.Models;
using Newtonsoft.Json;

namespace HackStack___Gemini.Prompts
{
    public static class PromptsService
    {
        private static readonly string endOutput = $@"

            DO NOT reference technical or database-specific fields (e.g., no mention of column names like departmentId or projectId).
            Only use visible, human-friendly data like names, departments, locations, job status, or project titles.
            Use all relevant details to form conclusions—like who is assigned to what, where projects are located, how long they last, and whether they're fixed-cost.

                Then, convert the output strictly into valid, styled HTML:
            - Wrap everything in a single <div>.
            - Use semantic tags where natural:
              - <h2>, <h3> for important sections (optional, not required).
              - <p> for text blocks like summaries, issues, recommendations, actions, or notes.
            - Use <strong> for important actions.
            - Use <em> to emphasize recommendations.
            - Use <u> for any critical notes.
            - Apply inline styles for spacing, font-size, and weight, but DO NOT include font-family.
            - Use <br> or styled <div>s for spacing when needed.
            - Do NOT use <ul>, <li>, or <table>.
            - Do NOT add any explanations, commentary, or extra text.
            - You ARE allowed to reference names, departments, or any other information present in the provided dataset.
            - Return only the final HTML block, ready to render.";

        private static readonly string dashboardDefaultPrompt = $@"
                    Your job is to:
                Analyze departmental performance based on workforce roles, billability, and overall output versus cost.
                Identify any underperforming departments using human-readable information only (e.g., names, roles, projects).
                Recommend staffing actions such as hiring, downsizing, or training—based on productivity, tenure, and utilization.
                Detect employees who appear underutilized or overburdened based on their work assignments, availability, or timeline data.
                Provide high-level, actionable recommendations and strategic insights for the CEO.
                ";


        private static readonly string queryJobPrompt = $@"
                     Analyze departmental performance based on workforce roles, billability, and overall output versus cost.
                Identify any underperforming departments using human-readable information only (e.g., names, roles, projects).
                Recommend staffing actions such as hiring, downsizing, or training—based on productivity, tenure, and utilization.
                Detect employees who appear underutilized or overburdened based on their work assignments, availability, or timeline data.
                Provide high-level, actionable recommendations and strategic insights for the CEO.
                ";

        private static readonly string dashboardOutputPrompt = $@"
                    - Use all available data (e.g., names, departments, roles) in the query or dataset if relevant.
                    - Focus on insightful, actionable, and relevant information. Use reasoning to provide context and depth.
                    ";
        public static string GetPrompt(ExcelData data, string promptName, string query)
        {
            string defaultPromptData = $@"
                    You're a business analyst giving insights to the CEO based on this company Excel data.

                    Use this data:
                    Departments: {JsonConvert.SerializeObject(data.Departments, Formatting.None)}
                    Employees: {JsonConvert.SerializeObject(data.Employees, Formatting.None)}
                    Projects: {JsonConvert.SerializeObject(data.Projects, Formatting.None)}
            ";

            switch (promptName)
            {
                case "Dashboard":
                    return defaultPromptData + dashboardDefaultPrompt + dashboardOutputPrompt + endOutput;
                case "query":
                    return  $@" {defaultPromptData} Your job is to: {query} {queryJobPrompt}
                        this query {query} has the highest priority make sure you follow the it and give the output in
                        given content and in given length
                        Answer this query: {query} {endOutput}
                    ";
                default:
                    return "";
            }
        }
    }
}
