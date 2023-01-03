using Dncy.MQMessageActivator;
using MQMessageActivatorTest.Models;
using MQMessageActivatorTest.Stores;

namespace MQMessageActivatorTest.MessageHandlers
{
    public class DemoMesageHandler:MessageHandler
    {

        private readonly ILogger<DemoMesageHandler> _logger;

        private readonly DemoStore _demoStore;
        private readonly TranDemoStore _tranDemoStore;


        /// <inheritdoc />
        public DemoMesageHandler(ILogger<DemoMesageHandler> logger, DemoStore demoStore, TranDemoStore tranDemoStore)
        {
            _logger = logger;
            _demoStore = demoStore;
            _tranDemoStore = tranDemoStore;
        }

        [Subscribe("/{clientId:int}/user/events/score")]
        public void PostMessage(string clientId,UserNewScoreMessage customMessage)
        {
            _logger.LogInformation("/user/events/score");
            _demoStore.OutPutHashCode();
            _demoStore.OutPutHashCode();
        }


        [Subscribe("/{clientId:int}/user/events/score")]
        public async Task<string> WithRe(string clientId,UserNewScoreMessage customMessage)
        {
            _logger.LogInformation("ddddddddd");
            await Task.Delay(1);
            _demoStore.OutPutHashCode();
            _demoStore.OutPutHashCode();
            return "123123";
        }
    }
}
