namespace pnp_temperature_controller
{
    public class FixedSizeDictonary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : notnull
    {
        readonly int size;
        readonly Queue<TKey> orderedKeys = new();
        public FixedSizeDictonary(int maxSize) => size = maxSize;
        public new void Add(TKey key, TValue value)
        {
            orderedKeys.Enqueue(key);
            if (size != 0 && Count >= size) Remove(orderedKeys.Dequeue());
            base.Add(key, value);
        }
    }
}
