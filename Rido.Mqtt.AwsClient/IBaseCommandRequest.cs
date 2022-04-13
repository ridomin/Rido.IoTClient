namespace Rido.Mqtt.AwsClient
{
    public interface IBaseCommandRequest<T>
    {
        public T DeserializeBody(string payload);
    }
}
