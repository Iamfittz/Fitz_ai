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
            }
            catch
            {

            }
        }
    }
}
