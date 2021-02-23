using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EntityConnecter.Models
{
    [Serializable]
    public class PT_INFO
    {
        public int PT_NO
        {
            get;
            set;
        }
        public string PT_NM
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetDictionary()
        {
            Dictionary<string, string> dicResult = new Dictionary<string, string>();

            for (int i = 0; i < this.GetType().GetFields().Length; i++)
            {
                dicResult.Add(this.GetType().GetFields()[i].ToString(), this.GetType().GetFields()[i].GetValue(this.GetType().GetFields()[i]).ToString());
            }

            return dicResult;
        }
    }
}
