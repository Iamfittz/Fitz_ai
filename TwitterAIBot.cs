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
            try
            {
                // Get parameters from configuration
                var sinceId = _botConfiguration.Twitter.SinceId;
                var count = _botConfiguration.Twitter.MentionCount;

                // Receiving new mentions
                var mentions = await _twitterClient.Timelines.GetMentionsTimelineAsync(new GetMentionsTimelineParameters
                {
                    SinceId = sinceId, // Start reading from our "bookmark"
                    PageSize = count, // Take the specified number of tweets
                    TrimUser = true, // Do not load unnecessary information about users
                    IncludeEntities = false // Do not load additional tweet entities
                });

                if(mentions !=null && mentions.Any()) // Check if there are new mentions
                {
                    foreach(var mention in mentions) // For each mention
                    {
                        //Generate response using AI
                        string aiResponse = await GenerateAIResponseAsync(mention.Text);

                        if(!string.IsNullOrEmpty(aiResponse)) // If the AI ​​successfully generated a response
                        {
                            // Publish the response as a reply to the mention
                            await PublishTweetAsync(aiResponse, mention.Id);
                        }
                        // Update SinceId to avoid processing the same tweets
                        _botConfiguration.Twitter.SinceId = mention.Id;
                    }

                    _logger.LogInformation($"Processed {mentions.Count()} mentions");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing mentions");
            }
         
        }

        public async Task RunAsync(int checkInterval = 60)
        {
            try
            {
                _logger.LogInformation("The bot has been launched and is starting to work");
                // Publish the starting message
                await PublishTweetAsync("Hello! I'm an AI bot and I'm online! 🤖");

                while (true)
                {
                    await ProcessMentionsAsync();
                    // Wait the specified number of seconds before the next check
                    await Task.Delay(checkInterval + 1000);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error in the bot");
                throw;
            }
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
