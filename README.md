# Solution structure (ASP.NET MVC 5) 
 ![Solution structure](https://user-images.githubusercontent.com/33324048/200167298-123ef5f2-19ab-4056-84d2-7aa378c6220b.JPG)
 
 
 **GRP.Web:** Contains views (.cshtml/.js and .css/web controllers). Frontend technology used: Easyui (https://www.jeasyui.com/) library and jQuery
 
 **GRP.Models:** Contains model classes used to pass data from view to api controllers and vice versa
 
 **GRP API:** contain all api rest controllers used to post or get data from db
 
 **GRP.DataAccess:** contains dal helper classes used to call database operations using stored procedures
 
 
  # نظام الشؤون القانونية - ملخص

- نظام الشؤون القانونية يستخدم من قبل إدارة الشؤون القانونية في الهلال الاحمر لإدخال بيانات قضايا وجلسات وإرسال إشعار للموظف عند تسجيله في جلسة.
الشاشات المذكورة أسفل هي:
* 1- شاشة لإدخال بيانات القضية(LawReg)
* 2- شاشة لإدخال جلسة في قضية(SessionReg)


 # below screen is used to register a new case
 ### LawReg.cshtml
 ![law register](https://user-images.githubusercontent.com/33324048/200169683-71f64baf-cf2e-4bbb-96f1-152e077771b1.JPG)
 
 ### post ajax request for saving new case (in ...GRP.Web/Areas/LAW/scripts/Register/LawReg.js)
 ```JavaScript
   doAjax('LAW/Register/SaveLawCaseRequest', 'post', JSON.stringify(LawCaseReqData), 'application/json',
       function (result) {
           hideProgress('#divLawInfo');
           if (result.res > 0) {
               showAlert('تنبيه', 'تم الحفظ بنجاح.', 'success');
               if (afterSaveCBFunc)
                   afterSaveCBFunc();
               else {
                   //Set CaseSerial & Year
                   if (LawCaseReqData.LawCaseReqMstr.df == 'I') {
                       $('#CaseSerial').numberbox('setValue', result.CaseSerial); //set new CaseSerial
                       LawCaseReqData.LawCaseReqMstr.CASE_SERIAL = result.CaseSerial;
                   }
                   _LawCaseChangesObj.hasChanges = false;
                   _LawCaseCurrentReqObj = LawCaseReqData;
                   //function to refresh page with new case serial
                   //NewCaseFormRequest();
                   getCaseInfoForRequest($('#CaseYear').numberbox('getValue'), $('#CaseSerial').numberbox('getValue'));
               }

           } else {
               showAlert('خطأ', result.msg, 'error');
           }
       },
       function (error) {
           debugger;
           hideProgress('#divLawInfo');
           showAlert('خطأ', error, 'error');
       }
       );
```

### GRP.Web / web controller (in ...GRP.Web/Areas/LAW/Controllers/LawReg.cs)
making an http post request through the "PostRequest" method with the form data "reqData". which will hit the api endpoint (LawReg/SaveLawCaseRequest)
``` C Sharp
[HttpPost]
 public async Task<ActionResult> SaveLawCaseRequest(LawCaseReqSaveRequest reqData)
 {
     RestClient<LawCaseReqSaveResult> client = new RestClient<LawCaseReqSaveResult>(Request.GetOwinContext());
     LawCaseReqSaveResult lawSaveResult = await client.PostRequest("LawReg/SaveLawCaseRequest", reqData);
     return Json(lawSaveResult);
 }
```

### GRP.Api / api Controller (in GRP.API/Controllers/LAW/LawRegController.cs)
``` C Sharp
[HttpPost]
 public IHttpActionResult SaveLawCaseRequest(LawCaseReqSaveRequest saveReq)
 {
     LawRegDH dah = new LawRegDH(Environment);
     LawCaseReqSaveResult retVal = dah.SaveLawCaseRequest(saveReq, Convert.ToDecimal(UserId));

     return Ok(retVal);
 }
 ```
 
 ### GRP.Dataaccess / LawRegDH class (in GRP.DataAccess/Handlers/LAW/LawRegDH.cs)
 ``` C Sharp
 public LawCaseReqSaveResult SaveLawCaseRequest(LawCaseReqSaveRequest saveReq, Decimal UserId)
        {
            LawCaseReqSaveResult retVal = new LawCaseReqSaveResult();
            try
            {
                List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

                string xmlData = GetLawCaseMasterDataAsXml(saveReq.LawCaseReqMstr);

                //Input Params
                lstParams.Add(new OracleDbParameter("I_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, UserId));
                if (string.IsNullOrEmpty(xmlData))
                    lstParams.Add(new OracleDbParameter("I_MASTER_DATA", OracleDataType.XmlType, OracleDbParameterDirection.Input, DBNull.Value));
                else
                    lstParams.Add(new OracleDbParameter("I_MASTER_DATA", OracleDataType.XmlType, OracleDbParameterDirection.Input, xmlData));

                //Output Params
                lstParams.Add(new OracleDbParameter("O_CASE_YEAR", OracleDataType.Decimal, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_CASE_SERIAL", OracleDataType.Decimal, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_AFFECTED_ROWS", OracleDataType.Decimal, OracleDbParameterDirection.Output));
                lstParams.Add(new OracleDbParameter("O_ERR_MSG", OracleDataType.Varchar2, 500, OracleDbParameterDirection.Output));

                DALHelper dalh = new DALHelper(ConnectionStringName);
                DataTable dtResultData = dalh.GetDataResultSet("LAW_FRM_CASE_REQ_DML", OracleCommandType.StoredProcedure, lstParams);

                retVal.CaseYear = lstParams.First(prm => prm.Name == "O_CASE_YEAR").Value.ToString().Replace("null", string.Empty);
                retVal.CaseSerial = lstParams.First(prm => prm.Name == "O_CASE_SERIAL").Value.ToString().Replace("null", string.Empty);
                string rs = lstParams.First(prm => prm.Name == "O_AFFECTED_ROWS").Value.ToString().Replace("null", string.Empty);
                retVal.res = string.IsNullOrEmpty(rs) ? 0 : Convert.ToInt32(rs);
                retVal.msg = lstParams.First(prm => prm.Name == "O_ERR_MSG").Value.ToString().Replace("null", string.Empty);

                lstParams.Clear();

                return retVal;
            }
            catch (Exception ex)
            {
                retVal.res = 0;
                retVal.msg = ex.Message;

                return retVal;
            }
        }
```

# below is Session regisrtation screen
### SessionReg.cshtml
![session add](https://user-images.githubusercontent.com/33324048/200170901-b722f311-d64b-42b8-8eb1-7a42889c1acb.JPG)

### Post ajax request for saving a new session (in ...GRP.Web/Areas/LAW/scripts/Register/SessionReg.js)
```JavaScript
doAjax('LAW/Register/SaveLawSessionRequest', 'post', JSON.stringify(LawSessionReqData), 'application/json',
            function (result) {
                hideProgress('#SessionRegLayout');
                if (result.res > 0 && result.msg == '') {
                    FileData = undefined; //empty file data
                    showAlert('تنبيه', 'تم الحفظ بنجاح.', 'success');
                    $('#txtSessionSerial').textbox('setValue', result.SessionSerial);
                    //TODO: function to refresh LawReg.cshtml with new added session
                } else {
                    showAlert('خطأ', result.msg, 'error');
                }
                _LawEmpsSessionGrdDeletedRecords = [];
            },
            function (error) {
                hideProgress('#SessionRegLayout');
                showAlert('خطأ', error, 'error');
            }
            );
```
 ### GRP.Web / web contoller (in ...GRP.Web/Areas/LAW/Controllers/SessionReg.cs)
 NotifyList notify is used to send SignalR notification to added employees using **_notificationsHubContex** object
 
 ``` C Sharp
  private readonly Microsoft.AspNet.SignalR.IHubContext _notificationsHubContex =
            Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<RequestsNotificationHub>();
            
  [HttpPost]
  public async Task<ActionResult> SaveLawSessionRequest(LawSessionReqSaveRequest sessionReq)
  {
      RestClient<LawSessionReqSaveResult> client = new RestClient<LawSessionReqSaveResult>(Request.GetOwinContext());
      //foreach (LawSessionReqAttachments Att in sessionReq.LawSessionReqAtt)
      //{
      //    //byte[] bytes = Att.FileBytes.SelectMany(BitConverter.GetBytes).ToArray();
      //    byte[] bytes = Att.FileBytes.Cast<int>().Select(i => (byte)i).ToArray();
      //    Att.ATTACH_FILE = bytes;
      //    //sessionReq.fileBytes = bytes;
      //}

      LawSessionReqSaveResult result = await client.PostRequest("SessionReg/SaveNewLawSessionRequest", sessionReq);
      if (sessionReq.LawCaseReqDet.DF != "D" && result.res > 0 && result.msg == "")
      {
          foreach (NotifyList notify in result.notifyLst)
          {
              Notification notification = new Notification();
              notification.GroupName = notify.NOTICE_FLAG;
              notification.GroupText = notify.NOTICE_NAME;
              notification.Text = string.Format("{0} - {1}", result.SessionSerial, notify.EMPNAME);
              notification.Params.Add("rId", SessionRegNotification.GetNotificationId(notify.NOTICE_FLAG, result.SessionSerial));
              notification.Params.Add("tabTxt", SessionRegNotification.GetApproveNotificationTabText(notify.NOTICE_FLAG, result.SessionSerial));
              notification.Params.Add("url", SessionRegNotification.GetApprovalnotificationUrl(notify.NOTICE_FLAG, result.SessionSerial, result.CaseYear, result.CaseSerial));

              _notificationsHubContex.Clients.User(notify.USER_ID).addRequestNotification(notification);
          }
      }

      return Json(result);
  }
```
### GRP.API / api contoller (in ...GRP.API/Controllers/LAW/SessionRegController.cs)
```C Sharp
[HttpPost]
public IHttpActionResult SaveNewLawSessionRequest(LawSessionReqSaveRequest sessionReq)
{
    SessionRegDH dah = new SessionRegDH(Environment);
    LawSessionReqSaveResult retVal = dah.SaveNewSessionRequest(sessionReq, Convert.ToDecimal(UserId));

    return Ok(retVal);
}
```

### GRP.Dataaccess / SessionRegDH class (in ...GRP.DataAccess/Handlers/LAW/SessionRegDH.cs)
``` CSharp
public LawSessionReqSaveResult SaveNewSessionRequest(LawSessionReqSaveRequest sessionReq, Decimal UserId)
 {
     LawSessionReqSaveResult retVal = new LawSessionReqSaveResult();
     try
     {
         List<OracleDbParameter> lstParams = new List<OracleDbParameter>();

         string xmlData = GetSessionMasterDataAsXml(sessionReq.LawCaseReqDet);

         //Input Params
         lstParams.Add(new OracleDbParameter("I_USER_ID", OracleDataType.Decimal, OracleDbParameterDirection.Input, UserId));
         lstParams.Add(new OracleDbParameter("I_CASE_YEAR", OracleDataType.Decimal, OracleDbParameterDirection.Input, sessionReq.LawCaseReqDet.CASE_YEAR));
         lstParams.Add(new OracleDbParameter("I_CASE_SERIAL", OracleDataType.Decimal, OracleDbParameterDirection.Input, sessionReq.LawCaseReqDet.CASE_SERIAL));
         //lstParams.Add(new OracleDbParameter("I_SESSION_ATT", OracleDataType.Blob, OracleDbParameterDirection.Input, sessionReq.fileBytes));


         if (string.IsNullOrEmpty(xmlData))
             lstParams.Add(new OracleDbParameter("I_SESSION_DATA", OracleDataType.XmlType, OracleDbParameterDirection.Input, DBNull.Value));
         else
             lstParams.Add(new OracleDbParameter("I_SESSION_DATA", OracleDataType.XmlType, OracleDbParameterDirection.Input, xmlData));

         xmlData = GetSessionEmpsDataAsXml(sessionReq.LawSessionEmps);
         if (string.IsNullOrEmpty(xmlData))
             lstParams.Add(new OracleDbParameter("I_EMP_SESSION", OracleDataType.XmlType, OracleDbParameterDirection.Input, DBNull.Value));
         else
             lstParams.Add(new OracleDbParameter("I_EMP_SESSION", OracleDataType.XmlType, OracleDbParameterDirection.Input, xmlData));

         //xmlData = GetSessionAttachmentsDataAsXml(sessionReq.LawSessionReqAtt);
         //if (string.IsNullOrEmpty(xmlData))
         //    lstParams.Add(new OracleDbParameter("I_SESSION_ATTACH", OracleDataType.XmlType, OracleDbParameterDirection.Input, DBNull.Value));
         //else
         //    lstParams.Add(new OracleDbParameter("I_SESSION_ATTACH", OracleDataType.XmlType, OracleDbParameterDirection.Input, xmlData));

         //Output Params
         lstParams.Add(new OracleDbParameter("O_NEXT_NOTIFY", OracleDataType.RefCursor, OracleDbParameterDirection.Output));
         lstParams.Add(new OracleDbParameter("O_CASE_YEAR", OracleDataType.Decimal, OracleDbParameterDirection.Output));
         lstParams.Add(new OracleDbParameter("O_CASE_SERIAL", OracleDataType.Decimal, OracleDbParameterDirection.Output));
         lstParams.Add(new OracleDbParameter("O_SESSION_ID", OracleDataType.Decimal, OracleDbParameterDirection.Output));
         lstParams.Add(new OracleDbParameter("O_AFFECTED_ROWS", OracleDataType.Decimal, OracleDbParameterDirection.Output));
         lstParams.Add(new OracleDbParameter("O_ERR_MSG", OracleDataType.Varchar2, 500, OracleDbParameterDirection.Output));

         DALHelper dalh = new DALHelper(ConnectionStringName);
         DataSet dtResultData = dalh.GetDataMultipleResultSet("LAW_FRM_CASE_SESSION_DML", OracleCommandType.StoredProcedure, lstParams);

         if (dtResultData.Tables["O_NEXT_NOTIFY"].Rows.Count > 0)
         {
             retVal.notifyLst = InstanceMapper<NotifyList>.CreateList(dtResultData.Tables["O_NEXT_NOTIFY"].Rows);
         }
         else
         {
             retVal.notifyLst = new List<NotifyList>();
         }
         retVal.CaseYear = lstParams.First(prm => prm.Name == "O_CASE_YEAR").Value.ToString().Replace("null", string.Empty);
         retVal.CaseSerial = lstParams.First(prm => prm.Name == "O_CASE_SERIAL").Value.ToString().Replace("null", string.Empty);
         retVal.SessionSerial = lstParams.First(prm => prm.Name == "O_SESSION_ID").Value.ToString().Replace("null", string.Empty);
         string rs = lstParams.First(prm => prm.Name == "O_AFFECTED_ROWS").Value.ToString().Replace("null", string.Empty);
         retVal.res = string.IsNullOrEmpty(rs) ? 0 : Convert.ToInt32(rs);
         retVal.msg = lstParams.First(prm => prm.Name == "O_ERR_MSG").Value.ToString().Replace("null", string.Empty);

         lstParams.Clear();

         return retVal;
     }
     catch (Exception ex)
     {
         retVal.res = 0;
         retVal.msg = ex.Message;

         return retVal;
     }
 }
 ```
