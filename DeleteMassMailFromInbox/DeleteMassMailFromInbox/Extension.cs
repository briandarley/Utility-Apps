using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeleteMassMailFromInbox
{
    public static class Extension
    {
        public static string ToQueryParams<T>(this T value) where T : class
        {
            Dictionary<string, object> source = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject((object)value));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, object> keyValuePair in source.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>)(c => c.Value != null)).Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>)(c => !(c.Value is JArray))).Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>)(c => c.Value.ToString().Length > 0)))
                stringBuilder.Append(keyValuePair.Key + "=" + HttpUtility.UrlEncode(keyValuePair.Value.ToString()) + "&");
            foreach (KeyValuePair<string, object> keyValuePair in source.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>)(c => c.Value is JArray)))
            {
                foreach (JValue jvalue in (JArray)keyValuePair.Value)
                    stringBuilder.Append(keyValuePair.Key + "=" + HttpUtility.UrlEncode(jvalue.Value.ToString()) + "&");
            }
            return Regex.Replace(stringBuilder.ToString(), "\\&$", "");
        }
    }
}
