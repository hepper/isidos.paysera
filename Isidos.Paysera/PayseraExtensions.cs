using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace Isidos.Paysera
{
    public static class PayseraExtensions
    {
        public static string ToQueryString(this NameValueCollection source)
        {
            var query = string.Join("&",
                    source.AllKeys
                        .Where(key => !string.IsNullOrWhiteSpace(source[key]))
                        .Select(key => string.Join("&", source.GetValues(key)
                            .Select(val => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(val))))));

            return string.Join("&", query);
        }

        public static string ToString(this bool? source, string defaultValue = "")
        {
            if (!source.HasValue)
            {
                return !string.IsNullOrEmpty(defaultValue) 
                    ? defaultValue 
                    : string.Empty;
                
            }
            return source.Value ? "1" : "0";
        }

        public static int ToInt(this string source, int? defaultValue = null)
        {
            if (!string.IsNullOrEmpty(source))
            {
                int result;
                if (int.TryParse(source, out result))
                    return result;

            }
            return defaultValue.HasValue ? defaultValue.Value : default(int);
        }

        public static int? ToNullableInt(this string source)
        {
            if (!string.IsNullOrEmpty(source))
            {
                int result;
                if (int.TryParse(source, out result))
                    return result;

            }
            return null;
        }

        public static EntityValidationResult Validate<T>(this T entity) where T : class
        {
            return new EntityValidator<T>().Validate(entity);
        }

        
        public static string EncodeBase64UrlSafe(this string text)
        {
            var textAsBytes = Encoding.UTF8.GetBytes(text);
            var encodedText = Convert.ToBase64String(textAsBytes);
            encodedText = encodedText.Replace('+', '-');
            encodedText = encodedText.Replace('/', '_');
            return encodedText;
        }


        public static string DecodeBase64UrlSafe(this string encodedText)
        {
            encodedText = encodedText.Replace('-', '+');
            encodedText = encodedText.Replace('_', '/');
            var textAsBytes = Convert.FromBase64String(encodedText);
            var text = Encoding.UTF8.GetString(textAsBytes);
            return text;
        }
    }
}