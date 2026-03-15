using Business.Abstract;
using Business.Exceptions;
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
        // Required Services
        private readonly IRoomService roomService;
        private readonly IRoomMemberService roomMemberService;
        private readonly IMessageDal messageDal;

        // Redis
        private readonly IConnectionMultiplexer redis;

        public ChatManager(
            IConnectionMultiplexer _redis, 
            IMessageDal _messageDal,
            IRoomMemberService _roomMemberService,
            IRoomService _roomService)
        {
            redis = _redis;

            messageDal = _messageDal;
            roomMemberService = _roomMemberService;
            roomService = _roomService;
        }

        public async Task SendMessageAsync(Guid senderId, SendMessageDto dto)
        {
            if(!await CanSendMessage(dto.RoomId, senderId))
            {
                throw new AuthorizationFailedException("You do not have permission to send messages in this room.");
            }


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
            var payload = JsonSerializer.Serialize(new MessageCacheDto
            {
                SenderId = senderId,
                Content = dto.Content,
                SentAt = message.SentAt,
            });
            await db.ListRightPushAsync(cacheKey, payload);
            await db.ListTrimAsync(cacheKey, -50, -1); // son 50 mesajı tut


            // Redis pub/sub configuration
            var subscriber = redis.GetSubscriber();
            var channel = $"chatroom:{dto.RoomId}";
            await subscriber.PublishAsync(RedisChannel.Literal(channel), payload);
        }

        public async Task<IReadOnlyCollection<MessageCacheDto>> GetRoomHistoryAsync(Guid roomId)
        {
            var db = redis.GetDatabase();
            var cacheKey = $"chat:room:{roomId}:messages";
            var cachedMessages = await db.ListRangeAsync(cacheKey);

            if(cachedMessages.Length > 0)
            {
                var deserializedDtos = 
                    cachedMessages.Select(x => JsonSerializer.Deserialize<MessageCacheDto>(x))
                    .ToList();

                return deserializedDtos;
            }

            var existing = await messageDal.GetAllAsync(x => x.RoomId == roomId);
            var dtos = existing.Select(x => new MessageCacheDto
            {
                SenderId = x.SenderId,
                Content = x.Content,
                SentAt = x.SentAt
            }).ToList();

            foreach (var dto in dtos)
            {
                var payload = JsonSerializer.Serialize(dto);
                await db.ListRightPushAsync(cacheKey, payload);
            }
            await db.ListTrimAsync(cacheKey, -50, -1);


            return dtos;
        }

        public async Task<bool> CanSendMessage(Guid roomId, Guid senderId)
        {
            // This method can be used to check if the user has permission to send messages in the room
            // For example, you can check if the user is a member of the room or if the room is admin-only
            // This logic can be implemented based on your application's requirements

            var room = await roomService.GetRoomByIdAsync(roomId);
            var sender = await roomMemberService.GetRoomMemberAsync(roomId, senderId);


            if(sender == null || (room.AdminOnly && sender.Role == Entities.Enums.RoomRole.Member))
            {
                return false;
            }

            return true;
        }
    }
}
