using Tweetinvi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Fitz_ai
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Create configuration from JSON file
                var configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                // Получаем настройки бота
                var botConfig = configuration.Get<BotConfiguration>();
                using (var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder
                        .AddConsole()
                        .SetMinimumLevel(LogLevel.Information);
                }))
                {
                    var logger = loggerFactory.CreateLogger<TwitterAIBot>();
                    var bot = new TwitterAIBot(botConfig, logger);

                    Console.WriteLine("Бот запускается...");
                    await bot.RunAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey();
            }
        }
    }
}
