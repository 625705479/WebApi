using System.Reflection;

namespace WebApiProject1.Application.UntinesHelper
{
    /// <summary>
    /// 错误枚举统一管理类（包含特性、枚举、扩展方法）
    /// </summary>
    public static class EnumExtensions
    {
        #region 错误信息特性
        /// <summary>
        /// 枚举绑定中英文错误描述特性
        /// </summary>
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class EnumErrorInfoAttribute : Attribute
        {
            public string Chinese { get; }
            public string English { get; }

            public EnumErrorInfoAttribute(string chinese, string english)
            {
                Chinese = chinese?.Trim() ?? string.Empty;
                English = english?.Trim() ?? string.Empty;
            }
        }
        #endregion

        #region 全局错误枚举
        /// <summary>
        /// 统一业务错误类型枚举
        /// </summary>
        public enum MyErrorEnum
        {
            [EnumErrorInfo("验证码错误", "Verification Code Error")]
            VerificationCodeError,

            [EnumErrorInfo("查询错误", "Query Error")]
            QueryError,

            [EnumErrorInfo("挡位错误", "Gear Position Error")]
            GearPositionError,

            [EnumErrorInfo("插入数据失败", "Data insertion failed")]
            DataInsertionFailed,

            [EnumErrorInfo("删除数据失败", "Failed to delete data")]
            FailedToDeleteData,

            [EnumErrorInfo("系统异常", "System Exception")]
            SystemFailed,

            [EnumErrorInfo("接口异常500", "Interface Exception 500")]
            InterfaceException,

            [EnumErrorInfo("申请人ID不能为空", "Applicant ID cannot be empty")]
            ApplicantIdEmpty,

            [EnumErrorInfo("请假天数必须大于0", "The number of leave days must be greater than 0")]
            LeaveDaysMustGreaterZero,

            [EnumErrorInfo("流程ID：{0}已经处于被取消，请勿重新申请", "Process ID: {0} has been cancelled, please do not reapply")]
            InstanceAlreadyCancelled
        }
        #endregion

        #region 枚举扩展方法
        /// <summary>
        /// 获取纯中文提示
        /// </summary>
        public static string GetChinese(this MyErrorEnum enumValue)
        {
            var attr = GetAttr(enumValue);
            return attr?.Chinese ?? string.Empty;
        }

        /// <summary>
        /// 获取格式化中文提示，支持{0}占位符
        /// </summary>
        public static string GetChinese(this MyErrorEnum enumValue, params object[] args)
        {
            string txt = GetChinese(enumValue);
            return args == null || args.Length == 0 ? txt : string.Format(txt, args);
        }

        /// <summary>
        /// 获取纯英文提示（无格式化）
        /// </summary>
        public static string GetEnglish(this MyErrorEnum enumValue)
        {
            var attr = GetAttr(enumValue);
            return attr?.English ?? string.Empty;
        }

        /// <summary>
        /// 获取格式化英文提示，支持{0}占位符
        /// </summary>
        public static string GetEnglish(this MyErrorEnum enumValue, params object[] args)
        {
            string txt = GetEnglish(enumValue);
            return args == null || args.Length == 0 ? txt : string.Format(txt, args);
        }

        /// <summary>
        /// 内部反射获取特性
        /// </summary>
        private static EnumErrorInfoAttribute GetAttr(MyErrorEnum enumValue)
        {
            try
            {
                FieldInfo field = enumValue.GetType().GetField(enumValue.ToString());
                return field.GetCustomAttributes(typeof(EnumErrorInfoAttribute), false)
                            .FirstOrDefault() as EnumErrorInfoAttribute;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}