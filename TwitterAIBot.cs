using Tweetinvi;
using Microsoft.Extensions.Logging;
using OpenAI_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Parameters;

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

        public async Task ProcessMentionsAsync()
        {
            // Получаем параметры из конфигурации
            var sinceId = _botConfiguration.Twitter.SinceId;
            var count = _botConfiguration.Twitter.MentionCount;

            // Получаем новые упоминания
            var mentions = await _twitterClient.Timelines.GetMentionsTimelineAsync(new GetMentionsTimelineParameters
            {
                SinceId = sinceId,
                PageSize = count,
                TrimUser = true,
                IncludeEntities = false
            });

            // ... (обработка упоминаний будет здесь)
        }

        private async Task<string> GenerateAIResponseAsync(string promt)
        {
            try
            {
                //create chat with OpenAI
                var chat = _openAIAPI.Chat.CreateConversation();

                //setting the context for the bot

                chat.AppendSystemMessage("You are a friendly Twitter bot that keeps your answers short and to the point.");
                chat.AppendUserInput(promt);

                //get an answer
                string response = await chat.GetResponseFromChatbotAsync();

                //trim your response tot CT character limit
                if (response.Length > 280)
                {
                    response = response.Substring(0, 277) + "...";
                }

                _logger.LogInformation($"Ai response generated : {response}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating AI response");
                return null;
            }
        }

        private async Task<bool> PublishTweetAsync(string text,long? replyToTweetId = null)
        {
            try
            {
                //create parameters for publishing a tweet
                var tweetParams = new Tweetinvi.Parameters.PublishTweetParameters(text);

                //if this is a reply to another tweet, set InReplyToTweetId
                if (replyToTweetId.HasValue)
                {
                    tweetParams.InReplyToTweetId = replyToTweetId.Value;
                }
                //publish a tweet
                var tweet = await _twitterClient.Tweets.PublishTweetAsync(tweetParams);

                _logger.LogInformation($"Tweet successfully published: {text}");
                return true;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting tweet");
                return false;
            }

        }
    }
}
