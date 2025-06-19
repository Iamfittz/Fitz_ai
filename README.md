# 🤖 Fitz AI Twitter Bot

**Fitz AI** is a smart Twitter bot that leverages **OpenAI GPT** to generate short, friendly replies to tweets that mention its handle. It monitors incoming mentions, crafts contextual responses, and posts them automatically.

---

## 🚀 Features

- 📡 Real‑time tracking of Twitter mentions
- 🤖 AI‑powered reply generation via **OpenAI API** (GPT‑3/4)
- ✍️ Automatic posting of reply tweets
- ⏱ Scheduled checks that respect Twitter API rate limits
- 📊 Monthly tweet counter with auto‑reset
- 🛡 Robust error handling and logging

---

## 🔧 Tech Stack

- [.NET 6+](https://dotnet.microsoft.com/)
- [Tweetinvi API](https://github.com/linvi/Tweetinvi) — Twitter SDK
- [OpenAI API](https://platform.openai.com/docs) — AI responses
- `ILogger` — structured logging
- `appsettings.json` — configuration

---

## 📁 Project Structure

```
├── BotConfiguration.cs        # Twitter + OpenAI settings
├── TwitterAIBot.cs            # Core bot logic
├── Program.cs                 # Entry point
├── appsettings.template.json  # Config template
```

---

## ⚙️ Quick Start

1. Clone the repo:
   ```bash
   git clone https://github.com/your-username/FitzAI-TwitterBot.git
   cd FitzAI-TwitterBot
   ```

2. Create `appsettings.json` based on the template:
   ```json
   {
     "Twitter": {
       "ConsumerKey": "your_consumer_key",
       "ConsumerSecret": "your_consumer_secret",
       "AccessToken": "your_access_token",
       "AccessTokenSecret": "your_access_token_secret",
       "BearerToken": "your_bearer_token"
     },
     "OpenAI": {
       "ApiKey": "your_openai_api_key"
     }
   }
   ```

3. Restore dependencies and run:
   ```bash
   dotnet restore
   dotnet run
   ```

---
