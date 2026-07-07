using System;
using System.Collections.Generic;

namespace Fiourp
{
    /// <summary>
    /// Adding or removing elements in this list doesn't happen instantly, but only when ProcessChanges is called
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DeferredList<T>
    {
        private List<T> items = new();
        private List<T> pendingAdd = new();
        private List<T> pendingRemove = new();

        public IReadOnlyList<T> Items => items;

        public DeferredList() { }

        public DeferredList(IEnumerable<T> initialItems)
        {
            items.AddRange(initialItems);
        }

        public void Add(T item) => pendingAdd.Add(item);
        public void Remove(T item) => pendingRemove.Add(item);
        public void ProcessChanges()
        {
            foreach (var item in pendingRemove) items.Remove(item);
            foreach (var item in pendingAdd) items.Add(item);
            pendingAdd.Clear();
            pendingRemove.Clear();
        }

        public void ProcessChanges(Action<T> onAdded, Action<T> onRemoved)
        {
            foreach (var item in pendingRemove)
            {
                onRemoved?.Invoke(item);
                items.Remove(item);
            }
            foreach (var item in pendingAdd)
            {
                items.Add(item);
                onAdded?.Invoke(item);
            }
            pendingAdd.Clear();
            pendingRemove.Clear();
        }
    }
}