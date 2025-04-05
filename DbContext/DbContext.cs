using HackStack___Gemini.Models;

namespace HackStack___Gemini.DbContext
{
    public class DbContext
    {
        private ExcelData Data { get; set; } = null;

        public ExcelData setData(ExcelData data)
        {
            this.Data = data;
            return this.Data;
        }

        public ExcelData getData(ExcelData data)
        {
            return this.Data;
        }
    }
}
