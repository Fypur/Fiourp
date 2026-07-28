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
        private Stack<T> pendingAdd = new();
        private Stack<T> pendingRemove = new();

        public IReadOnlyList<T> Items => items;

        public DeferredList() { }

        public DeferredList(IEnumerable<T> initialItems)
        {
            items.AddRange(initialItems);
        }

        public void Add(T item) => pendingAdd.Push(item);
        public void Remove(T item) => pendingRemove.Push(item);
        public void ProcessChanges()
        {
            foreach (var item in pendingRemove) items.Remove(item);
            foreach (var item in pendingAdd) items.Add(item);
            pendingAdd.Clear();
            pendingRemove.Clear();
        }

        public void ProcessChanges(Action<T> onAdded, Action<T> onRemoved)
        {
            while (pendingRemove.Count > 0)
            {
                var item = pendingRemove.Pop();

                onRemoved?.Invoke(item);
                items.Remove(item);
            }
            while (pendingAdd.Count > 0)
            {
                var item = pendingAdd.Pop();

                items.Add(item);
                onAdded?.Invoke(item);
            }
        }
    }
}