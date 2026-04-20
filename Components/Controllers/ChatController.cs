using Azure.AI.OpenAI;
using Azure.AI.OpenAI.Chat;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;

namespace StudyAIChat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly AzureOpenAIClient? _client;
        private readonly string? _deployment;

        public ChatController(AzureOpenAIClient? client, IConfiguration config)
        {
            _client = client;
            _deployment = config["AzureOpenAI:DeploymentName"];
        }

        public class ChatRequest
        {
            public string? Message { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Message))
                return BadRequest("Message is required.");

            string reply = "";
            if (_client != null) 
            {
                var chatClient = _client.GetChatClient(_deployment);

                ChatCompletion completion = await chatClient.CompleteChatAsync(
                    req.Message
                );

                reply = completion.Content[0].Text;
            }
            else
            {
                return BadRequest("Azure OpenAI client is not configured.");
            }
            
            return Ok(new { reply });
        }
    }
}
