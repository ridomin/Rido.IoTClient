using Rido.IoTClient.Hive.TopicBindings;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.Tests.Hive
{
    public class CommandBinderFixture
    {

        class CmdRequest : IBaseCommandRequest<CmdRequest>
        {
            public CmdRequest DeserializeBody(string payload) => new();
        }

        class CmdResponse : BaseCommandResponse
        {
            public static string Result => "result";
        }


        [Fact]
        public void ReceiveCommand()
        {
            var mqttClient = new MockMqttClient();
            var command = new Command<CmdRequest, CmdResponse>(mqttClient, "myCmd");
            bool cmdCalled = false;
            command.OnCmdDelegate = async m =>
            {
                cmdCalled = true;
                return await Task.FromResult(new CmdResponse() { Status = 222 });
            };
            mqttClient.SimulateNewMessage("pnp/mock/commands/myCmd", "{}");
            Assert.True(cmdCalled);
        }

        [Fact]
        public void ReceiveCommand_Component()
        {
            var mqttClient = new MockMqttClient();
            var command = new Command<CmdRequest, CmdResponse>(mqttClient, "myCmd", "myComp");
            bool cmdCalled = false;
            command.OnCmdDelegate = async m =>
            {
                cmdCalled = true;
                return await Task.FromResult(new CmdResponse());
            };
            mqttClient.SimulateNewMessage("pnp/mock/commands/myComp*myCmd", "{}");
            Assert.True(cmdCalled);
        }
    }
}
