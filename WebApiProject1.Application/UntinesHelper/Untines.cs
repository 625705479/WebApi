using System.Text;

namespace WebApiProject1.Application.UntinesHelper
{
    public static class Untines
    {
        private static readonly char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        private static readonly Random random = new Random();
        public static Type GetNonNullableType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return type.GetGenericArguments()[0];
            }

            // 如果类型不是 Nullable<>，则直接返回原类型  
            return type;
        }
        internal static (string, byte[]) Create()
        {
            string code = GenerateCode(4);
            byte[] imageBytes = GenerateImage(code);
            return (code, imageBytes);
        }
        /// <summary>
        /// 获取随机验证码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string GenerateCode(int length)
        {
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[random.Next(chars.Length)]);
            }
            return sb.ToString();
        }

        private static byte[] GenerateImage(string code)
        {
            return new byte[] { };
        }
        /// <summary>
        /// 封装错误信息设置方法
        /// </summary>
        /// <param name="result"></param>
        /// <param name="myError"></param>
        /// <param name="Message"></param>
        public static void SetError(ResultData<object> result, EnumExtensions.MyErrorEnum myError, string Message = null)
        {
            BaseResponse baseResponse = new BaseResponse();

            baseResponse.StatusCode = 500;
            baseResponse.ChineseError = myError.GetChinese();
            baseResponse.EnglishError = myError.GetEnglish();
            baseResponse.Message = Message;
            result.BaseResponse = baseResponse;
            Logger.Error("接口异常:" + baseResponse.StatusCode + baseResponse.Message + baseResponse.ChineseError + baseResponse.EnglishError);

        }
    }
}
