namespace ConsoleUpdate.Extensions
{
#pragma warning disable CS8603 // Possible null reference return.

    public static class MapSqlParserExtensions
    {
        private static string Peek(this string source, int peek) => source == null || peek < 0 ? null : source.Substring(0, source.Length < peek ? source.Length : peek);

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.

        private static (string, string) Pop(this string source, int pop) => source == null || pop < 0 ? (null, source) : (source.Substring(0, source.Length < pop ? source.Length : pop), source.Length < pop ? string.Empty : source.Substring(pop));

#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

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

#pragma warning restore CS8603 // Possible null reference return.
}