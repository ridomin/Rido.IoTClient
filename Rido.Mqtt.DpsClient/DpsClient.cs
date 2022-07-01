using Rido.MqttCore;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rido.Mqtt.DpsClient
{
    public class MqttDpsClient
    {
        readonly IMqttBaseClient mqttClient;
        int rid = 0;
        readonly TaskCompletionSource<DpsStatus> tcs = new TaskCompletionSource<DpsStatus>();

        public MqttDpsClient(IMqttBaseClient c)
        {
            mqttClient = c;
            _ = mqttClient.SubscribeAsync("$dps/registrations/res/#").ConfigureAwait(false);
            mqttClient.OnMessage += async m =>
            {
                if (m.Topic.StartsWith($"$dps/registrations/res/"))
                {
                    var topicSegments = m.Topic.Split('/');
                    int reqStatus = Convert.ToInt32(topicSegments[3]);
                    if (reqStatus >= 400)
                    {
                        tcs.SetException(new ApplicationException(m.Payload));
                    }
                    var dpsRes = JsonSerializer.Deserialize<DpsStatus>(m.Payload);
                    if (dpsRes != null && dpsRes.Status == "assigning")
                    {
                        // TODO: ready retry-after
                        await Task.Delay(2500); //avoid throtling
                        var pollTopic = $"$dps/registrations/GET/iotdps-get-operationstatus/?$rid={rid++}&operationId={dpsRes.OperationId}";
                        var puback = await mqttClient.PublishAsync(pollTopic, string.Empty);
                    }
                    else
                    {
                        if (dpsRes != null && dpsRes.Status == "assigned")
                        {
                            tcs.SetResult(dpsRes);
                        }
                    }
                }
            };
        }

        public async Task<DpsStatus> ProvisionDeviceIdentity()
        {
            var putTopic = $"$dps/registrations/PUT/iotdps-register/?$rid={rid++}";
            var registrationId = mqttClient.ConnectionSettings.DeviceId;
            var modelId = mqttClient.ConnectionSettings.ModelId;
            var puback = await mqttClient.PublishAsync(putTopic, new { registrationId, payload = new { modelId } });
            if (puback > 0)
            {
                throw new ApplicationException("PubAck > 0 publishing DPS PUT");
            }
            return await tcs.Task.TimeoutAfter(TimeSpan.FromSeconds(30));
        }

    }
}
