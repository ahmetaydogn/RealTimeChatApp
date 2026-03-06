using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Json;

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
            Response.Headers["Content-Type"] = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["Connection"] = "keep-alive";

            var subscriber = redis.GetSubscriber();
            var channel = $"chatroom:{roomId}";

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously); ;

            ct.Register(() => tcs.TrySetResult(true));

            await subscriber.SubscribeAsync(channel, async (ch, message) =>
            {
                if (ct.IsCancellationRequested)
                    return;

                await Response.WriteAsync($"data: {message}\n\n", ct);
                await Response.Body.FlushAsync(ct);
            });

            await Response.WriteAsync("event: ping\ndata: connected\n\n", ct);
            await Response.Body.FlushAsync(ct);

            await tcs.Task;

            await subscriber.UnsubscribeAsync(channel);
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
