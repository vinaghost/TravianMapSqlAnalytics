using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebMVC
{
    public static class Constants
    {
        public const string Select2Combox = "select2-tmsa";

        public static readonly Dictionary<int, string> TribeNames = new()
        {
            {0, "All" },
            {1, "Romans" },
            {2, "Teutons" },
            {3, "Gauls" },
            {4, "Nature " },
            {5, "Natars" },
            {6, "Egyptians" },
            {7, "Huns" },
            {8, "Spartans" },
        };

        public static readonly List<SelectListItem> Tribes =
        [
            new SelectListItem("All", "0"),
            new SelectListItem("Romans", "1"),
            new SelectListItem("Teutons", "2"),
            new SelectListItem("Gauls", "3"),
            new SelectListItem("Natars", "5"),
            new SelectListItem("Egyptians", "6"),
            new SelectListItem("Huns", "7"),
            new SelectListItem("Spartans", "8"),
        ];
    }
}