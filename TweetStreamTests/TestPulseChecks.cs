using Xunit;
using TwitterStreamReader.PulseChecks;

namespace TweetStreamTests
{
    public class TestPulseChecks
    {
        [Fact]
        public void Test_IterationCount()
        {
            var tweetStats = new StatisticsCalculator();

            Assert.Equal(0, tweetStats.totalIterationCount);
            Assert.True(tweetStats.incrementIterationCount(1)); // increment count and report every 1 record            

            Assert.False(tweetStats.incrementIterationCount(99)); // increment count and report every 99 records
            Assert.Equal(2, tweetStats.totalIterationCount);

            tweetStats.totalElapsedMilliseconds = 59999; // set to 1 ms less than a minute
            Assert.Equal(2, tweetStats.iterationsPerMinute());

            while (!tweetStats.incrementIterationCount(60000)) { }
            Assert.Equal(60000, tweetStats.totalIterationCount);

            tweetStats.totalElapsedMilliseconds = 60000; // set to exactly 1 minute
            Assert.Equal(60000, tweetStats.iterationsPerMinute());

            Assert.True(tweetStats.incrementIterationCount(0)); // Invalid value defaults to displaying the total for every tweet. 

        }       
    }
}
