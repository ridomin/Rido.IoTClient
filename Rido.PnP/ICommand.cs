using System;
using System.Threading.Tasks;

namespace Rido.PnP
{
    public interface ICommand<T, TResponse>
        where T : IBaseCommandRequest<T>, new()
        where TResponse : BaseCommandResponse
    {
        Func<T, Task<TResponse>> OnCmdDelegate { get; set; }
    }
}