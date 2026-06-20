using System.Drawing;
using MinesServer.GameShit.Buildings;

namespace MinesServer.GameShit.GUI.Horb.Canvas
{
    /// <summary>
    /// Logical container for canvas content.
    /// Renders connections first, then child elements/points.
    /// </summary>
    public sealed class CanvasPanel
    {
        private readonly List<CanvasElement> _children = new();
        private readonly List<Connection> _connections = new();

        // Domain objects like TeleportPoint can be stored separately and
        // converted to CanvasElement[] during Render().
        private readonly List<object> _logicalChildren = new();
        public string Name { get; }
        public int Width { get; }
        public int Height { get; }
        public Color BackgroundColor { get; }

        public CanvasPanel(string name, int width, int height, Color backgroundColor)
        {
            Name = name;
            Width = width;
            Height = height;
            BackgroundColor = backgroundColor;
        }

        /// <summary>
        /// Add raw canvas elements that will be rendered after connections.
        /// </summary>
        public CanvasPanel Add(params CanvasElement[] elements)
        {
            if (elements is { Length: > 0 })
                _children.AddRange(elements);
            return this;
        }

        /// <summary>
        /// Add logical elements that know how to render themselves to canvas elements.
        /// Currently used for TeleportPoint; can be extended for other types.
        /// </summary>
        public CanvasPanel Add(params object[] elements)
        {
            if (elements is { Length: > 0 })
                _logicalChildren.AddRange(elements);
            return this;
        }

        public CanvasPanel AddConnection(params Connection[] connections)
        {
            if (connections is { Length: > 0 })
                _connections.AddRange(connections);
            return this;
        }

        /// <summary>
        /// Renders panel into a flat array of CanvasElement.
        /// Order:
        ///  1. Background
        ///  2. Connections
        ///  3. Raw children
        ///  4. Logical children (e.g. TeleportPoint)
        /// </summary>
        public CanvasElement[] Render()
        {
            var result = new List<CanvasElement>();

            // Background fill (same behavior as old Buttonsg: Image + Rect)
            result.Add(CanvasElement.Image(Name, Width, Height));
            result.Add(CanvasElement.Rect(BackgroundColor, Width, Height, offsetX: 0, offsetY: 0));

            // 1) Connections
            foreach (var connection in _connections)
            {
                result.AddRange(connection.Render());
            }

            // 2) Raw children
            result.AddRange(_children);

            // 3) Logical children that know how to render themselves
            foreach (var child in _logicalChildren)
            {
                switch (child)
                {
                    case TeleportPoint tp:
                        result.AddRange(tp.Render());
                        break;
                }
            }

            return result.ToArray();
        }
    }
}

