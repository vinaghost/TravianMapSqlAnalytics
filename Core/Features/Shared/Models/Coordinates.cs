using LinqKit;
using System.Linq.Expressions;

namespace Core.Features.Shared.Models
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
        private const int _deltaWorld = 2 * _sizeWorld + 1;

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
                Power().Invoke(DeltaImpl().Invoke(x1, x2)) + Power().Invoke(DeltaImpl().Invoke(y1, y2));
        }

        private static Expression<Func<long, long>> Power()
        {
            return (x) => x * x;
        }

        private static Expression<Func<int, int, long>> DeltaImpl()
        {
            return (c1, c2) => (c1 - c2 + 3 * _sizeWorld + 1) % (2 * _sizeWorld + 1) - _sizeWorld;
        }

        private static long Delta(int c1, int c2)
        {
            return (c1 - c2 + 3 * _sizeWorld + 1) % (2 * _sizeWorld + 1) - _sizeWorld;
        }

        public static bool InSimpleRange(this Coordinates coord1, Coordinates coord2, int distance)
        {
            var x = Delta(coord1.X, coord2.X);
            var y = Delta(coord1.Y, coord2.Y);
            return x + y <= distance;
        }

        public static IList<BoudingBox> GetBoudingBoxes(this Coordinates coord, int distance)
        {
            var (x, y) = coord;

            var x_max = x + _sizeWorld;
            var y_max = y + _sizeWorld;

            var x_min = x - _sizeWorld;
            var y_min = y - _sizeWorld;

            var box = new BoudingBox(x_min, x_max, y_min, y_max);

            if (IsValid(box)) return [box];

            if (IsValidOneSide(box))
            {
                if (x_max > _sizeWorld || x_min < -_sizeWorld)
                {
                    return box.GetXSide();
                }
                else
                {
                    return box.GetYSide();
                }
            }
            else
            {
                return box.Get4Side();
            }
        }

        private static bool IsValid(BoudingBox box)
        {
            var (x_min, x_max, y_min, y_max) = box;
            return x_max < _sizeWorld && x_min > -_sizeWorld && y_max < _sizeWorld && y_min > -_sizeWorld;
        }

        private static bool IsValidOneSide(BoudingBox box)
        {
            var (x_min, x_max, y_min, y_max) = box;
            var x_side = x_max > _sizeWorld || x_min < -_sizeWorld;
            var y_side = y_max > _sizeWorld || y_min < -_sizeWorld;
            return x_side != y_side;
        }

        private static IList<BoudingBox> GetXSide(this BoudingBox box)
        {
            var (x_min, x_max, y_min, y_max) = box;

            if (x_max > _sizeWorld)
            {
                return [
                    new BoudingBox(x_min, 200, y_min, y_max),
                    new BoudingBox(-200, x_max - _deltaWorld, y_min, y_max),
                ];
            }
            else
            {
                return [
                    new BoudingBox(x_min + _deltaWorld, 200, y_min, y_max),
                    new BoudingBox(-200, x_max, y_min, y_max),
                ];
            }
        }

        private static IList<BoudingBox> GetYSide(this BoudingBox box)
        {
            var (x_min, x_max, y_min, y_max) = box;

            if (y_max > _sizeWorld)
            {
                return [
                    new BoudingBox(x_min, x_max, y_min, 200),
                    new BoudingBox(x_min, x_max, -200, y_max - _deltaWorld),
                ];
            }
            else
            {
                return [
                    new BoudingBox(x_min, x_max, y_min + _deltaWorld, 200),
                    new BoudingBox(x_min, x_max, -200, y_max),
                ];
            }
        }

        private static IList<BoudingBox> Get4Side(this BoudingBox box)
        {
            var (x_min, x_max, y_min, y_max) = box;

            var x = new List<int>
            {
                x_min < -_sizeWorld ? x_min + _deltaWorld : x_min,
                x_max > _sizeWorld ? x_max - _deltaWorld : x_max,
            };

            var y = new List<int>
            {
                y_min < -_sizeWorld ? y_min + _deltaWorld : y_min,
                y_max > _sizeWorld ? y_max - _deltaWorld : y_max,
            };

            x_min = x.Min();
            x_max = x.Max();

            y_min = y.Min();
            y_max = y.Max();

            return [
                new BoudingBox(x_max, 200, y_max, 200),
                new BoudingBox(x_max, 200, -200, y_min),
                new BoudingBox(-200, x_min, y_max, 200),
                new BoudingBox(-200, x_min, -200, y_min),

            ];
        }
    }
}