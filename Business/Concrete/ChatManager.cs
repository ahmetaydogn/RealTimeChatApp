using Business.Abstract;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
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
            // Add message to the database
            var message = new Message
            {
                RoomId = dto.RoomId,
                SenderId = senderId,
                Content = dto.Content,
                SentAt = DateTime.UtcNow
            };
            await messageDal.AddAsync(message);

            // Pull last 50 messages from redis cache
            var db = redis.GetDatabase();
            var cacheKey = $"chat:room:{dto.RoomId}:messages";
            var payload = JsonSerializer.Serialize( new
            {
                senderId,
                content = dto.Content,
                sentAt = message.SentAt,
            });
            await db.ListRightPushAsync(cacheKey, payload);
            await db.ListTrimAsync(cacheKey, -50, -1); // son 50 mesajı tut


            // Redis pub/sub configuration
            var subscriber = redis.GetSubscriber();
            var channel = $"chatroom:{dto.RoomId}";
            await subscriber.PublishAsync(RedisChannel.Literal(channel), payload);
        }

        public async Task<IReadOnlyCollection<Message>> GetRoomHistoryAsync(Guid roomId)
        {
            return await messageDal.GetAllAsync(x => x.RoomId == roomId);
        }
    }
}
