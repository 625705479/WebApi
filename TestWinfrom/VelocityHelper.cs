using NVelocity;
using NVelocity.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWinfrom
{
    public static class VelocityHelper
    {
        /// <summary>
        /// 渲染vm模板
        /// </summary>
        /// <param name="templateFilePath">模板完整路径</param>
        /// <param name="model">模板变量</param>
        /// <returns>渲染完成文本</returns>
        public static string Render(string templateFilePath, Dictionary<string, object> model)
        {
            if (!File.Exists(templateFilePath))
            {
                throw new FileNotFoundException("找不到模板文件", templateFilePath);
            }

            var velocityEngine = new VelocityEngine();
            velocityEngine.Init();

            var ctx = new VelocityContext();
            foreach (var kv in model)
            {
                ctx.Put(kv.Key, kv.Value);
            }

            string templateText = File.ReadAllText(templateFilePath, Encoding.UTF8);
             var writer = new StringWriter();
            velocityEngine.Evaluate(ctx, writer, string.Empty, templateText);

            return writer.ToString();
        }
    }
}
