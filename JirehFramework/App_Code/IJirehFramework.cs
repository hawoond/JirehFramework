using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

// 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드 및 config 파일에서 인터페이스 이름  "IService"을 변경할 수 있습니다.
[ServiceContract]
public interface IJirehFramework
{
    [OperationContract]
    [WebInvoke(Method = "POST", UriTemplate = "Message/{sInputMessage}")]
    string CallService(string sInputMessage);

    [OperationContract]
	CompositeType GetDataUsingDataContract(CompositeType composite);

    [OperationContract]
    string FileDownload(string sDownloadUri, string sPort, string sUserID, string sUserPassword, string sFilename);

    [OperationContract]
    string HttpDownload(string FileKind, string FilePath);
}

// 아래 샘플에 나타낸 것처럼 데이터 계약을 사용하여 복합 형식을 서비스 작업에 추가합니다.
[DataContract]
public class CompositeType
{
	bool boolValue = true;
	string stringValue = "Hello ";

	[DataMember]
	public bool BoolValue
	{
		get { return boolValue; }
		set { boolValue = value; }
	}

	[DataMember]
	public string StringValue
	{
		get { return stringValue; }
		set { stringValue = value; }
	}
}
