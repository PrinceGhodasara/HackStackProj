namespace HackStack___Gemini.Models
{
    public class GeminiResponse
    {
        public List<Candidate> candidates { get; set; }
    }

    public class Candidate
    {
        public Content content { get; set; }
    }

    public class Content
    {
        public List<Part> parts { get; set; }
    }

    public class Part
    {
        public string text { get; set; }
    }

    public class Recommendation
    {
        public string department { get; set; }
        public string issue { get; set; }
        public string suggestion { get; set; }
        public string action { get; set; }
    }

    public class RecommendationResponse
    {
        public string summary { get; set; }
        public List<Recommendation> recommendations { get; set; }
    }

    public class ApiResult
    {
        public ApiResult(bool isSuccess, string Error,string result = null)
        {
            this.isSuccess = isSuccess;
            this.result = result;
            this.Error = Error;
        }

        public bool isSuccess { get; set; }
        public string result { get; set; }
        public string Error { get; set; }
    }

    public static class ApiErrors
    {
        public static ApiResult GeminiError { get; } = new ApiResult(false, "Error from Gemini API", null);
        public static ApiResult NoResFromGemini { get; } = new ApiResult(false, "No response content from Gemini.", null);
        public static ApiResult ParseError { get; } = new ApiResult(false, "Failed to parse Gemini response.", null);
        
    }


}
