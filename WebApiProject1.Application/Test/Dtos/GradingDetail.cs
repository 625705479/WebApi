namespace WebApiProject1.Application.Test
{
    public class grading_detail
    {

        /// <summary>
        /// id
        /// </summary>
        public long id { get; set; }


        /// <summary>
        /// line_no线别
        /// </summary>
        public string line_no { get; set; }


        /// <summary>
        /// grading_position档位
        /// </summary>
        public string grading_position { get; set; }


        /// <summary>
        /// max_num可存放最大数量
        /// </summary>
        public long max_num { get; set; }


        /// <summary>
        /// place_type放置方式 1平放 2竖放
        /// </summary>
        public long place_type { get; set; }


        /// <summary>
        /// power功率
        /// </summary>
        public object power { get; set; }


        /// <summary>
        /// electric电流
        /// </summary>
        public object electric { get; set; }


        /// <summary>
        /// grade等级
        /// </summary>
        public object grade { get; set; }


        /// <summary>
        /// color色系
        /// </summary>
        public object color { get; set; }


        /// <summary>
        /// item料号
        /// </summary>
        public string item { get; set; }


        /// <summary>
        /// order工单
        /// </summary>
        public object order { get; set; }

        /// <summary>
        /// region区域A区 B区
        /// </summary>
        public object region { get; set; }


        /// <summary>
        /// grading_type档位类型 0-NG档 1-非NG档
        /// </summary>
        public long grading_type { get; set; }


        /// <summary>
        /// is_use当前档位使用状态
        /// </summary>
        public string is_use { get; set; }

        /// <summary>
        /// update_time修改时间
        /// </summary>
        public object update_time { get; set; }
    }
}
