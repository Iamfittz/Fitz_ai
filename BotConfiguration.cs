using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitz_ai
{
    //This class will correspond to the requirements of our bot,
    //which we get from the configuration file.
    public class BotConfiguration
    {
        public TwitterConfig Twitter { get; set; }
        public OpenAIConfig OpenAI { get; set; }
    }

    public class TwitterConfig
    {
        public string ConsumerKey {  get; set; }
        public string ConsumerSecret { get; set; }
        public string AccessToken {  get; set; }
        public string AccessTokenSecret { get; set; }
        public string BearerToken {  get; set; }

        public long SinceId { get; set; }
        public int MentionCount { get; set; }

    }

    public class OpenAIConfig
    {
        public string ApiKey { get; set; }
    }

    
}
