using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityConnecter
{
    public class StaticUtils
    {
        public static Dictionary<string, string> GetDictionaryFieldValue(object oModel)
        {
            Dictionary<string, string> dicResult = new Dictionary<string, string>();

            for (int i = 0; i < oModel.GetType().GetFields().Length; i++)
            {
                dicResult.Add(oModel.GetType().GetFields()[i].ToString(), oModel.GetType().GetFields()[i].GetValue(oModel.GetType().GetFields()[i]).ToString());
            }

            return dicResult;
        }
    }
}
