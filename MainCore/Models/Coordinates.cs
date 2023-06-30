namespace MainCore.Models
{
    public class Coordinates
    {
        public Coordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString()
        {
            return $"{X}|{Y}";
        }
    }

    public static class CoordinatesExtenstion
    {
        private const int _sizeWorld = 200;

        public static double Distance(this Coordinates coord1, Coordinates coord2)
        {
            var x = Delta(coord1.X, coord2.X);
            var y = Delta(coord1.Y, coord2.Y);
            return Math.Round(Math.Sqrt(x * x + y * y), 2);
        }

        private static double Delta(int c1, int c2)
        {
            return (c1 - c2 + (3 * _sizeWorld + 1)) % (2 * _sizeWorld + 1) - _sizeWorld;
        }
    }
}