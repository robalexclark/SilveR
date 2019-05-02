using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using SilveR;
using SilveR.Models;
using SilveR.Services;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ControlledForms.IntegrationTests
{
    public class SilveRTestWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            if (File.Exists("SilveR.db"))
                File.Delete("SilveR.db");

            builder.ConfigureServices(services =>
            {
                services.AddDbContext<SilveRContext>(options => options.UseSqlite("Data Source=SilveR.db"));
                services.AddScoped<ISilveRRepository, SilveRRepository>();

                //R processing services comprising of R processor and queue services
                services.AddSingleton<IRProcessorService, RProcessorService>();
                services.AddHostedService<QueuedHostedService>();
                services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
                //services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

                services.AddMvc();
            });
        }     
    }

    public static class ObjectExtensions
    {
        public static IDictionary<string, string> ToKeyValue(this object metaToken)
        {
            if (metaToken == null)
            {
                return null;
            }

            JToken token = metaToken as JToken;
            if (token == null)
            {
                return ToKeyValue(JObject.FromObject(metaToken));
            }

            if (token.HasValues)
            {
                var contentData = new Dictionary<string, string>();
                foreach (var child in token.Children().ToList())
                {
                    var childContent = child.ToKeyValue();
                    if (childContent != null)
                    {
                        contentData = contentData.Concat(childContent)
                            .ToDictionary(k => k.Key, v => v.Value);
                    }
                }

                return contentData;
            }

            var jValue = token as JValue;
            if (jValue?.Value == null)
            {
                return null;
            }

            var value = jValue?.Type == JTokenType.Date ?
                jValue?.ToString("o", CultureInfo.InvariantCulture) :
                jValue?.ToString(CultureInfo.InvariantCulture);

            return new Dictionary<string, string> { { token.Path, value } };
        }
    }


}