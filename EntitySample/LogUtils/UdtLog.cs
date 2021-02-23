using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace EntityConnecter.LogUtils
{
    /// <summary>
    /// UdtLog의 요약 설명입니다.
    /// </summary>
    public class UdtLog
    {
        public UdtLog()
        {
        }

        public string IP_ADDRESS
        {
            get;
            set;
        }

        public string CALL_SERVICE
        {
            get;
            set;
        }

        public string LOG
        {
            get;
            set;
        }

        public string LOG_DATA
        {
            get;
            set;
        }

        public string LOG_DTM
        {
            get;
            set;
        }
    }
}
