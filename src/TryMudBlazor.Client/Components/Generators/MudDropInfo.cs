namespace TryMudBlazor.Client.Components.Generators
{
    using System;

    namespace TryMudBlazor.Client.Components.Generators
    {
        /// <summary>
        /// Lightweight POCO that mimics MudBlazor's drop info payload.
        /// Used by local MudDropContainer/OnDrop handlers to provide
        /// the dropped item and source/target indices/containers.
        /// </summary>
        /// <typeparam name="T">Item type being dragged/dropped.</typeparam>
        public class MudDropInfo<T>
        {
            /// <summary>
            /// The item that was dragged.
            /// </summary>
            public T? Item { get; set; }

            /// <summary>
            /// The index in the target collection where the item was dropped.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// The index in the source collection where the item originated.
            /// If not available, -1.
            /// </summary>
            public int FromIndex { get; set; } = -1;

            /// <summary>
            /// Optional reference to the originating container (any object).
            /// </summary>
            public object? FromContainer { get; set; }

            /// <summary>
            /// Optional reference to the target container (any object).
            /// </summary>
            public object? ToContainer { get; set; }

            public MudDropInfo() { }

            public MudDropInfo(T? item, int index = -1, int fromIndex = -1, object? fromContainer = null, object? toContainer = null)
            {
                Item = item;
                Index = index;
                FromIndex = fromIndex;
                FromContainer = fromContainer;
                ToContainer = toContainer;
            }

            public override string ToString()
            {
                return $"MudDropInfo(Item={Item?.ToString() ?? "null"}, Index={Index}, FromIndex={FromIndex})";
            }
        }
    }
}
