using BenchmarkDotNet.Running;
using BenchmarkTest;

namespace tree_demo_back
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Contains("--benchmark"))
            {
                var summary = BenchmarkRunner.Run<Test>();
            }
            else
            {
                CreateHostBuilder(args).Build().Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();

                    logging.AddConsole();
                });
    }
}
