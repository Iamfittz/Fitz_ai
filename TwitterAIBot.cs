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

    }
}
