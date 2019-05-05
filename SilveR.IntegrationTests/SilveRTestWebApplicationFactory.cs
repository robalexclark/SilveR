using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SilveR;
using SilveR.Helpers;
using SilveR.Models;
using SilveR.Services;
using System.Collections.Generic;
using System.Data;
using System.IO;

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
            if (File.Exists("SilveR.db"))
                File.Delete("SilveR.db");

            LoadDatasets();
        }

        private void LoadDatasets()
        {
            SheetNames = new Dictionary<int, string>();

            DbContextOptionsBuilder<SilveRContext> optionsBuilder = new DbContextOptionsBuilder<SilveRContext>();
            optionsBuilder.UseSqlite("Data Source=SilveR.db");
            SilveRContext silverContext = new SilveRContext(optionsBuilder.Options);
            silverContext.Database.Migrate();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = File.Open("_test dataset.xlsx", FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    int counter = 1;
                    foreach (DataTable dataTable in result.Tables)
                    {
                        this.SheetNames.Add(counter, dataTable.TableName);

                        var dataset = dataTable.GetDataset(dataTable.TableName, 0);
                        silverContext.Datasets.Add(dataset);
                        silverContext.SaveChanges();

                        counter++;
                    }
                }
            }
        }
    }
}