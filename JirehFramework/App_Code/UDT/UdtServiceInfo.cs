using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// UdtServiceInfo의 요약 설명입니다.
/// </summary>
public class UdtServiceInfo
{
    #region 속성
    /// <summary>
    /// 서비스 이름
    /// </summary>
    private string mServiceName;
    public string ServiceName
    {
        get
        {
            return mServiceName;
        }
        set
        {
            mServiceName = value;
        }
    }

    /// <summary>
    /// Input Message type
    /// </summary>
    private string mInputType;
    public string InputType
    {
        get
        {
            return mInputType;
        }

        set
        {
            mInputType = value;
        }
    }

    /// <summary>
    /// Return Message Type
    /// </summary>
    private string mReturnType;
    public string ReturnType
    {
        get
        {
            return mReturnType;
        }

        set
        {
            mReturnType = value;
        }
    }

    /// <summary>
    /// Input 파라미터
    /// </summary>
    private Dictionary<string, string> mDicParams;
    public Dictionary<string, string> DicParams
    {
        get
        {
            return mDicParams;
        }

        set
        {
            mDicParams = value;
        }
    }

    /// <summary>
    /// ConnectionString 이름
    /// </summary>
    private string mConnectName;
    public string ConnectName
    {
        get
        {
            return mConnectName;
        }

        set
        {
            mConnectName = value;
        }
    }

    private string mIpAddress;
    public string IpAddress
    {
        get
        {
            return mIpAddress;
        }
        set
        {
            mIpAddress = value;
        }
    }
    #endregion

    #region 생성자
    /// <summary>
    /// 그냥 매너상 생성자
    /// </summary>
    public UdtServiceInfo()
    {
        this.mServiceName = string.Empty;
        this.mInputType = string.Empty;
        this.mReturnType = string.Empty;
        this.mDicParams = new Dictionary<string, string>();
        this.mConnectName = string.Empty;
        this.mIpAddress = string.Empty;
    }

    /// <summary>
    /// 데이터 삽입 생성자
    /// </summary>
    /// <param name="sServiceName"></param>
    /// <param name="sInputType"></param>
    /// <param name="sReturnType"></param>
    /// <param name="dicParams"></param>
    /// <param name="sConnectName"></param>
    public UdtServiceInfo(string sServiceName, string sInputType, string sReturnType, Dictionary<string, string> dicParams, string sConnectName, string sIpAddress)
    {
        this.mServiceName = sServiceName;
        this.mInputType = sInputType;
        this.mReturnType = sReturnType;
        this.mDicParams = dicParams;
        this.mConnectName = sConnectName;
        this.mIpAddress = sIpAddress;
    }
    #endregion

    #region 기타 기능
    /// <summary>
    /// 클론 생성 함수
    /// </summary>
    /// <returns></returns>
    public UdtServiceInfo Clone()
    {
        UdtServiceInfo cloneUdtServiceInfo = new UdtServiceInfo();
        cloneUdtServiceInfo.ServiceName = this.ServiceName;
        cloneUdtServiceInfo.InputType = this.InputType;
        cloneUdtServiceInfo.ReturnType = this.ReturnType;
        cloneUdtServiceInfo.DicParams = this.DicParams;
        cloneUdtServiceInfo.ConnectName = this.ConnectName;
        cloneUdtServiceInfo.IpAddress = this.IpAddress;

        return cloneUdtServiceInfo;
    }
    #endregion
}