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
        private readonly IConnectionMultiplexer redis;

        public ChatController(IConnectionMultiplexer _redis, IChatService _chatService)
        {
            redis = _redis;
            chatService = _chatService;
        }


        [HttpPost("send")]
        [Authorize]
        public async Task<IActionResult> SendMessage(SendMessageDto dto)
        {
            var senderId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await chatService.SendMessageAsync(senderId, dto);
            return Ok();
        }


        [HttpGet("stream/{roomId}")]
        public async Task Stream(Guid roomId, CancellationToken ct)
        {
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


        [HttpGet("history/{roomId}")]
        public async Task<IActionResult> GetHistory(Guid roomId)
        {
            var db = redis.GetDatabase();
            var cacheKey = $"chat:room:{roomId}:messages";
            var cached = await db.ListRangeAsync(cacheKey, 0, -1);

            if (cached.Length > 0)
            {
                var messages = cached.Select(x => JsonSerializer.Deserialize<object>(x)).ToList();
                return Ok(messages);
            }

            // Cache boşsa DB'den çek
            var dbMessages = await chatService.GetRoomHistoryAsync(roomId);
            return Ok(dbMessages);
        }
    }
}
