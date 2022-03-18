namespace Rido.Mqtt.HubClient
{
    public class EmptyCommandRequest : IBaseCommandRequest<EmptyCommandRequest>
    {
        public EmptyCommandRequest DeserializeBody(string payload)
        {
            return new EmptyCommandRequest();
        }
    }
}
