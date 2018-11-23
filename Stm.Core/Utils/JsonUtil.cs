using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Utils
{
    public class JsonUtil
    {
        public static string ToJson(Object data)
        {
            if (data == null) return "";

            return JsonConvert.SerializeObject(data);

        }



        public static T ToModel<T>(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return default(T);

            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
