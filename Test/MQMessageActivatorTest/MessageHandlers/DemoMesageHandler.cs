using Dncy.MQMessageActivator;
using MQMessageActivatorTest.Models;

namespace MQMessageActivatorTest.MessageHandlers
{
    public class DemoMesageHandler:MessageHandler
    {

        private readonly ILogger<DemoMesageHandler> _logger;


        /// <inheritdoc />
        public DemoMesageHandler(ILogger<DemoMesageHandler> logger)
        {
            _logger = logger;
        }

        [Subscribe("/{clientId:int}/user/events/score")]
        public void PostMessage(string clientId,UserNewScoreMessage customMessage)
        {
            _logger.LogInformation("hahahahah");
            Console.WriteLine("111111111111");
        }


        [Subscribe("/{clientId:int}/user/events/score")]
        public async Task<string> WithRe(string clientId,UserNewScoreMessage customMessage)
        {
            _logger.LogInformation("ddddddddd");
            Console.WriteLine("111111111111");
            await Task.Delay(1);
            return "123123";
        }
    }
}
