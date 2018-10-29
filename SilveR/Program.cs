using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace SilveR
{
    public class Program
    {
        public static string AppName { get; private set; } = "ƩilveR";

        public static void Main(string[] args)
        {
            if (args.Length > 0)
                AppName = args[0];

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}