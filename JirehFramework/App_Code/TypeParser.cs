using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

/// <summary>
/// TypeParser의 요약 설명입니다.
/// </summary>
public class TypeParser
{
    /// <summary>
    /// XML, JSON ,String 등 Input, OutPut 반환 타입을 조립 및 분석 함
    /// </summary>
    public TypeParser()
    {
        
    }

    /// <summary>
    /// xml을 분석하여 데이터 정리 후 리턴
    /// </summary>
    /// <param name="sXml"></param>
    /// <returns>기본 속성을 포함 한 형태의 클래스 지정 후 반환
    /// 기본 형태는 추후 작성</returns>
    public UdtServiceInfo XmlParser(string sXml)
    {
        UdtServiceInfo udtServiceInfo = new UdtServiceInfo();

        //0.xml에서 데이터 추출
        //0.udtServiceInfo에 데이터 넣기
        XElement xElement = XElement.Parse(sXml);

        udtServiceInfo.ServiceName = xElement.Element("ServiceName").Value;
        udtServiceInfo.InputType = xElement.Element("InputType").Value;
        udtServiceInfo.ReturnType = xElement.Element("ReturnType").Value;
        udtServiceInfo.ConnectName = xElement.Element("ConnectName").Value;
        udtServiceInfo.IpAddress = xElement.Element("IpAddress").Value;

        XElement paramElement = xElement.Element("DicParams");

        Dictionary<string, string> dicParmas = new Dictionary<string, string>();
        foreach (var item in paramElement.Elements())
        {
            dicParmas.Add(item.Name.ToString(), item.Value);
        }

        udtServiceInfo.DicParams = dicParmas;

        return udtServiceInfo;
    }

    public UdtServiceInfo JsonParser(string sJson)
    {
        UdtServiceInfo udtServiceInfo = new UdtServiceInfo();

        JObject jObject = JObject.Parse(sJson);

        udtServiceInfo.ServiceName = jObject["ServiceName"].ToString();
        udtServiceInfo.InputType = jObject["InputType"].ToString();
        udtServiceInfo.ReturnType = jObject["ReturnType"].ToString();
        udtServiceInfo.ConnectName = jObject["ConnectName"].ToString();
        udtServiceInfo.IpAddress = jObject["IpAddress"].ToString();

        JObject paramObject = (JObject)jObject["DicParams"];

        Dictionary<string, string> dicParmas = new Dictionary<string, string>();
        foreach (var item in paramObject)
        {
            dicParmas.Add(item.Key, item.Value.ToString());
        }

        udtServiceInfo.DicParams = dicParmas;

        return udtServiceInfo;
    }

    /// <summary>
    /// XML을 Dictionary로 변환
    /// </summary>
    /// <param name="sXml"></param>
    /// <returns></returns>
    public static Dictionary<string,string> XmlToDictionary(string sXml)
    {
        sXml = sXml.Replace("\n", "");
        XElement xElement = null;
        try
        {
            xElement = XElement.Parse(sXml);
        }
        catch(Exception ex)
        {
            sXml = sXml.Insert(sXml.Length, "</groot>");
            sXml = sXml.Insert(0, "<groot>");
            xElement = XElement.Parse(sXml);
        }
        Dictionary<string, string> dicResult = new Dictionary<string, string>();

        foreach (var item in xElement.Elements())
        {
            dicResult.Add(item.Name.ToString(), item.Value);
        }

        return dicResult;
    }

    /// <summary>
    /// Xml을 List로 변환
    /// </summary>
    /// <param name="sXml"></param>
    /// <returns></returns>
    public static List<string> XmlToList(string sXml)
    {
        //List<XElement> xElements = new List<XElement>();
        List<string> arrResult = new List<string>();
        XElement xElement = XElement.Parse(sXml);
       
        foreach (var item in xElement.Elements())
        {
            arrResult.Add(item.ToString());
        }

        return arrResult;
    }

    /// <summary>
    /// Json을 Dictionary로 변환
    /// </summary>
    /// <param name="sJson"></param>
    /// <returns></returns>
    public static Dictionary<string, string> JsonToDictionary(string sJson)
    {
        JObject jObject = JObject.Parse(sJson);
        Dictionary<string, string> dicResult = new Dictionary<string, string>();

        foreach (var item in jObject)
        {
            dicResult.Add(item.Key, item.Value.ToString());
        }

        return dicResult;
    }

    /// <summary>
    /// Json을 List로 변환
    /// </summary>
    /// <param name="sJson"></param>
    /// <returns></returns>
    public static List<string> JsonToList(string sJson)
    {
        JObject jObject = JObject.Parse(sJson);
        List<string> arrResult = new List<string>();

        foreach (var item in jObject)
        {
            arrResult.Add(item.Value.ToString());
        }

        return arrResult;
    }

    /// <summary>
    /// Dictionary를 Xml로 변환
    /// </summary>
    /// <param name="dicData"></param>
    /// <returns></returns>
    public static string DictionaryToXml(Dictionary<string,string> dicData)
    {
        XDocument xDocument = new XDocument(new XDeclaration("1.0", "UTF-8", null));
        XElement root = new XElement("groot");
        xDocument.Add(root);

        foreach (var item in dicData)
        {
            XElement xElement;
            try
            {
                xElement = new XElement(item.Key);
                xElement.Add(XElement.Parse(item.Value));
            }
            catch (Exception ex)
            {
                xElement = new XElement(item.Key, item.Value);
            }
            if (null != xElement)
            {
                root.Add(xElement);
            }
        }

        return xDocument.ToString().Replace("<groot>", "").Replace("</groot>", "");
    }

    /// <summary>
    /// Dictionary를 Json으로 변환
    /// </summary>
    /// <param name="sJson"></param>
    /// <returns></returns>
    public static string DictionaryToJson(Dictionary<string, string> dicData)
    {
        JObject jObject = new JObject();

        foreach (var item in dicData)
        {
            jObject.Add(item.Key, item.Value);
        }

        return jObject.ToString().Replace("\r\n","");
    }

    /// <summary>
    /// List를 Xml로 변환
    /// </summary>
    /// <param name="arrData"></param>
    /// <returns></returns>
    public static string ListToXml(List<string> arrData)
    {
        XDocument xDocument = new XDocument(new XDeclaration("1.0", "UTF-8", null));
        XElement root = new XElement("groot");
        xDocument.Add(root);

        for (int i=0; i<arrData.Count; ++i)
        {
            XElement xElement;

            try
            {
                xElement = new XElement("Data");
                xElement.Add(XElement.Parse(arrData[i]));
            }
            catch (Exception ex)
            {
                xElement = new XElement("Data", arrData[i]);
            }
            if (null != xElement)
            {
                root.Add(xElement);
            }
        }

        return xDocument.ToString().Replace("&lt;","<").Replace("&gt;", ">");
    }

    /// <summary>
    /// List를 Json으로 변환
    /// </summary>
    /// <param name="arrData"></param>
    /// <returns></returns>
    public static string ListToJson(List<string> arrData)
    {
        JObject jObject = new JObject();

        for (int i = 0; i < arrData.Count; ++i)
        {
            jObject.Add("Data", arrData[i]);
        }

        return jObject.ToString();
    }
}