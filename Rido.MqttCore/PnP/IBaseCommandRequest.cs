namespace Rido.MqttCore.PnP
{
    /// <summary>
    /// Interface to enable Command Request Deserialization
    /// </summary>
    /// <typeparam name="T">Type to deserialize</typeparam>
    public interface IBaseCommandRequest<T>
    {
        /// <summary>
        /// Deserialize command request
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        T DeserializeBody(string payload);
    }
}
