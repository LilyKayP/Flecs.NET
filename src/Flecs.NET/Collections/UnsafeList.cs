using System;
using System.Runtime.CompilerServices;
using Flecs.NET.Utilities;

namespace Flecs.NET.Collections
{
    /// <summary>
    ///     Unsafe list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public unsafe struct UnsafeList<T> : IDisposable
    {
        /// <summary>
        ///     Data storage for the unsafe list.
        /// </summary>
        public void* Data { get; private set; }

        /// <summary>
        ///     The capacity of the unsafe list.
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        ///     The current count of the unsafe list.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        ///     Represents whether or not the unsafe list is null.
        /// </summary>
        public readonly bool IsNull => Data == null;

        /// <summary>
        ///     Represents whether or not the unsafe list stores a managed type.
        /// </summary>
        public readonly bool IsManaged => RuntimeHelpers.IsReferenceOrContainsReferences<T>();

        /// <summary>
        ///     Creates an unsafe list with the specified capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public UnsafeList(int capacity)
        {
            if (capacity <= 0)
            {
                Data = null;
                Capacity = default;
                Count = default;
            }

            Data = Memory.AllocZeroed(capacity * Managed.ManagedSize<T>());
            Capacity = capacity;
            Count = 0;
        }

        /// <summary>
        ///     Gets a managed reference to the object at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="ArgumentException"></exception>
        public readonly ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((uint)index >= (uint)Count)
                    throw new ArgumentException($"List index {index} is out of range.", nameof(index));

                return ref Managed.GetTypeRef<T>(Data, index);
            }
        }

        /// <summary>
        ///     Adds an item to the unsafe list.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (Count == Capacity)
            {
                int newCapacity = Utils.NextPowOf2(Count + 1);
                Data = Memory.Realloc(Data, newCapacity * Managed.ManagedSize<T>());
                Capacity = newCapacity;
            }

            Managed.SetTypeRef(Data, item, Count++);
        }

        /// <summary>
        ///     Disposes the unsafe list and frees resources.
        /// </summary>
        public void Dispose()
        {
            if (Data == null)
                return;

            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                for (int i = 0; i < Count; i++)
                    Managed.FreeGcHandle(Data, i);

            Memory.Free(Data);
            Data = null;
        }
    }
}
