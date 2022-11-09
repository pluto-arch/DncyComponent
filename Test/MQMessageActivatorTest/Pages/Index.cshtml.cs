using Dncy.MQMessageActivator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MQMessageActivatorTest.Models;
using System.Text.Json;

namespace MQMessageActivatorTest.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly MessageHandlerActivator _messageHandlerActivator;

        public IndexModel(ILogger<IndexModel> logger,MessageHandlerActivator messageHandlerActivator)
        {
            _logger = logger;
            _messageHandlerActivator = messageHandlerActivator;
        }

        public async Task OnGet()
        {
            var json = new UserNewScoreMessage
            {
                UserId = 99823,
                Score = 20
            };

            await _messageHandlerActivator.ProcessRequestAsync("/110083/user/events/score",JsonSerializer.Serialize(json));
        }
    }
}