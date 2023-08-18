# VoiceToTextTgBot

Telegram bot backend console application that can transcribe in real-time (well it depends on your machine) voice messages to text message with punctuation marks on your language. Just start app and forward your voice message to bot chat. You need bot token to make app work. You can get token from @BotFather, when you create your Telegram bot.

Bot using libraries:
Telegram.Bot,
Telegram.Bots.Extension.Pollings;
Whisper.net,
Whisper.net.Runtime,
Whisper.net.Runtime.Clblast,
Naudio,
Concentus,
Concentus.Oggfile.

Bot create temporary files (voice.ogg, voice.wav and if you want voice.txt) in BaseDirectory, but delete them after sending text as message into chat.

Bot can download ggml models if it's not in BaseDirectory and save them there. For now ggml models do not temporary files, but maybe will be in future updates.

By default bot using small model, cause I think that it's result same as large-v1 result. 

