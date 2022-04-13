namespace Rido.Mqtt.Client
{
    public interface IBaseCommandRequest<T>
    {
        public T DeserializeBody(string payload);
    }
}
