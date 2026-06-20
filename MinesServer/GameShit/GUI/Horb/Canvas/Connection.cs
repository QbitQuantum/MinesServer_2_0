using System.Drawing;
using MinesServer.GameShit.Buildings;

namespace MinesServer.GameShit.GUI.Horb.Canvas
{
    /// <summary>
    /// Connection between two teleport points or from a point to arbitrary coordinates.
    /// Responsible for rendering itself as one or many CanvasElement.Line objects.
    /// </summary>
    public sealed class Connection
    {
        public TeleportPoint From { get; }
        public TeleportPoint? To { get; }
        public int? TargetX { get; }
        public int? TargetY { get; }
        public Color Color { get; }
        public bool IsDashed { get; }
        public int DashLength { get; }
        public int GapLength { get; }

        public Connection(
            TeleportPoint from,
            TeleportPoint to,
            Color color,
            bool isDashed = false,
            int dashLength = 20,
            int gapLength = 10)
        {
            From = from;
            To = to;
            Color = color;
            IsDashed = isDashed;
            DashLength = dashLength;
            GapLength = gapLength;
        }

        public Connection(
            TeleportPoint from,
            int targetX,
            int targetY,
            Color color,
            bool isDashed = false,
            int dashLength = 20,
            int gapLength = 10)
        {
            From = from;
            TargetX = targetX;
            TargetY = targetY;
            Color = color;
            IsDashed = isDashed;
            DashLength = dashLength;
            GapLength = gapLength;
        }

        public IEnumerable<CanvasElement> Render()
        {
            var elements = new List<CanvasElement>();

            int startX = From.CanvasX;
            int startY = From.CanvasY;
            int endX = To?.CanvasX ?? TargetX!.Value;
            int endY = To?.CanvasY ?? TargetY!.Value;

            if (!IsDashed)
            {
                elements.Add(CanvasElement.Line(
                    Color,
                    offsetX: startX,
                    offsetY: startY,
                    dx: endX,
                    dy: endY));
                return elements;
            }

            int totalDx = endX - startX;
            int totalDy = endY - startY;
            double totalLength = Math.Sqrt(totalDx * totalDx + totalDy * totalDy);

            if (totalLength < 0.1) return elements;

            double dirX = totalDx / totalLength;
            double dirY = totalDy / totalLength;
            double currentDist = 0;

            while (currentDist < totalLength)
            {
                double dashStart = currentDist;
                double dashEnd = Math.Min(currentDist + DashLength, totalLength);

                if (dashEnd > dashStart)
                {
                    int segStartX = startX + (int)(dirX * dashStart);
                    int segStartY = startY + (int)(dirY * dashStart);
                    int segEndX = startX + (int)(dirX * dashEnd);
                    int segEndY = startY + (int)(dirY * dashEnd);

                    elements.Add(CanvasElement.Line(
                        Color,
                        offsetX: segStartX,
                        offsetY: segStartY,
                        dx: segEndX,
                        dy: segEndY));
                }

                currentDist = dashEnd + GapLength;
            }

            return elements;
        }
    }
}

