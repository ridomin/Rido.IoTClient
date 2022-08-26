using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.MqttCore.PnP
{
    /// <summary>
    /// IWritableProperty
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWritableProperty<T>
    {
        /// <summary>
        /// PropertyName
        /// </summary>
        string PropertyName { get; }
        /// <summary>
        /// PropertyValue
        /// </summary>
        PropertyAck<T> PropertyValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Func<PropertyAck<T>, Task<PropertyAck<T>>> OnProperty_Updated { get; set; }

        /// <summary>
        /// Intializes the device twin based on a default value
        /// </summary>
        /// <param name="twin"></param>
        /// <param name="defaultValue"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task InitPropertyAsync(string twin, T defaultValue, CancellationToken cancellationToken = default);
        /// <summary>
        /// ReportPropertyAsync
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<int> ReportPropertyAsync(CancellationToken token = default);
    }
}