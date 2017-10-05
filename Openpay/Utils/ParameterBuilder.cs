using System.Net;

namespace Openpay.Utils
{
    internal static class ParameterBuilder
    {
        public static string ApplyParameterToUrl(string url, string argument, string value)
        {
            var token = "&";

            if (!url.Contains("?"))
                token = "?";

            return string.Format("{0}{1}{2}={3}", url, token, argument, WebUtility.UrlEncode(value));
        }
    }
}
