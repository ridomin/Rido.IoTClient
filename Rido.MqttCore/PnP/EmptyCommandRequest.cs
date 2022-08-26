namespace Rido.MqttCore.PnP
{
    /// <summary>
    /// Empty Command Request for untyped clients
    /// </summary>
    public class EmptyCommandRequest : IBaseCommandRequest<EmptyCommandRequest>
    {
        /// <summary>
        /// Deserialize payload
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public EmptyCommandRequest DeserializeBody(string payload)
        {
            return new EmptyCommandRequest();
        }
    }
}
