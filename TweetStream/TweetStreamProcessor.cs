using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

// You need to add the tweetinvi namespace
using Tweetinvi;
using Tweetinvi.Models;

// Rate calculator and counter
using TwitterStreamReader.PulseChecks;

//// stopwatch 
using System.Diagnostics;

namespace TwitterStreamReader
{
    public class TweetStreamProcessor
    {
        private const int Tweet_Reporting_Interval = 100;
        private readonly ILogger<TweetStreamProcessor> _logger;
        private readonly AppSettings _appSettings;
        private readonly IStatisticsCalculator _tweetCounter;

        private Stopwatch _TweetStreamTimer = new Stopwatch();

        public TweetStreamProcessor(IOptions<AppSettings> appSettings, ILogger<TweetStreamProcessor> logger, IStatisticsCalculator tweetCounter)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            _tweetCounter = tweetCounter ?? throw new ArgumentNullException(nameof(tweetCounter));
        }

        public async Task Run(string[] args)
        {
            _logger.LogInformation("Processing ...");

            var appCredentials = new ConsumerOnlyCredentials(_appSettings.Key, _appSettings.Secret)
            {
                BearerToken = _appSettings.BearerToken
            };

            var appClient = new TwitterClient(appCredentials);
            var sampleStreamV2 = appClient.StreamsV2.CreateSampleStream();
            sampleStreamV2.TweetReceived += (sender, args) =>
            {
                if (!_TweetStreamTimer.IsRunning)
                {
                    _TweetStreamTimer.Start();
                    return; // skip first tweet to eliminate time spent connecting to stream up front. 
                }
                _tweetCounter.totalElapsedMilliseconds = _TweetStreamTimer.ElapsedMilliseconds;

                if (_tweetCounter.incrementIterationCount(Tweet_Reporting_Interval))
                {
                    Console.Write(String.Format("Total Tweets: {0} -- ", _tweetCounter.totalIterationCount));
                    Console.WriteLine(String.Format("Tweets per minute: {0}", _tweetCounter.iterationsPerMinute()));
                }
            };

            try
            {
                await sampleStreamV2.StartAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error reading tweet stream.");
               
            }

            // If a future enhancement entails ending this task after a specified number of tweets or minutes. 
            // To end, close the console window. 
            _logger.LogInformation("Finished!");
            await Task.CompletedTask;
        }
    }
}