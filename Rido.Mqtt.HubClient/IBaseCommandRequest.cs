namespace Rido.Mqtt.HubClient
{
    public interface IBaseCommandRequest<T>
    {
        public T DeserializeBody(string payload);
    }
}
