namespace WebApiProject1.Application.Test.Dtos
{
    public class GradingQueryDetail :PageData
    {
        /// <summary>
        /// 默认初始化为一个新的 GradingDetail 实例。
        /// </summary>
        /// <value>
        /// 一个 GradingDetail 类型的对象
        /// </value>
        public grading_detail GradingDetail { get; set; } = new grading_detail();

    }
}
