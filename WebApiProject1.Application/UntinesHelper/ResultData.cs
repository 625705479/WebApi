namespace WebApiProject1.Application.UntinesHelper
{
    public class ResultData<T>
    {
        public T Data { get; set; }
        public string ChineseError { get; set; }
        public string EnglishError { get; set; }
        public int StatusCode { get; set; } = 200;
        /// <summary>
        /// 时间戳
        /// </summary>
        public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public string Message { get; set; }
        public int Pagenumber { get; set; }=1;
        public int PageSize { get; set; } = 20;
        public int TotalCount { get; set; }
    }
}