using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace EntityConnecter
{
    public class StaticUtils
    {
        public static Dictionary<string, string> GetDictionaryFieldValue(object oModel)
        {
            Dictionary<string, string> dicResult = new Dictionary<string, string>();

            for (int i = 0; i < oModel.GetType().GetProperties().Length; i++)
            {
                string sPropertyName = oModel.GetType().GetProperties()[i].Name;
                dicResult.Add(sPropertyName, oModel.GetType().GetProperty(sPropertyName).GetValue(oModel).ToString());
            }

            return dicResult;
        }

        /// <summary>
        /// XML을 Dictionary로 변환
        /// </summary>
        /// <param name="sXml"></param>
        /// <returns></returns>
        public static Dictionary<string, string> XmlToDictionary(string sXml)
        {
            XElement xElement = XElement.Parse(sXml);
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
            XElement xElement = XElement.Parse(sXml);
            List<string> arrResult = new List<string>();

            foreach (var item in xElement.Elements())
            {
                arrResult.Add(item.Value);
            }

            return arrResult;
        }

        ///// <summary>
        ///// Json을 Dictionary로 변환
        ///// </summary>
        ///// <param name="sJson"></param>
        ///// <returns></returns>
        //public Dictionary<string, string> JsonToDictionary(string sJson)
        //{
        //    JObject jObject = JObject.Parse(sJson);
        //    Dictionary<string, string> dicResult = new Dictionary<string, string>();

        //    foreach (var item in jObject)
        //    {
        //        dicResult.Add(item.Key, item.Value.ToString());
        //    }

        //    return dicResult;
        //}

        /// <summary>
        /// Dictionary를 Xml로 변환
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        public static string DictionaryToXml(Dictionary<string, string> dicData)
        {
            XDocument xDocument = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            XElement root = new XElement("root");
            xDocument.Add(root);

            foreach (var item in dicData)
            {
                XElement xElement = new XElement(item.Key, item.Value);
                root.Add(xElement);
            }

            return xDocument.ToString();
        }

        /// <summary>
        /// List를 Xml로 변환
        /// </summary>
        /// <param name="arrData"></param>
        /// <returns></returns>
        public static string ListToXml(List<string> arrData)
        {
            XDocument xDocument = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            XElement root = new XElement("root");
            xDocument.Add(root);

            for (int i = 0; i < arrData.Count; ++i)
            {
                XElement xElement = new XElement("Data" + i, arrData[i]);
                root.Add(xElement);
            }

            return xDocument.ToString();
        }
    }
}
