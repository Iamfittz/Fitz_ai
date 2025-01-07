using Tweetinvi;
using Microsoft.Extensions.Logging;
using OpenAI_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitz_ai
{
    public class TwitterAIBot
    {
        private readonly TwitterClient _twitterClient;
        private readonly OpenAIAPI _openAIAPI;
        private readonly ILogger<TwitterAIBot> _logger;
        private readonly BotConfiguration _botConfiguration;
        public TwitterAIBot(BotConfiguration botConfiguration, ILogger<TwitterAIBot> logger)
        {
            _botConfiguration = botConfiguration;
            _logger = logger;

            //Create a TwitterClient instance using data from botConfiguration
            _twitterClient = new TwitterClient(botConfiguration.Twitter.ConsumerKey,
                                               botConfiguration.Twitter.ConsumerSecret,
                                               botConfiguration.Twitter.AccessToken,
                                               botConfiguration.Twitter.AccessTokenSecret);

            //Create an instance of OpenAIAPI using the API key from botConfiguration.OpenAI

            _openAIAPI = new OpenAIAPI(botConfiguration.OpenAI.ApiKey);
        }
    }
}
