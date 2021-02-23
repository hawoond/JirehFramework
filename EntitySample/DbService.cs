using EntityConnecter.LogUtils;
using EntityConnecter.Models;
using EntityConnecter.Models.DB;
using EntityConnecter.Models.DB.SELECT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static EntityConnecter.StaticUtils;

namespace EntityConnecter
{
    public class DbService
    {
        public string CallQuery(string sServiceName, string sIpAddress, Dictionary<string, string> inputParams)
        {
            string ServiceName = Properties.Resources.ResourceManager.GetString(sServiceName);

            string sResult = string.Empty;
            Type type = this.GetType();
            MethodInfo method = type.GetMethod(ServiceName);

            if (method != null)
            {
                if (inputParams.Count == 0)
                {
                    sResult = method.Invoke(this, null).ToString();
                }
                else
                {
                    var arguments = method.GetParameters()
                                          .Select(p => inputParams[p.Name]).ToArray();
                    sResult = method.Invoke(this, arguments).ToString();
                }
            }

            UdtLog udtLog = new UdtLog();
            udtLog.CALL_SERVICE = ServiceName;
            udtLog.IP_ADDRESS = sIpAddress;
            udtLog.LOG_DTM = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            udtLog.LOG = "CallQuery";
            udtLog.LOG_DATA = sResult;

            LogWriter logWriter = new LogWriter();
            logWriter.WriteLogMessage(udtLog);

            return sResult;
        }

        public string TestConnect()
        {
            List<string> arrResult = new List<string>();
            // List<Dictionary<string, string>> arrResult2 = new List<Dictionary<string, string>>();

            using (imrEntities entities = new imrEntities())
            {
                var ptInfo = from b in entities.PatientInfo
                             orderby b.PT_NO
                             select b;

                foreach (var item in ptInfo)
                {
                    //뭔가 정렬 굉장히 맘에 안드는데..
                    arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new PT_INFO()
                                                                        {
                                                                            PT_NM = item.PT_NM,
                                                                            PT_NO = int.Parse(item.PT_NO)
                                                                        }
                                                                     ))
                                 );
                }
            }
            //arrResult.ToArray();
            return StaticUtils.ListToXml(arrResult);
        }

        /// <summary>
        /// 사용자 로그인
        /// </summary>
        /// <param name="sUsrId"></param>
        /// <param name="sUsrPw"></param>
        /// <returns></returns>
        public string S_CO_USR_LGN_SIN(string USER_NO, string USER_PW)
        {
            List<string> arrResult = new List<string>();
            // List<Dictionary<string, string>> arrResult2 = new List<Dictionary<string, string>>();

            using (imrEntities entities = new imrEntities())
            {
                var userInfo = from b in entities.OTUSINFO
                               where b.USER_NO == USER_NO
                               where b.USER_PW == USER_PW
                               where b.USE_YN == "Y"
                               select b;

                foreach (var item in userInfo)
                {
                    //뭔가 정렬 굉장히 맘에 안드는데..
                    arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new S_CO_USR_LGN_SIN()
                                                                        {
                                                                            USER_NM = item.USER_NM,
                                                                            USER_NO = item.USER_NO,
                                                                            USER_NICK = item.USER_NICK_NM,
                                                                            USER_BIRTH = item.USER_BIRTH.ToString(),
                                                                            USER_GROUP = item.USER_GROUP,
                                                                            USER_MAIL_ADDRESS = item.USER_MAIL_ADDRESS,
                                                                            USER_PHONE = item.USER_PHONE,
                                                                            SUCCESS_DOC = "성공",
                                                                            SUCCESS_YN = "Y"
                                                                        }
                                                                     ))
                                 );
                }

                if (arrResult.Count != 1)
                {
                    arrResult.Clear();
                    arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new S_CO_USR_LGN_SIN()
                                                                        {
                                                                            USER_NM = "",
                                                                            USER_NO = "",
                                                                            USER_NICK = "",
                                                                            USER_BIRTH = "",
                                                                            USER_GROUP = "",
                                                                            USER_MAIL_ADDRESS = "",
                                                                            USER_PHONE = "",
                                                                            SUCCESS_DOC = "실패",
                                                                            SUCCESS_YN = "N"
                                                                        }
                                                                     ))
                                 );
                }
            }
            //arrResult.ToArray();
            return StaticUtils.ListToXml(arrResult);
        }


        /// <summary>
        /// 찜 목록 데이터 삭제
        /// </summary>
        /// <param name="sContentID"></param>
        /// <param name="sUserID"></param>
        /// <returns></returns>
        public string D_MA_USR_FAV_SIN(string CONS_ID, string USER_NO)
        {
            List<string> arrResult = new List<string>();

            using (imrEntities entities = new imrEntities())
            {
                try
                {
                    var DeleteData = entities.OTUSMYLT.Where(a => a.CONS_ID == CONS_ID && a.USER_NO == USER_NO).SingleOrDefault();

                    if (DeleteData != null)
                    {
                        entities.OTUSMYLT.Remove(DeleteData);
                        entities.SaveChanges();
                        arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new ErrorReturn()
                                                                        {
                                                                            ERROR_YN = "N"
                                                                          ,
                                                                            ERROR_DOC = "성공"
                                                                        }
                                                                     )));
                    }
                    else
                    {
                        arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new ErrorReturn()
                                                                        {
                                                                            ERROR_YN = "Y"
                                                                          ,
                                                                            ERROR_DOC = "삭제할 데이터가 없습니다."
                                                                        }
                                                                     )));
                    }
                }
                catch (Exception ex)
                {
                    arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new ErrorReturn()
                                                                        {
                                                                            ERROR_YN = "Y"
                                                                          ,
                                                                            ERROR_DOC = "데이터 삭제에 실패하였습니다. err message: " + ex.Message
                                                                        }
                                                                     )));
                }
            }

            return StaticUtils.ListToXml(arrResult);
        }

        /// <summary>
        /// 리뷰 데이터 삭제(업데이트)
        /// </summary>
        /// <param name="sContentID"></param>
        /// <param name="sReviewSeq"></param>
        /// <param name="sUserID"></param>
        /// <returns></returns>
        public string U_MA_USR_REV_SIN(string CONS_ID, string REV_SEQ, string USER_NO)
        {
            List<string> arrResult = new List<string>();

            using (imrEntities entities = new imrEntities())
            {
                try
                {
                    int reviewSeq = int.Parse(REV_SEQ);
                    var UpdateData = entities.OTCSREVW.Where(a => a.CONS_ID == CONS_ID
                                                               && a.REV_SEQ == reviewSeq
                                                               && a.USER_NO == USER_NO).SingleOrDefault();

                    if (UpdateData != null)
                    {
                        UpdateData.USE_YN = "N";
                        entities.SaveChanges();
                        arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new ErrorReturn()
                                                                        {
                                                                            ERROR_YN = "N"
                                                                          ,
                                                                            ERROR_DOC = "성공"
                                                                        }
                                                                     )));
                    }
                    else
                    {
                        arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new ErrorReturn()
                                                                        {
                                                                            ERROR_YN = "N"
                                                                          ,
                                                                            ERROR_DOC = "업데이트할 데이터가 없습니다."
                                                                        }
                                                                     )));
                    }
                }
                catch (Exception ex)
                {
                    arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new ErrorReturn()
                                                                        {
                                                                            ERROR_YN = "N"
                                                                          ,
                                                                            ERROR_DOC = "데이터 업데이트에 실패하였습니다. err message: " + ex.Message
                                                                        }
                                                                     )));
                }
            }

            return StaticUtils.ListToXml(arrResult);
        }
        /// <summary>
        /// 컨텐츠 리스트 조회, 검색
        /// </summary>
        /// <param name="PAGE_NO">페이징 숫자</param>
        /// <param name="SEARCH_WORD">검색 할 컨텐츠 이름, 빈값 : 전체 조회</param>
        /// <param name="CATEGORY">검색 할 컨텐츠 카테고리, 빈값 : 전체 조회</param>
        /// <returns></returns>
        public string S_MA_CTS_SRCH_LIST(string PAGE_NO, string SEARCH_WORD, string CATEGORY)
        {
            int nPageNo = int.Parse(PAGE_NO);
            List<string> arrResult = new List<string>();

            // 페이징 최대 조회 갯수
            int nMaxPage = 30;


            using (imrEntities entities = new imrEntities())
            {
                var downloadRate = from download in entities.OTUSCSLT
                                   from b in entities.OTCSINFO
                                   where b.CONS_NM.Contains(SEARCH_WORD)
                                      && download.CONS_ID == b.CONS_ID
                                   select download;



                var starRate = from star in entities.OTCSREVW
                               from b in entities.OTCSINFO
                               where b.CONS_NM.Contains(SEARCH_WORD)
                                  && star.CONS_ID == b.CONS_ID
                               select star;
                int starSum = 0;
                foreach (var starItem in starRate)
                {
                    if (null != starItem.CONS_ST)
                    {
                        starSum += int.Parse(starItem.CONS_ST);
                    }
                }

                int downCount = downloadRate.Count<OTUSCSLT>();
                int starAvg = 0;
                if (starRate.Count<OTCSREVW>() > 0)
                {
                    starAvg = starSum / starRate.Count<OTCSREVW>();

                }
                else
                {
                    starAvg = 0;
                }

                var contentsInfo = from b in entities.OTCSINFO
                                   from c in entities.OTADCOPY
                                   from master in entities.MTCOTPCD
                                   from master2 in entities.MTCOTPCD
                                   from f in entities.OTCSSCRE
                                   where b.USE_YN == "Y"
                                      && master.USE_YN == b.USE_YN
                                      && master.CO_CD_DTL == b.CONS_TP_CD
                                      && master2.USE_YN == b.USE_YN
                                      && master2.CO_CD_DTL == b.CONS_CT_CD
                                      && b.CONS_COPY_CD == c.CONS_COPY_CD
                                      && b.CONS_ID == f.CONS_ID
                                      && b.CONS_CT_CD.Contains(CATEGORY)
                                      && b.CONS_NM.Contains(SEARCH_WORD)
                                   orderby b.CONS_SEQ descending
                                   select new
                                   {
                                       CONS_ID = b.CONS_ID,
                                       CONS_TP = master.CO_CD_DOC,
                                       CONS_CT = master2.CO_CD_DOC,
                                       CONS_DOC = b.CONS_DOC,
                                       CONS_NM = b.CONS_NM,
                                       COPY_NM = c.COPY_NM,
                                       CONS_PRICE = b.CON_PRICE,
                                       STAR_RATE = starAvg.ToString(),
                                       DOWN_RATE = downCount.ToString(),
                                       CONS_THUMB_PATH = f.CONS_THUMB
                                   };


                var result = contentsInfo.Skip((nPageNo - 1) * nMaxPage).Take(nMaxPage);

                foreach (var item in result)
                {
                    //뭔가 정렬 굉장히 맘에 안드는데..
                    arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new S_MA_CTS_SRCH_LIST()
                                                                        {
                                                                            CONS_ID = item.CONS_ID,
                                                                            CONS_NM = item.CONS_NM,
                                                                            CONS_DOC = item.CONS_DOC,
                                                                            CONS_CT = item.CONS_CT,
                                                                            CONS_PRICE = item.CONS_PRICE.ToString(),
                                                                            CONS_THUMB_PATH = Convert.ToBase64String(ImageToByteArray(item.CONS_THUMB_PATH)),
                                                                            CONS_TP = item.CONS_TP,
                                                                            COPY_NM = item.COPY_NM,
                                                                            DOWN_RATE = item.DOWN_RATE,
                                                                            STAR_RATE = item.STAR_RATE
                                                                        }
                                                                     ))
                                 );
                }
            }
            //arrResult.ToArray();
            return StaticUtils.ListToXml(arrResult);
        }

        /// <summary>
        /// 리뷰 입력 작업중
        /// </summary>
        /// <param name="USER_NO"></param>
        /// <param name="CONS_ID"></param>
        /// <param name="REV_DOC"></param>
        /// <param name="CONS_ST"></param>
        /// <returns></returns>
        public string I_MA_USR_REV_SIN(string USER_NO, string CONS_ID, string REV_DOC, string CONS_ST)
        {
            List<string> arrResult = new List<string>();

            using (imrEntities entities = new imrEntities())
            {
                try
                {
                    OTCSREVW reviewData = new OTCSREVW
                    {
                        USER_NO = USER_NO,
                        CONS_ID = CONS_ID,
                        REV_DOC = REV_DOC,
                        CONS_ST = CONS_ST,
                        REV_DTM = DateTime.Now,
                        CONS_SEQ = entities.OTCSINFO.Where(a => a.CONS_ID == CONS_ID && a.USE_YN == "Y").SingleOrDefault().CONS_SEQ,
                        REV_SEQ = entities.OTCSREVW.Where(a => a.CONS_ID == CONS_ID && a.USE_YN == "Y").Count() + 1,
                        USE_YN = "Y"
                    };

                    entities.OTCSREVW.Add(reviewData);
                    entities.SaveChanges();

                    arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new ErrorReturn()
                                                                        {
                                                                            ERROR_YN = "N",
                                                                            ERROR_DOC = "성공"
                                                                        }
                                                                     )));
                }
                catch (Exception ex)
                {
                    arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                           new ErrorReturn()
                                                                           {
                                                                               ERROR_YN = "Y",
                                                                               ERROR_DOC = "데이터 입력에 실패하였습니다. err message: " + ex.Message
                                                                           }
                                                                        )));
                }
            }

            return StaticUtils.ListToXml(arrResult);
        }

        /// <summary>
        /// 찜 목록 데이터 입력
        /// </summary>
        /// <param name="USER_NO"></param>
        /// <param name="CONS_ID"></param>
        /// <returns></returns>
        public string I_MA_USR_FAV_SIN(string USER_NO, string CONS_ID)
        {
            List<string> arrResult = new List<string>();

            using (imrEntities entities = new imrEntities())
            {
                try
                {
                    OTUSMYLT myListData = new OTUSMYLT
                    {
                        USER_NO = USER_NO,
                        CONS_ID = CONS_ID,
                        ENT_DTM = DateTime.Now,
                        SEQ = entities.OTUSMYLT.Count()
                    };

                    entities.OTUSMYLT.Add(myListData);
                    entities.SaveChanges();

                    arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new ErrorReturn()
                                                                        {
                                                                            ERROR_YN = "N",
                                                                            ERROR_DOC = "성공"
                                                                        }
                                                                     )));
                }
                catch (Exception ex)
                {
                    arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                           new ErrorReturn()
                                                                           {
                                                                               ERROR_YN = "Y",
                                                                               ERROR_DOC = "데이터 입력에 실패하였습니다. err message: " + ex.Message
                                                                           }
                                                                        )));
                }
            }

            return StaticUtils.ListToXml(arrResult);
        }

        /// <summary>
        /// 구매내역 리스트 입력
        /// </summary>
        /// <param name="USER_NO"></param>
        /// <param name="CONS_ID"></param>
        /// <returns></returns>
        public string I_MA_USR_PRCH_SIN(string USER_NO, string CONS_ID)
        {
            List<string> arrResult = new List<string>();

            using (imrEntities entities = new imrEntities())
            {
                try
                {
                    OTUSCSLT purchaseData = new OTUSCSLT
                    {
                        USER_NO = USER_NO,
                        CONS_ID = CONS_ID,
                        CONS_BUY_DTM = DateTime.Now,
                        CONS_SEQ = entities.OTUSCSLT.Count()
                    };

                    entities.OTUSCSLT.Add(purchaseData);
                    entities.SaveChanges();

                    arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new ErrorReturn()
                                                                        {
                                                                            ERROR_YN = "N"
                                                                          ,
                                                                            ERROR_DOC = "성공"
                                                                        }
                                                                     )));
                }
                catch (Exception ex)
                {
                    arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                           new ErrorReturn()
                                                                           {
                                                                               ERROR_YN = "Y"
                                                                             ,
                                                                               ERROR_DOC = "데이터 입력에 실패하였습니다. err message: " + ex.Message
                                                                           }
                                                                        )));
                }
            }

            return StaticUtils.ListToXml(arrResult);
        }

        /// <summary>
        /// 찜 목록 조회
        /// </summary>
        /// <param name="USER_NO"></param>
        /// <returns></returns>
        public string S_MA_USR_FAV_LIST(string USER_NO)
        {
            List<string> arrResult = new List<string>();
            // List<Dictionary<string, string>> arrResult2 = new List<Dictionary<string, string>>();

            using (imrEntities entities = new imrEntities())
            {
                var favInfo = from b in entities.OTUSMYLT
                              from c in entities.OTCSINFO

                              where b.USER_NO == USER_NO
                                 && b.CONS_ID == c.CONS_ID

                              select new
                              {
                                  CONS_ID = b.CONS_ID,
                                  CONS_NM = c.CONS_NM,
                                  CONS_THUMB_PATH = entities.OTCSSCRE.Where(a => a.CONS_ID == b.CONS_ID).FirstOrDefault().CONS_THUMB,
                                  CONS_TP = entities.MTCOTPCD.Where(a => a.CO_CD_DTL == c.CONS_TP_CD && a.USE_YN == "Y").FirstOrDefault().CO_CD_DOC,
                                  FAV_DTM = b.ENT_DTM
                              };

                foreach (var item in favInfo)
                {
                    //뭔가 정렬 굉장히 맘에 안드는데..
                    arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new S_MA_USR_FAV_LIST()
                                                                        {
                                                                            CONS_ID = item.CONS_ID,
                                                                            CONS_NM = item.CONS_NM,
                                                                            CONS_THUMB_PATH = Convert.ToBase64String(ImageToByteArray(item.CONS_THUMB_PATH)),
                                                                            CONS_TP = item.CONS_TP,
                                                                            FAV_DTM = item.FAV_DTM.ToString("yyyyMMdd HH:mm:ss")
                                                                        }
                                                                     ))
                                 );
                }
            }

            return StaticUtils.ListToXml(arrResult);
        }

        /// <summary>
        /// 구매 내역 리스트 조회
        /// </summary>
        /// <param name="USER_NO"></param>
        /// <returns></returns>
        public string S_MA_USR_PRCH_LIST(string USER_NO)
        {
            List<string> arrResult = new List<string>();
            // List<Dictionary<string, string>> arrResult2 = new List<Dictionary<string, string>>();

            using (imrEntities entities = new imrEntities())
            {
                var prcInfo = from b in entities.OTUSCSLT
                              from c in entities.OTCSINFO

                              where b.USER_NO == USER_NO
                                 && b.CONS_ID == c.CONS_ID

                              select new
                              {
                                  CONS_ID = b.CONS_ID,
                                  CONS_NM = c.CONS_NM,
                                  CONS_THUMB_PATH = entities.OTCSSCRE.Where(a => a.CONS_ID == b.CONS_ID).FirstOrDefault().CONS_THUMB,
                                  CONS_TP = entities.MTCOTPCD.Where(a => a.CO_CD_DTL == c.CONS_TP_CD && a.USE_YN == "Y").FirstOrDefault().CO_CD_DOC,
                                  PRCH_DTM = b.CONS_BUY_DTM
                              };

                foreach (var item in prcInfo)
                {
                    //뭔가 정렬 굉장히 맘에 안드는데..
                    arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new S_MA_USR_PRCH_LIST()
                                                                        {
                                                                            CONS_ID = item.CONS_ID,
                                                                            CONS_NM = item.CONS_NM,
                                                                            CONS_THUMB_PATH = Convert.ToBase64String(ImageToByteArray(item.CONS_THUMB_PATH)),
                                                                            CONS_TP = item.CONS_TP,
                                                                            PRCH_DTM = item.PRCH_DTM.ToString("yyyyMMdd HH:mm:ss")
                                                                        }
                                                                     ))
                                 );
                }
            }

            return StaticUtils.ListToXml(arrResult);
        }

        /// <summary>
        /// 리뷰 리스트 조회
        /// </summary>
        /// <param name="USER_NO"></param>
        /// <returns></returns>
        public string S_MA_USR_REV_LIST(string USER_NO)
        {
            List<string> arrResult = new List<string>();
            // List<Dictionary<string, string>> arrResult2 = new List<Dictionary<string, string>>();

            using (imrEntities entities = new imrEntities())
            {
                var revInfo = from b in entities.OTCSREVW
                              from c in entities.OTCSINFO

                              where b.USER_NO == USER_NO
                                 && b.CONS_ID == c.CONS_ID

                              select new
                              {
                                  CONS_ID = b.CONS_ID,
                                  CONS_NM = c.CONS_NM,
                                  CONS_THUMB_PATH = entities.OTCSSCRE.Where(a => a.CONS_ID == b.CONS_ID).FirstOrDefault().CONS_THUMB,
                                  CONS_TP = entities.MTCOTPCD.Where(a => a.CO_CD_DTL == c.CONS_TP_CD && a.USE_YN == "Y").FirstOrDefault().CO_CD_DOC,
                                  REV_DTM = b.REV_DTM
                              };

                foreach (var item in revInfo)
                {
                    //뭔가 정렬 굉장히 맘에 안드는데..
                    arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new S_MA_USR_REV_LIST()
                                                                        {
                                                                            CONS_ID = item.CONS_ID,
                                                                            CONS_NM = item.CONS_NM,
                                                                            CONS_THUMB_PATH = Convert.ToBase64String(ImageToByteArray(item.CONS_THUMB_PATH)),
                                                                            CONS_TP = item.CONS_TP,
                                                                            REV_DTM = item.REV_DTM.Value.ToString("yyyyMMdd HH:mm:ss")
                                                                        }
                                                                     ))
                                 );
                }
            }

            return StaticUtils.ListToXml(arrResult);
        }

        /// <summary>
        /// 컨텐츠 메인뷰 정보 조회
        /// </summary>
        /// <param name="CONS_ID"></param>
        /// <returns></returns>
        public string S_CV_CTS_INFO_SIN(string CONS_ID)
        {
            List<string> arrResult = new List<string>();
            // List<Dictionary<string, string>> arrResult2 = new List<Dictionary<string, string>>();

            using (imrEntities entities = new imrEntities())
            {
                var conInfo = from c in entities.OTCSINFO
                              from d in entities.OTADCOPY

                              where c.CONS_ID == CONS_ID
                                 && c.CONS_COPY_CD == d.CONS_COPY_CD

                              select new
                              {
                                  CONS_ID = c.CONS_ID,
                                  CONS_NM = c.CONS_NM,
                                  CONS_PREV_PATH = entities.OTCSSCRE.Where(a => a.CONS_ID == c.CONS_ID).FirstOrDefault().CONS_PREV_PATH,
                                  CONS_THUMB_PATH = entities.OTCSSCRE.Where(a => a.CONS_ID == c.CONS_ID).FirstOrDefault().CONS_THUMB,
                                  STAR_RATE = (from e in entities.OTCSREVW
                                               where e.CONS_ID == c.CONS_ID
                                                  && e.USE_YN == "Y"
                                               select e).Count().ToString(),
                                  CONS_PRICE = c.CON_PRICE,
                                  COPY_DOC = entities.MTCOTPCD.Where(a => a.CO_CD_DTL == c.CONS_COPY_CD && a.USE_YN == "Y").FirstOrDefault().CO_CD_DOC,
                                  CONS_SIZE = c.CONS_SIZE,
                                  CONS_DOC = c.CONS_DOC,
                                  COPY_NM = d.COPY_NM,
                                  COPY_PHONE = d.COPY_PHONE,
                                  COPY_MAIL_ADDRESS = d.COPY_MAIL,
                                  COPY_HOME_PAGE = d.COPY_HOMEPAGE,
                                  CONS_TP = entities.MTCOTPCD.Where(a => a.CO_CD_DTL == c.CONS_TP_CD && a.USE_YN == "Y").FirstOrDefault().CO_CD_DOC,
                                  CONS_CAT = entities.MTCOTPCD.Where(a => a.CO_CD_DTL == c.CONS_CT_CD && a.USE_YN == "Y").FirstOrDefault().CO_CD_DOC,
                                  CONS_PATH = c.CONS_PATH
                              };

                foreach (var item in conInfo)
                {
                    //뭔가 정렬 굉장히 맘에 안드는데..
                    arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new S_CV_CTS_INFO_SIN()
                                                                        {
                                                                            CONS_ID = item.CONS_ID,
                                                                            CONS_NM = item.CONS_NM,
                                                                            CONS_PREV_PATH = Convert.ToBase64String(CompressFolder(item.CONS_PREV_PATH)),
                                                                            CONS_THUMB_PATH = Convert.ToBase64String(ImageToByteArray(item.CONS_THUMB_PATH)),
                                                                            STAR_RATE = item.STAR_RATE,
                                                                            CONS_PRICE = item.CONS_PRICE.ToString(),
                                                                            COPY_DOC = item.COPY_DOC,
                                                                            CONS_SIZE = item.CONS_SIZE.ToString(),
                                                                            CONS_DOC = item.CONS_DOC,
                                                                            COPY_NM = item.COPY_NM,
                                                                            COPY_PHONE = item.COPY_PHONE,
                                                                            COPY_MAIL_ADDRESS = item.COPY_MAIL_ADDRESS,
                                                                            COPY_HOME_PAGE = item.COPY_HOME_PAGE,
                                                                            CONS_TP = item.CONS_TP,
                                                                            CONS_CAT = item.CONS_CAT,
                                                                            CONS_PATH = item.CONS_PATH
                                                                        }
                                                                     ))
                                 );
                }
            }

            return StaticUtils.ListToXml(arrResult);
        }

        /// <summary>
        /// 컨텐츠 리뷰 리스트 조회
        /// </summary>
        /// <param name="CONS_ID"></param>
        /// <returns></returns>
        public string S_CV_CTS_REV_LIST(string CONS_ID)
        {
            List<string> arrResult = new List<string>();
            // List<Dictionary<string, string>> arrResult2 = new List<Dictionary<string, string>>();

            using (imrEntities entities = new imrEntities())
            {
                var revInfo = from b in entities.OTCSINFO
                              from c in entities.OTCSREVW

                              where b.CONS_ID == CONS_ID
                                 && b.CONS_ID == c.CONS_ID

                              select new
                              {
                                  USER_NO = c.USER_NO
                                ,
                                  CONS_ID = b.CONS_ID
                                ,
                                  REV_SEQ = c.REV_SEQ
                                ,
                                  REV_DOC = c.REV_DOC
                                ,
                                  REV_DTM = c.REV_DTM
                                ,
                                  STAR_RATE = c.CONS_ST
                                ,
                                  USER_NM = entities.OTUSINFO.Where(a => a.USER_NO == c.USER_NO && a.USE_YN == "Y").FirstOrDefault().USER_NM
                                ,
                                  USER_NICK = entities.OTUSINFO.Where(a => a.USER_NO == c.USER_NO && a.USE_YN == "Y").FirstOrDefault().USER_NICK_NM
                              };

                foreach (var item in revInfo)
                {
                    //뭔가 정렬 굉장히 맘에 안드는데..
                    arrResult.Add(StaticUtils.DictionaryToXml(StaticUtils.GetDictionaryFieldValue(
                                                                        new S_CV_CTS_REV_LIST()
                                                                        {
                                                                            USER_NO = item.USER_NO
                                                                          ,
                                                                            CONS_ID = item.CONS_ID
                                                                          ,
                                                                            REV_SEQ = item.REV_SEQ.ToString()
                                                                          ,
                                                                            REV_DOC = item.REV_DOC
                                                                          ,
                                                                            REV_DTM = item.REV_DTM.Value.ToString("yyyyMMdd HH:mm:ss")
                                                                          ,
                                                                            STAR_RATE = item.STAR_RATE
                                                                          ,
                                                                            USER_NM = item.USER_NM
                                                                          ,
                                                                            USER_NICK = item.USER_NICK
                                                                        }
                                                                     ))
                                 );
                }
            }

            return StaticUtils.ListToXml(arrResult);
        }

        /// <summary>
        /// 앱 버전 조회
        /// </summary>
        /// <returns></returns>
        public string S_CO_ETC_VER_SIN()
        {
            List<string> arrResult = new List<string>();
            // List<Dictionary<string, string>> arrResult2 = new List<Dictionary<string, string>>();

            using (imrEntities entities = new imrEntities())
            {
                //아직 없엉
            }

            return StaticUtils.ListToXml(arrResult);
        }

        public int 입력_받은_숫자_만큼_팩토리얼_돌리는_함수_반환값은_인티저(int 숫자다)
        {
            int 숫자_보관 = 숫자다;
            int 결과_숫자 = 숫자_보관;

            for (; ; )
            {
                if (숫자_보관 == 0)
                {
                    break;
                }
                결과_숫자 = 결과_숫자 * (숫자_보관 - 1);
                숫자_보관 = 숫자_보관 - 1;
            }

            return 결과_숫자;
        }
    }
}
