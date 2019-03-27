using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Lib
{

    public class Kits
    {
        public static string ToJsonString(object obj)
        {
            System.Web.Script.Serialization.JavaScriptSerializer jsoner = new System.Web.Script.Serialization.JavaScriptSerializer();
            return jsoner.Serialize(obj);
        }

        public static T Deserialize<T>(string jsonStr)
        {
            System.Web.Script.Serialization.JavaScriptSerializer jsoner = new System.Web.Script.Serialization.JavaScriptSerializer();
            return jsoner.Deserialize<T>(jsonStr);
        }
    }
}
