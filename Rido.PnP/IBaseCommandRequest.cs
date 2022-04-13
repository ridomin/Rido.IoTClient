namespace Rido.PnP
{
    public interface IBaseCommandRequest<T>
    {
        T DeserializeBody(string payload);
    }
}
