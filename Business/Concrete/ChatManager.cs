using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using StackExchange.Redis;
using System.Text.Json;

namespace Business.Concrete
{
    public class ChatManager : IChatService
    {
        // Variables
        private readonly IMessageDal messageDal;
        private readonly IConnectionMultiplexer redis;

        public ChatManager(IConnectionMultiplexer _redis, IMessageDal _messageDal)
        {
            redis = _redis;
            messageDal = _messageDal;
        }

        public async Task SendMessageAsync(Guid senderId, SendMessageDto dto)
        {
            var message = new Message
            {
                RoomId = dto.RoomId,
                SenderId = senderId,
                Content = dto.Content,
                SentAt = DateTime.UtcNow
            };
            await messageDal.AddAsync(message);

            var subscriber = redis.GetSubscriber();
            var channel = $"chatroom:{dto.RoomId}";
            var payload = JsonSerializer.Serialize(new
            {
                senderId,
                content = dto.Content,
                sentAt = message.SentAt
            });
            await subscriber.PublishAsync(channel, payload);
        }
    }
}
