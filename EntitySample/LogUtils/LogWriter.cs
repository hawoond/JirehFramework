using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityConnecter.LogUtils
{
    public class LogWriter
    {
        public const string LOG_ROOT = @"c:\ImrSol\Logs";
        public LogWriter()
        {

        }

        public async void WriteLogMessage(UdtLog logData)
        {
            //@"c:\ImrSol\PerfectDB.realm"

            try
            {

                // 파일 저장
                string sFileDirectory = LOG_ROOT + "//" + DateTime.Now.ToString("yyyy-MM-dd") + "//" + logData.IP_ADDRESS;

                if (!System.IO.Directory.Exists(sFileDirectory))
                {
                    System.IO.Directory.CreateDirectory(sFileDirectory);
                }
                string sFile = sFileDirectory + "//" + DateTime.Now.ToString("HH");
                //if(!File.Exists(sFile))
                //{
                //    File.CreateText(sFile);
                //}
                using (StreamWriter sw = new StreamWriter(sFile, true))
                {
                    try
                    {
                        string sLogText = logData.LOG_DTM;
                        sLogText += "    ";
                        sLogText += logData.IP_ADDRESS;
                        sLogText += "    ";
                        sLogText += logData.CALL_SERVICE;
                        sLogText += "    ";
                        sLogText += logData.LOG;
                        sLogText += "    ";
                        sLogText += logData.LOG_DATA;

                        await sw.WriteLineAsync(sLogText);
                        await sw.FlushAsync();
                    }
                    catch (Exception ex)
                    {
                        await sw.WriteLineAsync("Log 기록 실패");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return;

        }
    }
}
