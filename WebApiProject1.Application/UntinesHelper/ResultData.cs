namespace WebApiProject1.Application.UntinesHelper
{
    public class ResultData<T>
    {
        public T Data { get; set; }
        /// <summary>
        /// 接口通用响应状态基类
        /// </summary>
        public BaseResponse BaseResponse { get; set; }
        /// <summary>
        /// 分页信息（非分页场景可置为null）
        /// </summary>
        public PageInfo PageInfo { get; set; }
    }
    /// <summary>
    /// 接口通用响应状态基类（存储通用的状态/错误信息）
    /// </summary>
    public class BaseResponse
    {
        /// <summary>
        /// 中文错误信息（默认：请求成功）
        /// </summary>
        public string ChineseError { get; set; } = "请求成功";

        /// <summary>
        /// 英文错误信息（默认：Request successful）
        /// </summary>
        public string EnglishError { get; set; } = "Request successful";

        /// <summary>
        /// 响应状态码（默认：200 成功）
        /// </summary>
        public int StatusCode { get; set; } = 200;

        /// <summary>
        /// 时间戳（毫秒，默认：当前UTC时间戳）
        /// </summary>
        public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        /// <summary>
        /// 附加消息（可选）
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 分页信息类（专门存储分页相关参数）
    /// </summary>
    public class PageInfo
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }


    }
}