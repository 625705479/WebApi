using Furion;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace WebApiProject1.Web.Core
{
    public class Startup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddJwt<JwtHandler>();

            services.AddCorsAccessor();

            // =========在这里注册后台扫描托管服务=========
            services.AddHostedService<CacheScanService>();

            services.AddControllers()
              .AddInjectWithUnifyResult<YourRESTfulResultProvider>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 如果你想关闭https重定向，注释掉这一行即可
            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCorsAccessor();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseInject(string.Empty);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class CacheScanService : BackgroundService
    {
        //调试可以改成20秒 TimeSpan.FromSeconds(20)
        private readonly TimeSpan _scanPeriod = TimeSpan.FromSeconds(20);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //框架自动调用ExecuteAsync，不要手动调用
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cacheDir = Path.Combine(AppContext.BaseDirectory, "Cache");
                    if (Directory.Exists(cacheDir))
                    {
                        var files = Directory.GetFiles(cacheDir);
                        foreach (var filePath in files)
                        {
                            try
                            {
                                string content = await File.ReadAllTextAsync(filePath, stoppingToken);
                                int splitIndex = content.LastIndexOf('|');
                                if (splitIndex <= 0)
                                {
                                    File.Delete(filePath);
                                    continue;
                                }
                                string expireText = content.Substring(splitIndex + 1);
                                if (DateTime.TryParse(expireText, out DateTime expireTime))
                                {
                                    if (DateTime.Now > expireTime)
                                    {
                                        File.Delete(filePath);
                                    }
                                }
                                else
                                {
                                    File.Delete(filePath);
                                }
                            }
                            catch (IOException)
                            {
                                // 文件占用跳过
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                await Task.Delay(_scanPeriod, stoppingToken);
            }
        }
    }
    }