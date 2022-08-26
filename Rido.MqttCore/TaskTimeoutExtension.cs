using System;
using System.Threading.Tasks;

namespace Rido.MqttCore
{
    /// <summary>
    /// TaskTimeoutExtension
    /// </summary>
    public static class TaskTimeoutExtension
    {
        /// <summary>
        /// Time out async task
        /// </summary>
        /// <typeparam name="T">name</typeparam>
        /// <param name="source">source</param>
        /// <param name="timeout">timeout as TimeSpan</param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public static async Task<T> TimeoutAfter<T>(this Task<T> source, TimeSpan timeout)
        {
            if (await Task.WhenAny(source, Task.Delay(timeout)) != source)
            {
                throw new TimeoutException();
            }
            return await source;
        }
    }
}
