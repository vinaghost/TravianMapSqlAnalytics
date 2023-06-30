using MainCore.Models;

namespace MainCore.Parsers
{
    public static class MapSqlParser
    {
        public static List<Village> GetVillages(string lines)
        {
            var villageLines = lines.Split("\n");
            var villages = new List<Village>();
            foreach (var line in villageLines)
            {
                var village = GetVillage(line);
                if (village == null) continue;
                villages.Add(village);
            }
            return villages;
        }

        public static Village GetVillage(string line)
        {
            if (string.IsNullOrEmpty(line)) return null;
            var villageLine = line.Remove(0, 30);
            villageLine = villageLine.Remove(villageLine.Length - 2, 2);
            var fields = villageLine.ParseLine();

            var MapId = int.Parse(fields[0]);
            var X = int.Parse(fields[1]);
            var Y = int.Parse(fields[2]);
            var Tribe = int.Parse(fields[3]);
            var Id = int.Parse(fields[4]);
            var Name = fields[5];
            var PlayerId = int.Parse(fields[6]);
            var PlayerName = fields[7];
            var AllyId = int.Parse(fields[8]);
            var AllyName = fields[9];
            var Pop = int.Parse(fields[10]);
            var Region = fields[11];
            var IsCapital = fields[12].Equals("TRUE");
            var IsCity = fields[13].Equals("TRUE");
            var VictoryPoints = fields[14].Equals("NULL") ? 0 : int.Parse(fields[14]);
            return new Village(MapId, X, Y, Tribe, Id, Name, PlayerId, PlayerName, AllyId, AllyName, Pop, Region, IsCapital, IsCity, VictoryPoints);
        }
    }

    public static class MapSqlExtensions
    {
        private static string Peek(this string source, int peek) => (source == null || peek < 0) ? null : source.Substring(0, source.Length < peek ? source.Length : peek);

        private static (string, string) Pop(this string source, int pop) => (source == null || pop < 0) ? (null, source) : (source.Substring(0, source.Length < pop ? source.Length : pop), source.Length < pop ? String.Empty : source.Substring(pop));

        public static string[] ParseLine(this string line)
        {
            return ParseLineImpl(line).ToArray();

            static IEnumerable<string> ParseLineImpl(string l)
            {
                string remainder = l;
                string field;
                while (remainder.Peek(1) != "")
                {
                    (field, remainder) = ParseField(remainder);
                    yield return field;
                }
            }
        }

        private const string GroupOpen = "'";
        private const string GroupClose = "'";

        private static (string field, string remainder) ParseField(string line)
        {
            if (line.Peek(1) == GroupOpen)
            {
                var (_, split) = line.Pop(1);
                return ParseFieldQuoted(split);
            }
            else
            {
                var field = "";
                var (head, tail) = line.Pop(1);
                while (head != "," && head != "")
                {
                    field += head;
                    (head, tail) = tail.Pop(1);
                }
                return (field, tail);
            }
        }

        private static (string field, string remainder) ParseFieldQuoted(string line) => ParseFieldQuoted(line, false);

        private static (string field, string remainder) ParseFieldQuoted(string line, bool isNested)
        {
            var field = "";
            var head = "";
            var tail = line;
            while (tail.Peek(1) != "" && tail.Peek(1) != GroupClose)
            {
                if (tail.Peek(1) == GroupOpen)
                {
                    (head, tail) = tail.Pop(1);
                    (head, tail) = ParseFieldQuoted(tail, true);
                    field += GroupOpen + head + GroupClose;
                }
                else
                {
                    (head, tail) = tail.Pop(1);
                    field += head;
                }
            }
            if (tail.Peek(2) == GroupClose + ",")
            {
                (head, tail) = tail.Pop(isNested ? 1 : 2);
            }
            else if (tail.Peek(1) == GroupClose)
            {
                (head, tail) = tail.Pop(1);
            }
            return (field, tail);
        }
    }
}