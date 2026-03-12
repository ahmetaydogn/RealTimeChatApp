using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Channels;

namespace Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService chatService;
        private readonly IRoomMemberService roomMemberService;
        private readonly IRoomService roomService;
        private readonly IConnectionMultiplexer redis;

        public ChatController(
            IConnectionMultiplexer _redis, 
            IChatService _chatService, 
            IRoomMemberService _roomMemberService,
            IRoomService _roomService)
        {
            redis = _redis;
            chatService = _chatService;
            roomMemberService = _roomMemberService;
            roomService = _roomService;
        }


        [HttpPost("send")]
        [Authorize]
        public async Task<IActionResult> SendMessage(SendMessageDto dto)
        {
            var senderId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Check user is a member of the room before sending message
            if (!await roomMemberService.IsUserInRoomAsync(dto.RoomId, senderId))
            {
                Response.StatusCode = StatusCodes.Status403Forbidden;
                return StatusCode(403, "You are not a member of this room.");
            }

            await chatService.SendMessageAsync(senderId, dto);
            return Ok();
        }


        [Authorize]
        [HttpGet("stream/{roomId}")]
        public async Task Stream(Guid roomId, CancellationToken ct)
        {
            var senderId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!await roomMemberService.IsUserInRoomAsync(roomId, senderId))
            {
                Response.StatusCode = StatusCodes.Status403Forbidden;
                await Response.WriteAsync("You are not a member of this room.", ct);
                return;
            }

            // Those are okey
            Response.Headers["Content-Type"] = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["Connection"] = "keep-alive";


            Channel<string> messageChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
            var channel = $"chatroom:{roomId}";

            var subscriber = redis.GetSubscriber();
            await subscriber.SubscribeAsync(channel, async (ch, message) =>
            {
                if (ct.IsCancellationRequested)
                    return;

                await messageChannel.Writer.WriteAsync($"data: {message}\n\n", ct);
            });

            await Response.WriteAsync("event: ping\ndata: connected\n\n", ct);
            await Response.Body.FlushAsync(ct);

            try
            {
                await foreach (var msg in messageChannel.Reader.ReadAllAsync(ct))
                {
                    await Response.WriteAsync(msg, ct);
                    await Response.Body.FlushAsync(ct);
                }

            }
            catch (OperationCanceledException exp)
            {
                // TODO : Client disconnected - expected, nothing to do
                throw;
            }
            finally
            {
                messageChannel.Writer.TryComplete();
                await subscriber.UnsubscribeAsync(channel);
            }
        }


        [Authorize]
        [HttpGet("history/{roomId}")]
        public async Task<IActionResult> GetHistory(Guid roomId)
        {
            var senderId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!await roomMemberService.IsUserInRoomAsync(roomId, senderId))
            {
                return StatusCode(403, "You are not a member of this room.");
            }

            var dbMessages = await chatService.GetRoomHistoryAsync(roomId);

            return Ok(dbMessages);
        }
    }
}
