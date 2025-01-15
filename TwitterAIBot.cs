using Tweetinvi;
using Microsoft.Extensions.Logging;
using OpenAI_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Parameters;
using Tweetinvi.Models;
using Tweetinvi.Core.Models;
using Tweetinvi.Parameters.V2;
using Tweetinvi.Client.V2;

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

        //public async Task ProcessMentionsAsync()
        //{
        //    try
        //    {
        //        var sinceId = _botConfiguration.Twitter.SinceId;
        //        var searchParameters = new SearchTweetsV2Parameters("@Fitz_AI_bot -is:retweet");

        //        // Добавляем SinceId только если он больше 0
        //        if (sinceId > 0)
        //        {
        //            searchParameters.SinceId = sinceId.ToString();
        //        }

        //        searchParameters.TweetFields = new HashSet<string> { "author_id", "created_at" };

        //        var mentions = await _twitterClient.SearchV2.SearchTweetsAsync(searchParameters);

        //        // Остальной код остается тем же
        //        if (mentions?.Tweets != null && mentions.Tweets.Any())
        //        {
        //            foreach (var mention in mentions.Tweets)
        //            {
        //                // Добавляем небольшую задержку между обработкой твитов
        //                await Task.Delay(2000); // 2 секунды между твитами

        //                string aiResponse = await GenerateAIResponseAsync(mention.Text);
        //                if (!string.IsNullOrEmpty(aiResponse))
        //                {
        //                    try
        //                    {
        //                        await _twitterClient.Tweets.PublishTweetAsync(
        //                            new PublishTweetParameters(aiResponse)
        //                            {
        //                                InReplyToTweetId = long.Parse(mention.Id)
        //                            });
        //                        _logger.LogInformation($"Ответили на твит {mention.Id}: {aiResponse}");

        //                        // Добавляем задержку после публикации
        //                        await Task.Delay(2000);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        _logger.LogError(ex, $"Ошибка при ответе на твит {mention.Id}");
        //                    }
        //                }
        //                _botConfiguration.Twitter.SinceId = long.Parse(mention.Id);
        //            }
        //            _logger.LogInformation($"Обработано {mentions.Tweets.Length} упоминаний");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Ошибка при обработке упоминаний");
        //    }
        //}
        public async Task ProcessMentionsAsync()
        {
            try
            {
                _logger.LogInformation("Checking for mentions...");

                var searchParameters = new SearchTweetsV2Parameters("@Fitz_AI_bot -is:retweet")
                {
                    TweetFields = new HashSet<string> { "author_id", "created_at" }
                };

                var mentions = await _twitterClient.SearchV2.SearchTweetsAsync(searchParameters);

                if (mentions?.Tweets != null && mentions.Tweets.Any())
                {
                    foreach (var mention in mentions.Tweets)
                    {
                        // Добавляем небольшую задержку между обработкой твитов
                        await Task.Delay(2000);

                        string aiResponse = await GenerateAIResponseAsync(mention.Text);
                        if (!string.IsNullOrEmpty(aiResponse))
                        {
                            bool published = await PublishTweetAsync(aiResponse, long.Parse(mention.Id));

                            if (published)
                            {
                                _logger.LogInformation($"Successfully replied to tweet {mention.Id}");
                            }
                            else
                            {
                                _logger.LogWarning($"Failed to publish reply to tweet {mention.Id}");
                            }
                        }

                        _botConfiguration.Twitter.SinceId = long.Parse(mention.Id);
                    }

                    _logger.LogInformation($"Processed {mentions.Tweets.Length} mentions");
                }
                else
                {
                    _logger.LogInformation("No new mentions found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing mentions");
                await Task.Delay(5000); // Добавляем задержку при ошибке
            }
        }

        public async Task RunAsync(int checkInterval = 900) // 15 минут (900 секунд)
        {
            try
            {
                _logger.LogInformation("The bot has been launched and is starting to work");
                await TestConnection();

                while (true)
                {
                    try
                    {
                        _logger.LogInformation("Checking for mentions...");
                        await ProcessMentionsAsync();
                        _logger.LogInformation("Waiting 15 minutes before next check (Twitter API limit)");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during ProcessMentionsAsync");
                    }

                    // Ждём 15 минут + небольшой запас (5 секунд)
                    await Task.Delay(905 * 1000);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error in the bot");
                throw;
            }
        }
        private async Task TestConnection()
        {
            try
            {
                var user = await _twitterClient.Users.GetAuthenticatedUserAsync();
                _logger.LogInformation($"Successfully connected as: {user.Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing connection");
            }
        }
        private async Task<string> GenerateAIResponseAsync(string promt)
        {
            //try
            //{
            //    //create chat with OpenAI
            //    var chat = _openAIAPI.Chat.CreateConversation();

            //    //setting the context for the bot

            //    chat.AppendSystemMessage("You are a friendly Twitter bot that keeps your answers short and to the point.");
            //    chat.AppendUserInput(promt);

            //    //get an answer
            //    string response = await chat.GetResponseFromChatbotAsync();

            //    //trim your response tot CT character limit
            //    if (response.Length > 280)
            //    {
            //        response = response.Substring(0, 277) + "...";
            //    }

            //    _logger.LogInformation($"Ai response generated : {response}");
            //    return response;
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Error generating AI response");
            //    return null;
            //}
            await Task.Delay(500);  // Небольшая задержка для имитации "размышления"

            // Простой ответ для тестирования
            return $"Hello! Test answering {promt}";
        }

        //private async Task<bool> PublishTweetAsync(string text, long? replyToTweetId = null)
        //{
        //    try
        //    {
        //        // Проверяем лимиты перед публикацией
        //        if (!CanPublishTweet())
        //        {
        //            return false;
        //        }

        //        // Создаем параметры для публикации твита
        //        var tweetParams = new PublishTweetParameters(text)
        //        {
        //            InReplyToTweetId = replyToTweetId
        //        };

        //        // Публикуем твит
        //        var publishedTweet = await _twitterClient.Tweets.PublishTweetAsync(tweetParams);

        //        if (publishedTweet != null)
        //        {
        //            // Увеличиваем счетчик твитов
        //            _botConfiguration.Twitter.MonthlyTweetCount++;
        //            _logger.LogInformation($"Tweet successfully published: {text}. Monthly count: {_botConfiguration.Twitter.MonthlyTweetCount}");
        //            return true;
        //        }

        //        _logger.LogWarning("Failed to publish tweet.");
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error posting tweet");
        //        return false;
        //    }
        //}
        private async Task<bool> PublishTweetAsync(string text, long? replyToTweetId = null)
        {
            try
            {
                // Проверяем лимиты перед публикацией
                if (!CanPublishTweet())
                {
                    _logger.LogWarning("Tweeting limits exceeded.");
                    return false;
                }

                // Создаем параметры для публикации твита
                var tweetParams = new PublishTweetParameters(text)
                {
                    InReplyToTweetId = replyToTweetId
                };

                // Публикуем твит
                var publishedTweet = await _twitterClient.Tweets.PublishTweetAsync(tweetParams);

                if (publishedTweet != null)
                {
                    // Увеличиваем счетчик твитов
                    _botConfiguration.Twitter.MonthlyTweetCount++;
                    _logger.LogInformation($"Tweet successfully published: {text}. Monthly count: {_botConfiguration.Twitter.MonthlyTweetCount}");
                    return true;
                }

                _logger.LogWarning("Failed to publish tweet.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting tweet");
                return false;
            }
        }

        private bool CanPublishTweet()
        {
            try
            {
                // Проверяем, не начался ли новый месяц
                if (_botConfiguration.Twitter.LastResetDate.Month != DateTime.Now.Month)
                {
                    _logger.LogInformation("Сбрасываем счетчик твитов для нового месяца");
                    _botConfiguration.Twitter.MonthlyTweetCount = 0;
                    _botConfiguration.Twitter.LastResetDate = DateTime.Now;
                }

                // Проверяем, не превышен ли лимит
                bool canPublish = _botConfiguration.Twitter.MonthlyTweetCount < 500;

                if (!canPublish)
                {
                    _logger.LogWarning("Достигнут месячный лимит твитов (500)");
                }

                return canPublish;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке лимитов");
                return false;
            }
        }
    }
}
