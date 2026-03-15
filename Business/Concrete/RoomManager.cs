using Business.Abstract;
using Business.Exceptions;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using Entities.Enums;
using StackExchange.Redis;
using System.Text.Json;

namespace Business.Concrete
{
    public class RoomManager : IRoomService
    {
        private readonly IRoomDal roomDal;
        private readonly IRoomMemberDal roomMemberDal;
        private readonly IConnectionMultiplexer redis;

        public RoomManager(IRoomDal roomDal, IRoomMemberDal roomMemberDal, IConnectionMultiplexer redis)
        {
            this.roomDal = roomDal;
            this.roomMemberDal = roomMemberDal;
            this.redis = redis;
        }

        public async Task AddRoomAsync(RoomDto roomDto)
        {
            if (string.IsNullOrWhiteSpace(roomDto.RoomName))
            {
                throw new ArgumentException("Room name cannot be empty.");
            }

            var room = new Room
            {
                RoomName = roomDto.RoomName,
                AdminOnly = roomDto.AdminOnly
            };

            await roomDal.AddAsync(room);
        }

        public async Task<Room?> GetRoomByIdAsync(Guid roomId)
        {
            return await roomDal.GetAsync(r => r.Id == roomId);
        }

        public async Task<IReadOnlyCollection<Room>> GetRoomsAsync()
        {
            return await roomDal.GetAllAsync();
        }

        public async Task JoinRoomAsync(Guid roomId, Guid userId)
        {
            var room = await roomDal.GetAsync(x => x.Id == roomId);
            if (room == null)
            {
                throw new KeyNotFoundException("Room not found.");
            }

            var existing = await roomMemberDal.GetAsync(x => x.RoomId == roomId && x.UserId == userId);
            if (existing != null)
            {
                throw new ConflictException("User is already a member of this room.");
            }

            var roomMember = new RoomMember
            {
                RoomId = roomId,
                UserId = userId,
                Role = RoomRole.Member,
                JoinedAt = DateTime.UtcNow
            };

            await roomMemberDal.AddAsync(roomMember);
            await PublishMembershipEventAsync(roomId, userId, "room_member_joined");
        }

        public async Task LeaveRoomAsync(Guid roomId, Guid userId)
        {
            var membership = await roomMemberDal.GetAsync(x => x.RoomId == roomId && x.UserId == userId);
            if (membership == null)
            {
                throw new KeyNotFoundException("Room membership not found.");
            }

            await roomMemberDal.DeleteAsync(membership);
            await PublishMembershipEventAsync(roomId, userId, "room_member_left");
        }

        private async Task PublishMembershipEventAsync(Guid roomId, Guid userId, string eventType)
        {
            var payload = JsonSerializer.Serialize(new
            {
                type = eventType,
                roomId,
                userId,
                occurredAt = DateTime.UtcNow
            });

            var subscriber = redis.GetSubscriber();
            var channel = $"chatroom:{roomId}";
            await subscriber.PublishAsync(RedisChannel.Literal(channel), payload);
        }
    }
}
