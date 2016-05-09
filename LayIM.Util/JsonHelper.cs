using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LayIM.Util
{
    public class JsonHelper
    {

        public static string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T DeserializeObject<T>(string obj)
        {
            return JsonConvert.DeserializeObject<T>(obj);
        }
    }
}
