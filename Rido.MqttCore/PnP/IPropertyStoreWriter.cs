﻿using System.Threading;
using System.Threading.Tasks;

namespace Rido.MqttCore.PnP
{
    public interface IPropertyStoreWriter
    {
        Task<int> ReportPropertyAsync(object payload, CancellationToken token = default);
    }
}
