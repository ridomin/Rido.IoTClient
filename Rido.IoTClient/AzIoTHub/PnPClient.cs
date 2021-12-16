using MQTTnet;
using MQTTnet.Client;
using Rido.IoTClient.AzDps;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub
{
    public class PnPClient //: IDisposable
    {
        public readonly IMqttClient Connection;

        public ConnectionSettings ConnectionSettings;
        public string InitialTwin = string.Empty;

        readonly GetTwinBinder GetTwinBinder;
        public readonly IReportPropertyBinder updateTwinBinder;

        public PnPClient(IMqttClient connection)
        {
            this.Connection = connection;
            GetTwinBinder = GetTwinBinder.GetInstance(connection);
            updateTwinBinder = UpdateTwinBinder.GetInstance(connection);
        }

        public Task<string> GetTwinAsync(CancellationToken cancellationToken = default) => GetTwinBinder.ReadPropertiesDocAsync(cancellationToken);

        public Task<int> UpdateTwinAsync(object payload, CancellationToken cancellationToken = default) => updateTwinBinder.ReportPropertyAsync(payload, cancellationToken);

      

        //public void Dispose()
        //{
        //    Connection.Dispose();
        //}
    }
}
