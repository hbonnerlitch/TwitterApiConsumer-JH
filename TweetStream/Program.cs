using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using TwitterStreamReader.PulseChecks;

namespace TwitterStreamReader
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            // create service collection
            var services = new ServiceCollection();
            ConfigureServices(services);

            // create service provider
            var serviceProvider = services.BuildServiceProvider();

            try
            {
                // entry to run app
                await serviceProvider.GetService<TweetStreamProcessor>().Run(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // configure logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            // build config
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            services.Configure<AppSettings>(configuration.GetSection("TweetStreamProcessor"));

            // add services:
            services.AddScoped(typeof(IStatisticsCalculator), typeof(StatisticsCalculator));

            // add app
            services.AddTransient<TweetStreamProcessor>();
        }
    }
}