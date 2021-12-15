using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient
{
    public class xxReadOnlyProperty<T>
    {
        readonly IUpdateTwinBinder updateTwin;
        public readonly string Name;
        readonly string component;
        public T PropertyValue;
        public int Version;

        public xxReadOnlyProperty(IUpdateTwinBinder updateBinder, string name, string component = "")
        {
            updateTwin = updateBinder;
            this.Name = name;
            this.component = component;
        }

        public async Task UpdateTwinPropertyAsync(T newValue, bool asComponent = false, CancellationToken cancellationToken = default)
        {
            PropertyValue = newValue;
            Version = await updateTwin.UpdateTwinAsync(ToJsonDict(asComponent), cancellationToken);
        }

        Dictionary<string, object> ToJsonDict(bool asComponent = false)
        {
            Dictionary<string, object> result;
            if (asComponent == false)
            {
                result = new Dictionary<string, object> { { Name, PropertyValue } };
            }
            else
            {
                Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>>
                {
                    { component, new Dictionary<string, object>() }
                };
                dict[component].Add("__t", "c");
                dict[component].Add(Name, PropertyValue);
                result = dict.ToDictionary(pair => pair.Key, pair => (object)pair.Value);
            }
            return result;
        }
    }
}
