using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IConnectionMultiplexer redis;

        public ChatController(IConnectionMultiplexer _redis)
        {
            redis = _redis;
        }

        [HttpGet("stream/{roomId}")]
        public async Task Stream(int roomId, CancellationToken ct)
        {
            var subscriber = redis.GetSubscriber();
            var channel = $"chatroom:{roomId}";

            var tcs = new TaskCompletionSource();

            await subscriber.SubscribeAsync(channel, async (ch, message) =>
            {
                if (ct.IsCancellationRequested)
                {
                    tcs.TrySetResult();
                    return;
                }

                await Response.WriteAsync(message + "\n");
                await Response.Body.FlushAsync(ct);
            });

            await tcs.Task;

            await subscriber.UnsubscribeAsync(channel);
        }
    }
}
