using Microsoft.AspNetCore.Mvc;

namespace MapSqlAspNetCoreMVC.Extension
{
    public static class ControllerExtensions
    {
        public static string ShortControllerName<T>() where T : Controller
        {
            return typeof(T).Name.Replace("Controller", "");
        }

        public static string RemoveController(this string fullControllerClassName)
        {
            return fullControllerClassName.Replace("Controller", "");
        }
    }
}