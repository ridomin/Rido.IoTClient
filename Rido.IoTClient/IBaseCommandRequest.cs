namespace Rido.IoTClient
{
    public interface IBaseCommandRequest<T>
    {
        public T DeserializeBody(string payload);
    }
}
