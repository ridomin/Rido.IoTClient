using System.Threading;
using System.Threading.Tasks;

namespace Rido.MqttCore.PnP
{
    /// <summary>
    /// Interface for device properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyProperty<T>
    {
        /// <summary>
        /// Property Name 
        /// </summary>
        string PropertyName { get; }
        /// <summary>
        /// PropertyValue 
        /// </summary>
        T PropertyValue { get; set; }
        /// <summary>
        /// Property version (Device Twin reported $version)
        /// </summary>
        int Version { get; set; }
        /// <summary>
        /// Submit property
        /// </summary>
        /// <param name="cancellationToken">async cancel token</param>
        /// <returns></returns>
        Task<int> ReportPropertyAsync(CancellationToken cancellationToken = default);
    }
}