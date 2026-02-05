using System.Reflection;

namespace WebApiProject1.Application.UntinesHelper
{
    public static class EnumExtensions
    {
        // 获取中文错误信息
        public static string GetChinese(this MyErrorEnum enumValue)
        {
            FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            var attribute = fieldInfo.GetCustomAttributes(typeof(EnumErrorInfoAttribute), false)
                                   .FirstOrDefault() as EnumErrorInfoAttribute;
            return attribute?.Chinese;
        }

        // 获取英文错误信息
        public static string GetEnglish(this MyErrorEnum enumValue)
        {
            FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            var attribute = fieldInfo.GetCustomAttributes(typeof(EnumErrorInfoAttribute), false)
                                   .FirstOrDefault() as EnumErrorInfoAttribute;
            return attribute?.English;
        }
        #region 报错信息枚举
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
            [EnumErrorInfo("系统异常", "System Failed ")]
            SystemFailed,
            [EnumErrorInfo("接口异常500", "  Interface Exception ")]
            InterfaceException,

        }
        #endregion

        // 扩展特性以支持中英文
        public class EnumErrorInfoAttribute : Attribute
        {
            public string Chinese { get; }
            public string English { get; }

            public EnumErrorInfoAttribute(string chinese, string english)
            {
                Chinese = chinese;
                English = english;
            }
        }

    }
}
