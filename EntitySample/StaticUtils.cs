using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
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
                var itemValue = oModel.GetType().GetProperty(sPropertyName).GetValue(oModel);

                if (itemValue != null)
                {
                    dicResult.Add(sPropertyName, itemValue.ToString());
                }
                else
                {
                    dicResult.Add(sPropertyName, null);
                }
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
            //List<XElement> xElements = new List<XElement>();
            List<string> arrResult = new List<string>();
            XElement xElement = XElement.Parse(sXml);

            foreach (var item in xElement.Elements())
            {
                arrResult.Add(item.ToString());
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

            return xDocument.ToString().Replace("<groot>", "").Replace("</groot>", "").Replace("&lt;", "<").Replace("&gt;", ">");
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

            for (int i = 0; i < arrData.Count; ++i)
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

            return xDocument.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
        }

        /// <summary>
        /// Image를 Byte array로
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public static byte[] ImageToByteArray(string sUri)
        {
            using (var stream = new MemoryStream())
            {
                Image image = Image.FromFile(sUri);

                image.Save(stream, image.RawFormat);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// 폴더 압축해서 byte array로 리턴
        /// </summary>
        /// <param name="sPath"></param>
        /// <returns></returns>
        public static byte[] CompressFolder(string sPath)
        {
            string zipPath = System.IO.Path.GetTempPath() + @"\tempPrevImg.zip";
            try
            {
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }

                ZipFile.CreateFromDirectory(sPath, zipPath);
                return File.ReadAllBytes(zipPath);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static byte[] CompressFolder(string sPath, string sTempFileName)
        {
            // 압축 결과물 Path
            string zipPath = System.IO.Path.GetTempPath() + sTempFileName + ".zip";
            try
            {
                // 압축 파일 생성 할 부분에 파일 있는지 검사
                if (File.Exists(zipPath))
                {
                    // 있으면 지운다
                    File.Delete(zipPath);
                }
                // 없으니까 만들자
                ZipFile.CreateFromDirectory(sPath, zipPath);
                // 압축 할 디렉토리
                return File.ReadAllBytes(zipPath);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
