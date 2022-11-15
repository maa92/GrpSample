//Emps Datagrid
var _LawEmpsSessionDetChangesObj, _LawEmpsSessionDetCurrentGridRow;
var _LawEmpsSessionDetGrdData = _LawEmpsSessionDetGrdEditors = _LawEmpsSessionDetGrdDeletedRecords = [];
var _LawEmpsSessionDetGridEditIndex = undefined;
var _LawEmpsSessionDetEditingList = []
//Files Datagrid
var _LawFilesSessionDetChangesObj, _LawFilesSessionDetCurrentGridRow;
var _LawFilesSessionDetGrdData = _LawFilesSessionDetGrdEditors = _LawFilesSessionDetGrdDeletedRecords = [];
var _LawFilesSessionDetGridEditIndex = undefined;
var _LawFilesSessionDetEditingList = [];

var EmpLoader = function (param, success, error) {
    var q = param.q || '';
    if (q.length < 2) { return false }

    doAjax('HR/Common/SearchEmp?q=' + encodeURIComponent(q), 'post', {}, 'application/json',
        function (result) { success(result); }, function (error) { error.apply(this, arguments); }
    );
}

function addHijriIconDet() {
    $('.easyui-hdatebox').next().find("a").addClass("hdateboxicon");
}

function SessionDetailsViewFileFormatter(value, row, index) {
    return '<a href="javascript:{}"  onclick="DisplaySessionFile(this);">عرض</a>' + ' | ' + '<a href="javascript:{}"  onclick="DownloadSessionFile(this);">تحميل</a>';
}

function DisplaySessionFile(rowBtn) {
    $('#tblFilesInSessionDet').datagrid('selectRow', $(rowBtn).closest('tr').index());
    var selRow = $('#tblFilesInSessionDet').datagrid('getSelected');
    console.log(selRow);
    const b64Data = selRow.fileObj.InputStream; //base64 string
    const byteChar = atob(b64Data);  //converts base64 string To binary
    const byteNumbers = new Array(byteChar.length);
    for (var i = 0; i < byteChar.length; i++) {
        byteNumbers[i] = byteChar.charCodeAt(i);
    }
    debugger;
    var byteArray = new Uint8Array(byteNumbers);
    var fileData = JSON.stringify(selRow.fileObj.InputStream);
    var blob = new Blob([byteArray], { type: selRow.fileObj.ContentType });
    var f = new File([blob], selRow.fileObj.FileName, { type: blob.type });
    console.log(f);
    let link = document.createElement('a');
    //link.download = blob;
    link.href = window.open(URL.createObjectURL(blob));
    //link.click();
    console.log(blob);
}

function DownloadSessionFile(rowBtn) {
    debugger;
    $('#tblFilesInSessionDet').datagrid('selectRow', $(rowBtn).closest('tr').index());
    var selRow = $('#tblFilesInSessionDet').datagrid('getSelected');
    console.log(selRow);
    const b64Data = selRow.fileObj.InputStream; //base64 string
    const byteChar = atob(b64Data);  //converts base64 string To binary
    const byteNumbers = new Array(byteChar.length);
    for (var i = 0; i < byteChar.length; i++) {
        byteNumbers[i] = byteChar.charCodeAt(i);
    }
    debugger;
    var byteArray = new Uint8Array(byteNumbers);
    var fileData = JSON.stringify(selRow.fileObj.InputStream);
    var blob = new Blob([byteArray], { type: selRow.fileObj.ContentType });
    var f = new File([blob], selRow.fileObj.FileName, { type: blob.type });
    console.log(f);
    let link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = selRow.fileObj.FileName;
    link.click();
    URL.revokeObjectURL(blob);
    console.log(blob);
}


function openSessionDetailForm() {
    clearSessionDetailsForm();
    loadDetailFormLookupData();
    showProgress('#SessionDetLayout', 'جاري تحميل البيانات...');
    debugger;
    var SessionId = $('#dSessionDetailsForm').window('options').title.split(' : ')[1];
    doAjax('LAW/Register/SessionDetailsRet?CaseYear=' + $('#CaseYear').numberbox('getValue')
                                                      + '&CaseSerial=' + $('#CaseSerial').numberbox('getValue')
                                                      + '&SessionId=' + SessionId, 'post', null, 'application/json',
        function (result) {
            debugger;
            hideProgress('#SessionDetLayout');
            //console.log(result);
            //var strJson = JSON.stringify(result);
            var ParsedData = JSON.parse(result);  //parseing json object
            console.log(ParsedData);
            fillSessionDetailsForm(ParsedData); //ParsedData
        },
        function (error) {
            debugger;
            hideProgress('#SessionDetLayout');
            showAlert('خطأ', error, 'error');
        }
    );
}

function loadDetailFormLookupData() {
    doAjax('LAW/Register/LoadDetailFormLookups', 'get', {}, 'application/json',
        function (lookupData) {
            console.log(lookupData);
            $('#SessionDetStatus').combobox('loadData', lookupData.sStatus);
            $('#AttachmentTypeDet').combobox('loadData', lookupData.aType);
        },
        function (error) {
            showAlert('خطأ', error, 'error');
        });
}

function fillSessionDetailsForm(dataObj) {
    debugger;
    var fileList = [];
    console.log(dataObj);
    $('#txtSessionDetSerial').textbox('setValue', dataObj.LawSessionReqDet.SESSION_ID);
    $('#txtSessionDetDateH').hdatebox('setValue', dataObj.LawSessionReqDet.SESSION_DATE_H);
    $('#txtSessionDetDateG').datebox('setValue', dataObj.LawSessionReqDet.SESSION_DATE_G);
    $('#SessionDetStatus').combobox('setValue', dataObj.LawSessionReqDet.SESSION_STATUS);
    //$('#txtSessionDetCourtName').textbox('setValue', dataObj.LawSessionReqDet.THE_COURT);
    $('#txtSessionDetDay').textbox('setValue', dataObj.LawSessionReqDet.SESSION_DAY);
    $('#txtSessionDetTime').textbox('setValue', dataObj.LawSessionReqDet.SESSION_TIME);

    $('#txtSessionDetNotes').textbox('setValue', dataObj.LawSessionReqDet.SESSION_NOTES);
    var EmpDetails = dataObj.LawSessionEmps;
    $.each(EmpDetails, function (i, EmpDtl) {
        EmpDetails[i].EMP_CODE = EmpDtl.EMP_CODE + ' - ' + EmpDtl.EMP_NAME,
        EmpDetails[i].SESSION_NOTE = EmpDtl.SESSION_NOTE
    });
    _LawEmpsSessionDetGrdData = dataObj.LawSessionEmps;
    $('#tblEmpsInSessionDet').datagrid('loadData', dataObj.LawSessionEmps);

    var AttDetails = dataObj.SessionAttachments
    _LawFilesSessionDetGrdData = dataObj.SessionAttachments;
    var data = new FormData();
    $.each(AttDetails, function (i, AttDtl) {
        data.append('file', new Blob([JSON.stringify(AttDetails[i].fileObj)], { type: AttDetails[i].ATTACH_FILE_TYPE }));
        AttDetails[i].File = data,
        AttDetails[i].ATTACH_ID = AttDtl.ATTACH_ID,
        AttDetails[i].ATTACH_TYPE = AttDtl.ATTACH_TYPE_DESC
        AttDetails[i].File_Name = AttDtl.fileObj.FileName
    });
    $('#tblFilesInSessionDet').datagrid('loadData', AttDetails);

    $('#txtLawSessionReqCBy').textbox('setValue', dataObj.LawSessionReqDet.CBY);
    $('#txtLawSessionReqCDt').textbox('setValue', dataObj.LawSessionReqDet.CDT);
    $('#txtLawSessionReqUBy').textbox('setValue', dataObj.LawSessionReqDet.UBY);
    $('#txtLawSessionReqUDt').textbox('setValue', dataObj.LawSessionReqDet.UDT);
}

function clearSessionDetailsForm() {
    $('#txtSessionDetDateH').hdatebox('clear');
    $('#txtSessionDetDateG').datebox('clear');
    $('#numOfCaseSessionDet').numberbox('clear');
    $('#SessionDetStatus').combobox('clear');
    $('#txtSessionDetCourtName').textbox('clear');
    $('#txtSessionDetNotes').textbox('clear');
    $('#tblEmpsInSessionDet').datagrid('loadData', []);
    $('#tblFilesInSessionDet').datagrid('loadData', []);
}

$(document).ready(function () {
    setTimeout(addHijriIconDet, 1);
    $.fn.datebox.defaults.formatter = formatterG;
    $.fn.datebox.defaults.parser = parserG;

    //Files
    _LawFilesSessionDetChangesObj = _LawFilesSessionDetCurrentGridRow = null;
    _LawFilesSessionDetGrdData = _LawFilesSessionDetGrdEditors = _LawFilesSessionDetGrdDeletedRecords = [];

    _LawFilesSessionDetChangesObj = getCurrentFormChangesObj();
    _LawFilesSessionDetChangesObj = false;

    //Emps
    _LawEmpsSessionDetChangesObj = _LawEmpsSessionDetCurrentGridRow = null;
    _LawEmpsSessionGrdData = _LawEmpsSessionGrdEditors = _LawEmpsSessionGrdDeletedRecords = [];

    _LawEmpsSessionDetChangesObj = getCurrentFormChangesObj();
    _LawEmpsSessionDetChangesObj = false;

});

function TxtSessionDetYearHjChange(newVal, oldVal) {
    var wndId = this.id.split('_')[1];
    //if (!_LawCaseLoadingDataFromSearch) {
    if (_gsAppHDP.isValidStrDate(newVal)) {
        var gDateArr = _gsAppHDP.gDateFromStr(newVal).split('/');
        $('#txtSessionDetDateG').textbox('setValue', gDateArr[0] + '/' + gDateArr[1] + '/' + gDateArr[2]);
        //$('#txtSessionDateG').textbox('setValue', _gsAppHDP.gDateFromStr(newVal));            
    }
    // }
}

function TxtSessionDetYearGrgChange(nv, ov) {
    var wndId = this.id.split('_')[1];
    //if (!_LawCaseLoadingDataFromSearch) {
    if (nv != '' && nv != null) {
        //debugger;
        doAjax('Opr/Common/GetHijriDate?gDate=' + $('#txtSessionDetDateG').hdatebox('getValueF', 'yyyymmdd') + '&flag=' + 2, 'get', null, 'application/json',
             function (hDt) {
                 var y = hDt.substr(0, 4);
                 var m = hDt.substr(4, 2);
                 var d = hDt.substr(6, 2);
                 var hijDT = y + '/' + m + '/' + d;
                 //hideProgress('#divParticipationReq');
                 $('#txtSessionDetDateH').hdatebox('setValue', hijDT);
             },
             function (error) {
                 //hideProgress('#divParticipationReq');
                 showAlert('خطأ', error, 'error');
             }
         );
    }

    // }
}

function doSaveSessionDet() {
    //update session details
    debugger;
    var EmpDgArrDet = [];
    var FileDgArrDet = [];
    if (_LawEmpsSessionDetGridEditIndex != undefined)
        endEditingEmpsSessionDetRecordsData();

    var isNew = _LawCaseCurrentReqObj == null;
    var EmpRowsDet = $('#tblEmpsInSessionDet').datagrid('getRows');
    var FileRowsDet = $('#tblFilesInSessionDet').datagrid('getRows');
    for (var i = 0; i < EmpRowsDet.length; i++) {
        var isNewEmp = EmpRowsDet[i].df == undefined;  //if df does not exist in EmpRowsDet then set df to 'U'. if it does it means user added new emp in datagrid
        EmpDgArrDet.push({ df: isNewEmp ? 'U' : 'I', SESSION_ID: $('#txtSessionDetSerial').numberbox('getValue'), EMP_CODE: EmpRowsDet[i].EMP_CODE.split(' - ')[0], EmpName: EmpRowsDet[i].EMP_CODE.split(' - ')[1], SESSION_NOTE: EmpRowsDet[i].SESSION_NOTE });
    }
    _LawEmpsSessionDetGrdData = EmpDgArrDet;
    var empChangesDetList = _.filter(_LawEmpsSessionDetGrdData, function (sessionEmp) { return sessionEmp.df == 'I' || sessionEmp.df == 'U' });
    var LawSessionReqData = {
        LawCaseReqDet: {
            df: 'U',
            CASE_YEAR: $('#CaseYear').numberbox('getValue'),
            CASE_SERIAL: $('#CaseSerial').numberbox('getValue'),
            SESSION_ID: $('#txtSessionDetSerial').numberbox('getValue'),
            SESSION_DATE: $('#txtSessionDetDateG').datebox('getValue', 'yyyymmdd'),
            SESSION_STATUS: $('#SessionDetStatus').combobox('getValue'),
            BRANCH_SRL_ID: $('#ddlCaseBranch').combobox('getValue'),
            //THE_COURT: $('#txtSessionDetCourtName').textbox('getValue'),
            SESSION_DAY: $('#txtSessionDetDay').textbox('getValue'),
            SESSION_TIME: $('#txtSessionDetTime').textbox('getValue'),
            SESSION_NOTES: $('#txtSessionDetNotes').textbox('getValue'),
            IS_ATTEND: null
        },
        LawSessionEmps: empChangesDetList
    }

    showProgress('#SessionDetLayout', 'جاري حفظ البيانات ...');
    doAjax('LAW/Register/SaveLawSessionRequest', 'post', JSON.stringify(LawSessionReqData), 'application/json',
        function (result) {
            hideProgress('#SessionDetLayout');
            if (result.res > 0 && result.msg == '') {
                showAlert('تنبيه', 'تم الحفظ بنجاح.', 'success');
            } else {
                showAlert('خطأ', result.msg, 'error');
            }
            _LawEmpsSessionDetGrdDeletedRecords = [];
        },
        function (error) {
            hideProgress('#SessionDetLayout');
            showAlert('خطأ', error, 'error');
        });

}

function doDeleteSessionDet() {
    showConfirm('حذف سجل', 'سيتم حذف الجلسة <br/> رقم الجلسة (' + $('#txtSessionDetSerial').textbox('getValue') + ') ' + '<br/><div style="text-align:center;margin-top:10px;">هل أنت متأكد؟</div>',
        function (ok) {
            if (ok) {
                var EmpDgArrDet = [];
                var FileDgArrDet = [];
                if (_LawEmpsSessionDetGridEditIndex != undefined)
                    endEditingEmpsSessionDetRecordsData();

                var isNew = _LawCaseCurrentReqObj == null;
                var EmpRowsDet = $('#tblEmpsInSessionDet').datagrid('getRows');
                var FileRowsDet = $('#tblFilesInSessionDet').datagrid('getRows');
                for (var i = 0; i < EmpRowsDet.length; i++) {
                    EmpDgArrDet.push({ df: 'D', SESSION_ID: $('#txtSessionDetSerial').numberbox('getValue'), EMP_CODE: EmpRowsDet[i].EMP_CODE.split(' - ')[0], EmpName: EmpRowsDet[i].EMP_CODE.split(' - ')[1], SESSION_NOTE: EmpRowsDet[i].SESSION_NOTE });
                }
                _LawEmpsSessionDetGrdData = EmpDgArrDet;
                var empChangesDetList = _.filter(_LawEmpsSessionDetGrdData, function (sessionEmp) { return sessionEmp.df == 'D' });
                var LawSessionReqData = {
                    LawCaseReqDet: {
                        df: 'D',
                        CASE_YEAR: $('#CaseYear').numberbox('getValue'),
                        CASE_SERIAL: $('#CaseSerial').numberbox('getValue'),
                        SESSION_ID: $('#txtSessionDetSerial').numberbox('getValue'),
                        SESSION_DATE: $('#txtSessionDetDateG').datebox('getValue', 'yyyymmdd'),
                        SESSION_STATUS: $('#SessionDetStatus').combobox('getValue'),
                        BRANCH_SRL_ID: $('#ddlCaseBranch').combobox('getValue'),
                        THE_COURT: $('#txtSessionDetCourtName').textbox('getValue'),
                        SESSION_NOTES: $('#txtSessionDetNotes').textbox('getValue'),
                        IS_ATTEND: null
                    },
                    LawSessionEmps: empChangesDetList
                }

                showProgress('#SessionDetLayout', 'جاري حفظ البيانات ...');
                doAjax('LAW/Register/SaveLawSessionRequest', 'post', JSON.stringify(LawSessionReqData), 'application/json',
                    function (result) {
                        hideProgress('#SessionDetLayout');
                        if (result.res > 0 && result.msg == '') {
                            showAlert('تنبيه', 'تم حذف الجلسة بنجاح.', 'success');
                            refreshSessionDetailForm();
                        } else {
                            showAlert('خطأ', result.msg, 'error');
                        }
                        _LawEmpsSessionDetGrdDeletedRecords = [];
                    },
                    function (error) {
                        hideProgress('#SessionDetLayout');
                        showAlert('خطأ', error, 'error');
                    });
            }
        }
     );
}

function refreshSessionDetailForm() {
    debugger;
    //clear session detail form
    //clearSessionDetailForm();
    $('#dSessionDetailsForm').window('close');
    //refresh LawReg.cshtml with new session 
    showProgress('#divLawInfo', 'جاري تحميل البيانات ...');
    doAjax('LAW/Register/law_GetCaseInfoForReq?caseYear=' + $('#CaseYear').numberbox('getValue') + '&caseSrl=' + $('#CaseSerial').numberbox('getValue'), 'post', {}, 'application/json',
        function (result) {
            console.log(result);
            //result.LawReqMstr is for master data
            //load Session datagrid result.LawReqDetail
            hideProgress('#divLawInfo');
            $('#tblCaseSessions').datagrid('loadData', result.LawReqDetail);
        },
        function (error) {
            showAlert('خطأ', error, 'error');
        }
        );
}

function clearSessionDetailForm() {
    debugger;
    $('#txtSessionDetSerial').textbox('clear');
    $('#txtSessionDetDateH').hdatebox('clear');
    $('#txtSessionDetDateG').datebox('clear');
    $('#numOfCaseSessionDet').numberbox('clear');
    $('#SessionDetStatus').combobox('clear');
    $('#txtSessionDetCourtName').textbox('clear');
    $('#txtSessionDetNotes').textbox('clear');
    $('#tblEmpsInSessionDet').datagrid('loadData', []);
    $('#AttachmentTypeDet').combobox('clear');
    $('#tblFilesInSessionDet').datagrid('loadData', []);
    _LawEmpsSessionDetGrdData = [];  //empty session emp grid data
    _LawFilesSessionDetGrdData = []; //empty files grid data
}

function doAddEmpInSessionDet() {
    endEditingEmpsSessionDetRecordsData();
    if (_LawEmpsSessionDetGrdData.length == 0)
        $('#tblEmpsInSessionDet').datagrid('loadData', _LawEmpsSessionDetGrdData);
    var erId = _LawEmpsSessionDetGrdData.length + 1;
    $('#tblEmpsInSessionDet').datagrid('appendRow', { df: 'I', EmpName: '' });

    $('#tblEmpsInSessionDet').datagrid('selectRow', erId - 1).datagrid('beginEdit', erId - 1);
    _LawEmpsSessionDetGridEditIndex = erId - 1;

    var ed = $('#tblEmpsInSessionDet').datagrid('getEditor', { index: _LawEmpsSessionDetGridEditIndex, field: 'EMP_CODE' });

    if (ed) {
        $(ed.target).textbox('textbox').focus();
    }
    _LawEmpsSessionDetChangesObj.hasChanges = true;
}

function doDeleteEmpInSessionDet() {
    debugger;
    var currentRowData = $('#tblEmpsInSessionDet').datagrid('getSelected');

    _LawEmpsSessionDetEditingList = _.reject(_LawEmpsSessionDetEditingList, function (er) { er.eId == currentRowData.EmpName; });
    _LawEmpsSessionDetChangesObj.hasChanges = true;

    $('#tblEmpsInSessionDet').datagrid('deleteRow', _LawEmpsSessionDetGridEditIndex);
}

//************Add & Delete Files in Files Datagrid***************** 
function doAddAttachmentSessionDet() {
    debugger;
    if ($('#AttachmentTypeDet').combobox('getValue') == "") {
        showAlert('خطأ', 'الرجاء إختيار نوع المرفق', 'error');
    }
    else {
        //var file;
        var fileByteArray = [];
        var bytes = [];
        $('#FileDetInput').click();
        $('#FileDetInput').change(function (e) {
            debugger;
            console.log(e);
            var file = $('#FileDetInput')[0].files[0];
            console.log(file);
            var fileBlob = new Blob([file], { type: file.type });
            //***********************************
            let reader = new FileReader();
            reader.readAsArrayBuffer(fileBlob);
            //reader.readAsBinaryString(file);
            //reader.readAsText(file);
            console.log(reader);
            reader.onloadend = function (evt) {
                debugger;
                if (evt.target.readyState == FileReader.DONE) {
                    console.log(evt.target.result);
                    var arrayBuffer = evt.target.result,
                        array = new Uint8Array(arrayBuffer);
                    //fileByteArray = arrayBuffer;
                    for (var i = 0; i < array.length; i++) {
                        fileByteArray.push(array[i]);
                    }
                    console.log(fileByteArray);
                    SaveSessionDetailFile(file, fileByteArray);
                }
            };
            reader.onerror = function () {
                console.log(reader.error);
            };
            //***********************************
            //$('#tblFilesInSessionDet').datagrid('endEdit', _LawFilesSessionGridEditIndex);
            //if (file.size > 1024 * 1024)  //1MB = 1024 * 1024
            //{
            //    alert('حجم الملف المسموح هو MB1')
            //} else {
            //    AddFileRowInDet(file, fileByteArray);  //fileByteArray
            //}
            //$('#FileDetInput').unbind(); //removes event handlers so that .change() doesn't call itself again
            //$('#tblFilesInSessionDet').datagrid('endEdit', _LawFilesSessionDetGridEditIndex);
        });


        //var file = $('#FileDetInput')[0].files[0];
    }
}

function SaveSessionDetailFile(file, fileByteArray) {
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0');
    var yyyy = today.getFullYear();
    today = yyyy + '/' + mm + '/' + dd;
    var fileDataDet = new FormData(file_SessionDetailForm);
    for(var value of fileDataDet.values()) {
        file = value;
    }
    var sessionAtt = {
        DF: 'I',
        ATTACH_ID: null,
        ATTACH_DATE: null,
        ATTACH_NOTES: null
    }
    $("#SessionDetailfileForm").form('submit', {
        type: 'post',
        dataType: 'json',
        data: fileDataDet,
        url: 'LAW/Register/SaveFileDet?df=' + 'I'
                                         + '&CaseYear=' + $('#CaseYear').numberbox('getValue')
                                         + '&CaseSerial=' + $('#CaseSerial').numberbox('getValue')/*4*/
                                         + '&SessionId=' + $('#txtSessionDetSerial').textbox('getValue')/*2*/
                                         + '&AttachDate=' + today
                                         + '&AttachFileName=' + file.name
                                         + '&AttachFileType=' + file.type
                                         + '&AttachNote=' + null
                                         + '&AttachType=' + $('#AttachmentTypeDet').combobox('getValue'),
        success: function (result) {
            var response = JSON.parse(result);
            console.log(response);
            if (response.res > 0 && response.msg == '') {
                AddFileRowInDet(file, fileByteArray, response.AttachmentId);
                showAlert('تنبيه', 'تم حفظ المرفق بنجاح', 'success');
            } else {
                $('#FileDetInput').unbind();  //removes event handlers so that .change() doesn't call itself again
                showAlert('خطأ', response.msg, 'error');
            }
        }
    });
}

function doDeleteAttachmentSessionDet() {
    debugger;
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0');
    var yyyy = today.getFullYear();
    today = yyyy + '/' + mm + '/' + dd;

    var selRow = $('#tblFilesInSessionDet').datagrid('getSelected');
    console.log(selRow);
    $("#SessionDetailfileForm").form('submit', {
        type: 'post',
        dataType: 'json',
        data: {},
        url: 'LAW/Register/DeleteFileDet?df=' + 'D'
                                         + '&CaseYear=' + $('#CaseYear').numberbox('getValue')
                                         + '&CaseSerial=' + $('#CaseSerial').numberbox('getValue')/*4*/
                                         + '&SessionId=' + $('#txtSessionDetSerial').textbox('getValue')/*2*/
                                         + '&AttachId=' + selRow.ATTACH_ID
                                         + '&AttachDate=' + today,
        //+ '&AttachFileName=' + null
        //+ '&AttachFileType=' + null
        //+ '&AttachNote=' + null,
        //+ '&AttachType=' + 1/*$('#AttachmentTypeDet').combobox('getValue')*/,
        success: function (result) {
            var response = JSON.parse(result);
            console.log(response);
            if (response.res > 0 && response.msg == '') {
                var currentRowData = $('#tblFilesInSessionDet').datagrid('getSelected');
                _LawEmpsSessionDetGrdDeletedRecords.push(currentRowData);
                _LawFilesSessionDetEditingList = _.reject(_LawFilesSessionDetEditingList, function (er) { er.eId == currentRowData.EMP_ID; });
                _LawFilesSessionDetChangesObj.hasChanges = true;

                $('#tblFilesInSessionDet').datagrid('deleteRow', _LawFilesSessionDetGridEditIndex);

                showAlert('تنبيه', 'تم حذف المرفق بنجاح', 'success');
            } else {
                $('#FileDetInput').unbind();  //removes event handlers so that .change() doesn't call itself again
                showAlert('خطأ', response.msg, 'error');
            }
        }
    });


}

function AddFileRowInDet(file, fileByteArray, AttachId) {
    debugger;
    console.log(file);
    endEditingFilesSessionDetRecordsData();
    if (_LawFilesSessionDetGrdData.length == 0)
        $('#tblFilesInSessionDet').datagrid('loadData', _LawFilesSessionDetGrdData);
    var erId = _LawFilesSessionDetGrdData.length + 1;
    $('#tblFilesInSessionDet').datagrid('appendRow', { df: 'I', File: fileByteArray, ATTACH_ID: AttachId });  //fileByteArray

    $('#tblFilesInSessionDet').datagrid('selectRow', erId - 1).datagrid('beginEdit', erId - 1);
    _LawFilesSessionDetGridEditIndex = erId - 1;

    var ed = $('#tblFilesInSessionDet').datagrid('getEditor', { index: _LawFilesSessionDetGridEditIndex, field: 'File_Name' });
    var edFile = $('#tblFilesInSessionDet').datagrid('getEditor', { index: _LawFilesSessionDetGridEditIndex, field: 'File' });
    if (ed) {
        $(ed.target).textbox('textbox').focus();
        $(ed.target).textbox('setText', file.name);
        //$(edFile.target).validatebox().val(file);
    }
    _LawFilesSessionDetChangesObj.hasChanges = true;
    $('#FileDetInput').unbind();  //removes event handlers so that .change() doesn't call itself again
    $('#tblFilesInSessionDet').datagrid('endEdit', _LawFilesSessionDetGridEditIndex);
}

function endEditingEmpsSessionDetRecordsData() {
    if (_LawEmpsSessionDetGridEditIndex == undefined) { return true }
    if ($('#tblEmpsInSessionDet').datagrid('validateRow', _LawEmpsSessionDetGridEditIndex)) {
        $('#tblEmpsInSessionDet').datagrid('endEdit', _LawEmpsSessionDetGridEditIndex);
        _LawEmpsSessionDetGridEditIndex = undefined;
        return true;
    } else {
        return true;
    }
}

function endEditingFilesSessionDetRecordsData() {
    if (_LawFilesSessionDetGridEditIndex == undefined) { return true }
    if ($('#tblFilesInSessionDet').datagrid('validateRow', _LawFilesSessionDetGridEditIndex)) {
        $('#tblFilesInSessionDet').datagrid('endEdit', _LawFilesSessionDetGridEditIndex);
        _LawFilesSessionDetGridEditIndex = undefined;
        return true;
    } else {
        return true;
    }
}

//*************************Emps DataGrid Events**************************
function LawEmpInSessionDetailsGrdSelect(index, row) {
}

function LawEmpInSessionDetailsGrdOnBeginEdit(index, row) {
    _LawEmpsSessionDetCurrentGridRow = _.clone(row);
    _LawEmpsSessionDetGridEditIndex = index;

    _LawEmpsSessionDetGrdEditors = $(this).datagrid('getEditors', index);
    if (row.EmpName) {
        $(_LawEmpsSessionDetGrdEditors[0].target).textbox('setText', row.EmpName);
    }
    $(_LawEmpsSessionDetGrdEditors[0].target).textbox('textbox').focus();
    _LawEmpsSessionDetChangesObj.hasChanges = true;
}

function LawEmpInSessionDetailsGrdOnClickCell(index, field) {
    if (_LawEmpsSessionDetGridEditIndex != index) {
        if (endEditingEmpsSessionDetRecordsData()) {
            $(this).datagrid('selectRow', index).datagrid('beginEdit', index);
        } else {
            setTimeout(function () { $('#tblEmpsInSession').datagrid('selectRow', _LawEmpsSessionDetGridEditIndex) }, 0);
        }
    }
}

function LawEmpInSessionDetailsGrdOnEndEdit(index, row, changes) {
    if (changes && (row.EmpName != _LawEmpsSessionDetCurrentGridRow.EmpName)) {
        //_LawFilesSessionGrdData[index].df = 'U';
        _LawEmpsSessionDetChangesObj.hasChanges = true;
    }

    _LawEmpsSessionDetGrdEditors = [];
}

//***********************************************************************

//*************************Files DataGrid Events *************************
function LawFilesInSessionDetailsGrdSelect(index, row) {
}

function LawFilesInSessionDetailsGrdOnBeginEdit(index, row) {
    _LawFilesSessionDetCurrentGridRow = _.clone(row);
    _LawFilesSessionDetGridEditIndex = index;

    _LawFilesSessionDetGrdEditors = $(this).datagrid('getEditors', index);
    if (row.EMP_ID) {
        $(_LawFilesSessionDetGrdEditors[0].target).textbox('setText', row.EMP_ID);
    }
    $(_LawFilesSessionDetGrdEditors[0].target).textbox('textbox').focus();
    _LawFilesSessionDetChangesObj.hasChanges = true;
}

function LawFilesInSessionDetailsGrdOnClickCell(index, field) {
    if (_LawFilesSessionDetGridEditIndex != index) {
        if (endEditingFilesSessionDetRecordsData()) {
            $(this).datagrid('selectRow', index).datagrid('beginEdit', index);  //uncomment this if you need to begin edit on row
        } else {
            setTimeout(function () { $('#tblFilesInSessionDet').datagrid('selectRow', _LawFilesSessionDetGridEditIndex) }, 0);
        }
    }
}

function LawFilesInSessionDetailsGrdOnEndEdit(index, row, changes) {
    if (changes && (row.EMP_ID != _LawFilesSessionDetCurrentGridRow.EMP_ID)) {
        //_LawFilesSessionGrdData[index].df = 'U';
        _LawFilesSessionDetChangesObj.hasChanges = true;
    }

    _LawFilesSessionDetGrdEditors = [];
}
//*****************************************************************