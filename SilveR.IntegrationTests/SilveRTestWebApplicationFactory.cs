using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SilveR.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SilveR.IntegrationTests
{
    public class SilveRTestWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {
        private readonly string databaseDirectory;
        private readonly string connectionString;

        public Dictionary<int, string> SheetNames { get; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<SilveRContext>>();
                services.RemoveAll<SilveRContext>();
                services.AddDbContext<SilveRContext>(options => options.UseSqlite(connectionString));
            });
        }

        public SilveRTestWebApplicationFactory()
        {
            databaseDirectory = Path.Combine(Path.GetTempPath(), "SilveR.IntegrationTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(databaseDirectory);

            string sourceDatabasePath = Path.Combine(AppContext.BaseDirectory, "SilveR.db");
            string isolatedDatabasePath = Path.Combine(databaseDirectory, "SilveR.db");
            File.Copy(sourceDatabasePath, isolatedDatabasePath);

            connectionString = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder
            {
                DataSource = isolatedDatabasePath,
                Pooling = false
            }.ToString();

            DbContextOptionsBuilder<SilveRContext> optionsBuilder = new DbContextOptionsBuilder<SilveRContext>();
            optionsBuilder.UseSqlite(connectionString);
            using SilveRContext silverContext = new SilveRContext(optionsBuilder.Options);
            silverContext.Database.Migrate();

            UserOption userOptions = silverContext.UserOptions.Single();
            if (userOptions.GraphicsHeightJitter != 0 || userOptions.GraphicsWidthJitter != 0)
            {
                userOptions.GraphicsHeightJitter = 0;
                userOptions.GraphicsWidthJitter = 0;

                silverContext.SaveChanges();
            }

            SheetNames = silverContext.Datasets.Select(x => new KeyValuePair<int, string>(x.DatasetID, x.DatasetName)).ToDictionary(x => x.Key, x => x.Value);

            if (SheetNames.Count == 0)
            {
                throw new InvalidOperationException("No datasets found in SilveR database. Please add datasets before running integration tests.");
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && Directory.Exists(databaseDirectory))
            {
                Directory.Delete(databaseDirectory, true);
            }
        }
    }
}
