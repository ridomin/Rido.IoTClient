using System;
using System.Threading.Tasks;

namespace Rido.MqttCore.PnP
{
    /// <summary>
    /// Generic Command interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface ICommand<T, TResponse>
        where T : IBaseCommandRequest<T>, new()
        where TResponse : BaseCommandResponse
    {
        /// <summary>
        /// Delegate to expose command invocations
        /// </summary>
        Func<T, Task<TResponse>> OnCmdDelegate { get; set; }
    }
}