﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient
{
    public interface IPropertyStoreWriter
    {
        Task<int> ReportPropertyAsync(object payload, CancellationToken token = default);
    }
}
