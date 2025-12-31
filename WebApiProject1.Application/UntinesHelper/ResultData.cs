namespace WebApiProject1.Application.UntinesHelper
{
    public class ResultData<T>
    {
        public T Data { get; set; }
        public string ChineseError { get; set; }="请求成功";
        public string EnglishError { get; set; }= "Request successful";
        public int StatusCode { get; set; } = 200;
        public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public string Message { get; set; }
        public int Pagenumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}