using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SilveR;
using SilveR.Models;
using SilveR.Services;
using System.Collections.Generic;
using System.Linq;

namespace ControlledForms.IntegrationTests
{
    public class SilveRTestWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {
        public Dictionary<int, string> SheetNames { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddDbContext<SilveRContext>(options => options.UseSqlite("Data Source=SilveR.db"));
                services.AddScoped<ISilveRRepository, SilveRRepository>();

                //R processing services comprising of R processor and queue services
                services.AddSingleton<IRProcessorService, RProcessorService>();
                services.AddHostedService<QueuedHostedService>();
                services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

                services.AddMvc();
            });
        }

        public SilveRTestWebApplicationFactory()
        {
            DbContextOptionsBuilder<SilveRContext> optionsBuilder = new DbContextOptionsBuilder<SilveRContext>();
            optionsBuilder.UseSqlite("Data Source=SilveR.db");
            SilveRContext silverContext = new SilveRContext(optionsBuilder.Options);

            SheetNames = silverContext.Datasets.Select(x => new KeyValuePair<int, string>(x.DatasetID, x.DatasetName)).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}