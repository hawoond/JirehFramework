using EntityConnecter;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Web;

// 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드, svc 및 config 파일에서 클래스 이름  "Service"를 변경할 수 있습니다.
public class JirehFramework : IJirehFramework
{
    Encryption encryption = new Encryption();
    TypeParser typeParser = new TypeParser();

    EntityConnecter.DbService dbService = new EntityConnecter.DbService();

    public string CallService(string sInputMessage)
    {
        string sResult = string.Empty;
        // 수신 받은 메세지 복호화
        string sDecryptMessage = encryption.AESDecrypt256(sInputMessage, Encryption.KEY);

        if (sDecryptMessage.Trim().StartsWith("{"))
        {
            sResult = JsonComm(sDecryptMessage);
        }
        else if (sDecryptMessage.Trim().StartsWith("<"))
        {
            sResult = XmlComm(sDecryptMessage);
        }
        else
        {
            return Resources.Resource.Error_406 + "\nInputType 형식이 맞지 않습니다.";
        }

        sResult = encryption.AESEncrypt256(sResult, Encryption.KEY);

        return sResult;
    }


    private string JsonComm(string sInputJson)
    {
        string sResult = string.Empty;

        // 0. sInputJson 분석 후 데이터 정리(서비스 명, 파라미터 등)
        UdtServiceInfo udtServiceInfo = typeParser.JsonParser(sInputJson);
        // return type 정리
        // 1. DB or Service 통신 후 데이터 획득 하여 arrData에 저장

        // DB 통신 테스트
        EntityConnecter.DbService dbService = new EntityConnecter.DbService();
        List<Dictionary<string, string>> dicResult = new List<Dictionary<string, string>>();
        List<string> arrTempResult = TypeParser.XmlToList(ConnectDll(udtServiceInfo));

        for (int i = 0; i < arrTempResult.Count; i++)
        {
            dicResult.Add(TypeParser.XmlToDictionary(arrTempResult[i]));
        }

        // 2. 데이터 활용 해서 ConvertMessageType 함수로 메세지 타입에(XML or Json) 맞춰 조립
        // 0 : XML, 1: JSON, 2 : String
        sResult = ConvertMessageType(udtServiceInfo, dicResult);

        return sResult;
    }

    /// <summary>
    /// Input Type이 XML일때
    /// </summary>
    /// <param name="sInputXml">수신 받은 XML 데이터</param>
    /// <param name="nReturnType">반환 타입 0 : XML, 1: JSON, 2 : String (default : XML)</param>
    /// <returns></returns>
    private string XmlComm(string sInputXml)
    {
        string sResult = string.Empty;

        // 0. sInputXml 분석 후 데이터 정리(서비스 명, 파라미터 등)
        UdtServiceInfo udtServiceInfo = typeParser.XmlParser(sInputXml);

        // 1. DB or Service 통신 후 데이터 획득 하여 arrData에 저장
        // DB 통신 테스트
        EntityConnecter.DbService dbService = new EntityConnecter.DbService();
        List<Dictionary<string, string>> dicResult = new List<Dictionary<string, string>>();
        List<string> arrTempResult = TypeParser.XmlToList(ConnectDll(udtServiceInfo));

        for (int i = 0; i < arrTempResult.Count; i++)
        {
            dicResult.Add(TypeParser.XmlToDictionary(arrTempResult[i]));
        }
        // 2. 데이터 활용 해서 ConvertMessageType 함수로 메세지 타입에(XML or Json) 맞춰 조립
        // 0 : XML, 1: JSON, 2 : String
        sResult = ConvertMessageType(udtServiceInfo, dicResult);

        // 왜 나는 솔루션 탐색기가 안보이지
        return sResult;
    }

    /// <summary>
    /// 나중에
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public string StringComm(string value)
    {
        return "";
    }

    /// <summary>
    /// 묶음 호출 시 불리는 함수(XML)
    /// </summary>
    /// <param name="arrInputXml"></param>
    /// <param name="nReturnType"></param>
    /// <returns></returns>
    private List<string> ArrXmlComm(List<string> arrInputXml, int nReturnType)
    {
        List<string> arrResult = new List<string>();

        for (int i = 0; i < arrInputXml.Count; i++)
        {
            XmlComm(arrInputXml[i]);
        }

        return arrResult;
    }

    /// <summary>
    /// 묶음 호출 시 불리는 함수 (JSON)
    /// </summary>
    /// <param name="arrInputJson"></param>
    /// <param name="nReturnType"></param>
    /// <returns></returns>
    private List<string> ArrJsonComm(List<string> arrInputJson, int nReturnType)
    {
        List<string> arrResult = new List<string>();

        for (int i = 0; i < arrInputJson.Count; i++)
        {
            JsonComm(arrInputJson[i]);
        }

        return arrResult;
    }

    private string ConnectDll(UdtServiceInfo serviceInfo)
    {
        // dll 에서 호출 필요한 데이터들 줌
        // 함수명은 뭔지 모름
        return dbService.CallQuery(serviceInfo.ServiceName, serviceInfo.IpAddress, serviceInfo.DicParams);
    }

    /// <summary>
    /// Return Type, Paramater 받아서 Xml, Json, String으로 결과값 반환
    /// </summary>
    /// <param name="serviceInfo"></param>
    /// <returns>결과값</returns>
    private string ConvertMessageType(UdtServiceInfo serviceInfo, List<Dictionary<string, string>> arrDicResult)
    {
        string sResult = "지원하지 않는 반환 형식입니다.";
        List<string> arrResult = new List<string>();
        if (serviceInfo.ReturnType.Equals("JSON"))
        {
            for (int i = 0; i < arrDicResult.Count; i++)
            {
                arrResult.Add(TypeParser.DictionaryToJson(arrDicResult[i]));
            }

            sResult = TypeParser.ListToJson(arrResult);
        }
        else if (serviceInfo.ReturnType.Equals("XML"))
        {
            for (int i = 0; i < arrDicResult.Count; i++)
            {
                arrResult.Add(TypeParser.DictionaryToXml(arrDicResult[i]));
            }

            sResult = TypeParser.ListToXml(arrResult);
        }
        else
        {
            sResult = "지원하지 않는 반환 형식입니다.";
        }



        return sResult;
    }


    public CompositeType GetDataUsingDataContract(CompositeType composite)
    {
        if (composite == null)
        {
            throw new ArgumentNullException("composite");
        }
        if (composite.BoolValue)
        {
            composite.StringValue += "Suffix";
        }
        return composite;
    }

    /// <summary>
    /// FTP 파일 업로드
    /// </summary>
    /// <param name="uUploadUri"></param>
    /// <param name="sPort"></param>
    /// <param name="sUserID"></param>
    /// <param name="sUserPassword"></param>
    /// <param name="sFilename"></param>
    /// <param name="stFileStream"></param>
    /// <returns></returns>
    public bool FileUpload(Uri uUploadUri, string sPort, string sUserID, string sUserPassword, string sFilename, Stream stFileStream)
    {
        try
        {
            if (stFileStream.Position > 0)
            {
                if (sFilename != "" && sFilename != null)
                {
                    var reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(uUploadUri + ":" + sPort + "/" + sFilename));
                    reqFTP.UsePassive = true;
                    reqFTP.UseBinary = true;
                    reqFTP.Credentials = new NetworkCredential(sUserID, sUserPassword);

                    reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

                    int bufferLength = 2048;
                    byte[] buffer = new byte[bufferLength];

                    Stream uploadStream = reqFTP.GetRequestStream();
                    int contentLength = stFileStream.Read(buffer, 0, bufferLength);

                    while (contentLength != 0)
                    {
                        uploadStream.Write(buffer, 0, bufferLength);
                        contentLength = stFileStream.Read(buffer, 0, bufferLength);
                    }
                    //성공로그자리
                }
            }
            else
            {
                //로구ㅡ자리statusMessage = "File is not uploaded..";
                return false;
            }
        }
        catch (System.IO.IOException ex)
        {
            //실패로그자리 statusMessage = ex.Message;
            return false;
        }

        return true;
    }

    /// <summary>
    /// FTP 파일 다운로드
    /// </summary>
    /// <param name="uDownloadUri"></param>
    /// <param name="sPort"></param>
    /// <param name="sUserID"></param>
    /// <param name="sUserPassword"></param>
    /// <param name="sFilename"></param>
    /// <returns></returns>
    public string FileDownload(string sDownloadUri, string sPort, string sUserID, string sUserPassword, string sFilename)
    {
        FileStream streamResult = null;

        try
        {
            var reqFTP = (FtpWebRequest)WebRequest.Create(new Uri("ftp://" + sDownloadUri + ":" + sPort + "/" + sFilename));
            reqFTP.UsePassive = true;
            reqFTP.UseBinary = true;
            reqFTP.Credentials = new NetworkCredential(sUserID, sUserPassword);

            reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;

            // FTP Request 결과를 가져온다.
            using (FtpWebResponse resp = (FtpWebResponse)reqFTP.GetResponse())
            {
                // FTP 결과 스트림
                streamResult = resp.GetResponseStream() as FileStream;
            }
        }
        catch (System.IO.IOException ex)
        {
            //실패로그자리 statusMessage = ex.Message;
            return null;
        }

        byte[] bytes = new byte[streamResult.Length];
        streamResult.Position = 0;
        streamResult.Read(bytes, 0, (int)streamResult.Length);

        string sResult = Encoding.ASCII.GetString(bytes);

        return encryption.AESEncrypt256(sResult, Encryption.KEY);
    }

    public string HttpDownload(string FileKind, string FilePath)
    {
        string sRoot = @"D:\FTP\Platform\";
        // 폴더 권한 획득
        GrantAccess(sRoot + FileKind + @"\" + FilePath);
        //FileStream fsResult = File.OpenRead(sRoot + FileKind + @"\" + FilePath);
        //string sResult = Encoding.ASCII.GetString(File.ReadAllBytes(sRoot+FilePath));
        //return encryption.AESEncrypt256(sResult, Encryption.KEY);
        byte[] bytes = StaticUtils.CompressFolder(sRoot + FileKind + @"\" + FilePath, FilePath);
        //Convert.ToBase64String(bytes);
        //string sResult = Encoding.Default.GetString(bytes);
        //sResult = Convert.ToBase64String(bytes, 0, bytes.Length);


        string sResult = string.Empty;
        byte[] tempByte = new byte[3];
        for (int i = 0; i < bytes.Length; i++)
        {
            if (i > 2)
            {
                tempByte.SetValue(bytes[i], i % 3);
            }
            else
            {
                tempByte.SetValue(bytes[i], i);
            }

            if ((i+1) % 3 == 0)
            {
                sResult += Convert.ToBase64String(tempByte);
                tempByte = new byte[3];
            }
            else if (bytes.Length - (i + 1) <= 2)
            {
                sResult += Convert.ToBase64String(tempByte);
                tempByte = new byte[3];
            }
            else
            {
                continue;
            }
        }
        //sResult = Convert.ToBase64String(bytes,0, bytes.Length);

        return sResult;
    }

    public bool GrantAccess(string fullPath)
    {
        DirectoryInfo dInfo = new DirectoryInfo(fullPath);
        DirectorySecurity dSecurity = dInfo.GetAccessControl();
        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
        dInfo.SetAccessControl(dSecurity);
        return true;
    }
}
