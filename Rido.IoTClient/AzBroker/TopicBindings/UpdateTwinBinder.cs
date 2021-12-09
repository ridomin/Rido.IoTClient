﻿using MQTTnet.Client;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzBroker.TopicBindings
{
    public class UpdateTwinBinder
    {
        readonly ConcurrentDictionary<int, TaskCompletionSource<int>> pendingRequests;
        readonly IMqttClient connection;

        public UpdateTwinBinder(IMqttClient connection)
        {
            pendingRequests = new ConcurrentDictionary<int, TaskCompletionSource<int>>();
            this.connection = connection;
            _ = connection.SubscribeAsync("$az/iot/twin/patch/response/+");
            connection.ApplicationMessageReceivedAsync += async m =>
            {
                var topic = m.ApplicationMessage.Topic;
                if (topic.StartsWith("$az/iot/twin/patch/response/"))
                {
                    (int rid, int twinVersion) = TopicParser.ParseTopic(topic);
                    if (pendingRequests.TryRemove(rid, out var tcs))
                    {
                        tcs.SetResult(twinVersion);
                    }
                }
                await Task.Yield();
            };
        }

        public async Task<int> UpdateTwinAsync(object payload, CancellationToken cancellationToken = default)
        {
            var rid = RidCounter.NextValue();
            var puback = await connection.PublishAsync($"$az/iot/twin/patch/reported/?rid={rid}", payload, cancellationToken);
            var tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            if (puback?.ReasonCode == MqttClientPublishReasonCode.Success)
            {
                pendingRequests.TryAdd(rid, tcs);
            }
            else
            {
                Trace.TraceError($"Error '{puback?.ReasonCode}' publishing twin GET");
            }
            return await tcs.Task.TimeoutAfter(TimeSpan.FromSeconds(5));
        }
    }
}