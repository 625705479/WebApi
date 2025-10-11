using Furion;
using Furion.DataValidation;
using Furion.FriendlyException;
using Furion.UnifyResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using WebApiProject1.Application.UntinesHelper;

namespace WebApiProject1.Web.Core
{
    [UnifyModel(typeof(ResultData<>))]
    public class YourRESTfulResultProvider : IUnifyResultProvider
    {
        public IActionResult OnException(ExceptionContext context, ExceptionMetadata metadata)
        {
            return new JsonResult(YourRESTfulResult(metadata.StatusCode, ChineseError: metadata.Errors, EnglishError: metadata.Errors)
               ); // 当前行仅限 Furion 4.6.6+ 使用
        }

        public async Task OnResponseStatusCodes(HttpContext context, int statusCode, UnifyResultSettingsOptions unifyResultSettings = null)
        {
            // 设置响应状态码
            UnifyContext.SetResponseStatusCodes(context, statusCode, unifyResultSettings);
            switch (statusCode)
            {
                // 处理 401 状态码
                case StatusCodes.Status401Unauthorized:
                    await context.Response.WriteAsJsonAsync(YourRESTfulResult(statusCode, EnglishError: "401 Unauthorized", ChineseError: "401错误")
                        , App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                    break;
                // 处理 403 状态码
                case StatusCodes.Status403Forbidden:
                    await context.Response.WriteAsJsonAsync(YourRESTfulResult(statusCode, EnglishError: "403 Forbidden", ChineseError: "403错误")
                        , App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                    break;
                case StatusCodes.Status500InternalServerError:
                    await context.Response.WriteAsJsonAsync(YourRESTfulResult(statusCode, EnglishError: "500 Forbidden", ChineseError: "500错误")
                        , App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                    break;
                default: break;
            }

        }

        public IActionResult OnSucceeded(ActionExecutedContext context, object data)
        {
            return new JsonResult(YourRESTfulResult(StatusCodes.Status200OK, true, data)
                ); // 当前行仅限 Furion 4.6.6+ 使用

        }

        public IActionResult OnValidateFailed(ActionExecutingContext context, ValidationMetadata metadata)
        {
            return new JsonResult(YourRESTfulResult(metadata.StatusCode ?? StatusCodes.Status400BadRequest, ChineseError: metadata.Message, EnglishError: metadata.Message) // 如果需要只显示第一条错误，修改为：errors: metadata.FirstErrorMessage
                ); // 当前行仅限 Furion 4.6.6+ 使用

        }
        /// <summary>
        /// 返回 RESTful 风格结果集
        /// </summary>
        /// <param name="statusCode"></param>
        /// // <param name="data"></param>
        /// /// <param name="ChineseError"></param>
        /// /// <param name="EnglishError"></param>
        /// /// <param name="Message"></param>
        /// /// <param name="pagenumber"></param>
        /// /// <param name="pageSize"></param>
        /// /// <param name="totalCount"></param>
        /// /// <returns></returns>
        /// <returns></returns>
        private static ResultData<object> YourRESTfulResult(int statusCode, object data = default, object ChineseError = default, object EnglishError = default,object Message=default,int pagenumber=default,int pageSize=default,int totalCount = default)
        {
            return new ResultData<object>
            {
                StatusCode = statusCode,
                Data = data,
                ChineseError = (string)ChineseError,
                EnglishError = (string)EnglishError,
                Message = (string)Message,
                Pagenumber = pagenumber,
                PageSize= pageSize,
                TotalCount= totalCount,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
        }
    }
}
