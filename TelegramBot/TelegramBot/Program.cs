using System.Data;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace System
{
    class Program
    {
        // Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
        private static ITelegramBotClient _botClient;
        
        // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
        private static ReceiverOptions _receiverOptions;
        
        static async Task Main()
        {
            //Присваиваю переменной значение,в параметрах передаю токен
            _botClient = new TelegramBotClient("7716138583:AAFRoCnMNO_TWf-1f2hNukZyqg8s3g4p9mo");
            _receiverOptions = new ReceiverOptions //
            {
               AllowedUpdates = new [] //Здесь буду указывать тип обновлений для нашего бота
               {
                   UpdateType.Message //Сообщения (фото,видео,голосовые,текстовые)
               },
               
               //Обрабоька входящих сообщений когда бот был оффлайн
               //true - не обработывать,false - обрабатывать 
               ThrowPendingUpdates = true,
            };

            using var cts = new CancellationTokenSource();
            
            
            _botClient.StartReceiving(UpdateHandler,ErrorHandler,_receiverOptions,cts.Token);//Запуск бота
            var me = await _botClient.GetMeAsync();
            Console.WriteLine($"{me.Username},запущен!");
            
            await Task.Delay(-1);
            
        }

        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //Блок try чтобы под не падал в случае ошибки
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                    {
                        var message = update.Message;

                        var user = message.From;
                        
                        Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                        var chat = message.Chat;

                        await _botClient.SendTextMessageAsync(
                            chat.Id
                            ,message.Text);
                        
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            } 
        }
        
        private static  Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            //Переменная в котрой будет хранится код ошибки
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}


