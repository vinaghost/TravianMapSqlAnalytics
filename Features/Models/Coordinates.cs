using System.Linq.Expressions;

namespace Features.Models
{
    public record Coordinates(int X, int Y)
    {
        public override string ToString()
        {
            return $"{X}|{Y}";
        }
    }
    public record BoudingBox(int MinX, int MaxX, int MinY, int MaxY);

    public static class CoordinatesExtenstion
    {
        private const int _sizeWorld = 200;

        public static double Distance(this Coordinates coord1, int xX, int yY)
        {
            var x = Delta(coord1.X, xX);
            var y = Delta(coord1.Y, yY);
            return Math.Round(Math.Sqrt(x * x + y * y), 2);
        }

        [Expandable(nameof(DistanceImpl))]
        public static long Distance(int x1, int y1, int x2, int y2)
        {
            var x = Delta(x1, x2);
            var y = Delta(y1, y2);
            return x * x + y * y;
        }

        private static Expression<Func<int, int, int, int, long>> DistanceImpl()
        {
            return (x1, y1, x2, y2) =>
                (long)Math.Pow(DeltaImpl().Invoke(x1, x2), 2) + (long)Math.Pow(DeltaImpl().Invoke(y1, y2), 2);
        }

        private static Expression<Func<int, int, long>> DeltaImpl()
        {
            return (c1, c2) => (c1 - c2 + 3 * _sizeWorld + 1) % (2 * _sizeWorld + 1) - _sizeWorld;
        }

        private static long Delta(int c1, int c2)
        {
            return (c1 - c2 + 3 * _sizeWorld + 1) % (2 * _sizeWorld + 1) - _sizeWorld;
        }
    }
}