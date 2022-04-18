namespace Rido.MqttCore.PnP
{
    public interface IBaseCommandRequest<T>
    {
        T DeserializeBody(string payload);
    }
}
