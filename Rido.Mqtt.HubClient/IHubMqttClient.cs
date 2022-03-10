using Rido.MqttCore;
using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient
{
    public interface IHubMqttClient
    {
        IMqttBaseClient Connection { get; set; }
        Func<GenericCommandRequest, Task<GenericCommandResponse>> OnCommandReceived { get; set; }
        Func<JsonNode, Task<GenericPropertyAck>> OnPropertyUpdateReceived { get; set; }

        Task<string> GetTwinAsync(CancellationToken cancellationToken = default);
        Task<int> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default);
        Task<int> SendTelemetryAsync(object payload, CancellationToken t = default);
    }
}