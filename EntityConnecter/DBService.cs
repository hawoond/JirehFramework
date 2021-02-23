using EntityConnecter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace EntityConnecter
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드 및 config 파일에서 클래스 이름 "Service1"을 변경할 수 있습니다.
    public class DbService : IDbService
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public List<Dictionary<string, string>> TestConnect()
        {
            List<Dictionary<string, string>> arrResult = new List<Dictionary<string, string>>();

            using (imrEntities entities = new imrEntities())
            {
                var ptInfo = from b in entities.PatientInfo
                             orderby b.PT_NO
                             select b;

                foreach (var item in ptInfo)
                {
                    //뭔가 정렬 굉장히 맘에 안드는데..
                    arrResult.Add(StaticUtils.GetDictionaryFieldValue(
                                                                       StaticUtils.GetDictionaryFieldValue(
                                                                                                             new PatientInfo()
                                                                                                             {
                                                                                                                 PT_NM = item.PT_NM,
                                                                                                                 PT_NO = item.PT_NO
                                                                                                             }
                                                                                                           )
                                                                     )
                                 );
                }
            }

            return arrResult;
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
    }
}
