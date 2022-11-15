//Emps Datagrid
var _LawEmpsSessionChangesObj, _LawEmpsSessionCurrentGridRow;
var _LawEmpsSessionGrdData = _LawEmpsSessionGrdEditors = _LawEmpsSessionGrdDeletedRecords = [];
var _LawEmpsSessionGridEditIndex = undefined;
var _LawEmpsSessionEditingList = []
//Files Datagrid
var _LawFilesSessionChangesObj, _LawFilesSessionCurrentGridRow;
var _LawFilesSessionGrdData = _LawFilesSessionGrdEditors = _LawFilesSessionGrdDeletedRecords = [];
var _LawFilesSessionGridEditIndex = undefined;
var _LawFilesSessionEditingList = [];
//var fileArr = [];
var x = 0;

var EmpLoader = function (param, success, error) {
    var q = param.q || '';
    if (q.length < 2) { return false }

    doAjax('HR/Common/SearchEmp?q=' + encodeURIComponent(q), 'post', {}, 'application/json',
        function (result) { success(result); }, function (error) { error.apply(this, arguments); }
    );
}

function addHijriIcon() {
    $('.easyui-hdatebox').next().find("a").addClass("hdateboxicon");
}

$(document).ready(function () {
    setTimeout(addHijriIcon, 1);
    $.fn.datebox.defaults.formatter = formatterG;
    $.fn.datebox.defaults.parser = parserG;

    //Files
    _LawFilesSessionChangesObj = _LawFilesSessionCurrentGridRow = null;
    _LawFilesSessionGrdData = _LawFilesSessionGrdEditors = _LawFilesSessionGrdDeletedRecords = [];

    _LawFilesSessionChangesObj = getCurrentFormChangesObj();
    _LawFilesSessionChangesObj = false;

    //Emps
    _LawEmpsSessionChangesObj = _LawEmpsSessionCurrentGridRow = null;
    _LawEmpsSessionGrdData = _LawEmpsSessionGrdEditors = _LawEmpsSessionGrdDeletedRecords = [];

    _LawEmpsSessionChangesObj = getCurrentFormChangesObj();
    _LawEmpsSessionChangesObj = false;
    //$.fn.filebox.defaults = $.extend({}, $.fn.textbox.defaults, {
    //    buttonIcon: null,
    //    buttonText: "إختر الملف",
    //    buttonAlign: "left",
    //    inputEvents: {},
    //    accept: "",
    //    separator: ",",
    //    multiple: true
    //});

    $('#ddlEmpName').combobox({
        mode: 'remote',
        loader: EmpLoader,
        selectOnNavigation: false,
        valueField: 'eId',
        textField: 'eNm',
        icons: [{
            iconCls: 'icon-clear',
            handler: function (e) {
                $(e.data.target).textbox('clear');
                if (_LawFilesSessionCurrentGridRow)
                    _LawFilesSessionChangesObj.hasChanges = true;
            }
        }]
    });

    //***FileBox***
    $('#fbSessionFileUpload').filebox({
        buttonText: 'إختر الملف',
        buttonAlign: 'left',
        //name: 'file_name',
        multiple: true,
        onChange: function (e) {
            debugger;
            UploadFiles(this, "fbSessionFileUpload", "AttachBox", "div_files");  //AttachBox
        }
    });
});

function TxtSessionYearHjChange(newVal, oldVal) {
    var wndId = this.id.split('_')[1];
    //if (!_LawCaseLoadingDataFromSearch) {
    if (_gsAppHDP.isValidStrDate(newVal)) {
        var gDateArr = _gsAppHDP.gDateFromStr(newVal).split('/');
        $('#txtSessionDateG').textbox('setValue', gDateArr[0] + '/' + gDateArr[1] + '/' + gDateArr[2]);
        //$('#txtSessionDateG').textbox('setValue', _gsAppHDP.gDateFromStr(newVal));            
    }
    // }
}

function TxtSessionYearGrgChange(nv, ov) {
    var wndId = this.id.split('_')[1];
    //if (!_LawCaseLoadingDataFromSearch) {
    if (nv != '' && nv != null) {
        //debugger;
        doAjax('Opr/Common/GetHijriDate?gDate=' + $('#txtSessionDateG').hdatebox('getValueF', 'yyyymmdd') + '&flag=' + 2, 'get', null, 'application/json',
             function (hDt) {
                 var y = hDt.substr(0, 4);
                 var m = hDt.substr(4, 2);
                 var d = hDt.substr(6, 2);
                 var hijDT = y + '/' + m + '/' + d;
                 //hideProgress('#divParticipationReq');
                 $('#txtSessionDateH').hdatebox('setValue', hijDT);
             },
             function (error) {
                 //hideProgress('#divParticipationReq');
                 showAlert('خطأ', error, 'error');
             }
         );
    }

    // }
}

function openSessionRegForm() {
    doAjax('LAW/Register/LoadFormLookups', 'get', {}, 'application/json',
        function (lookupData) {
            console.log(lookupData);
            $('#SessionStatus').combobox('loadData', lookupData.sStatus);
            $('#AttachmentType').combobox('loadData', lookupData.aType);

            $('#SessionStatus').combobox('setValue', 1);  //set value to جديدة
        },
        function (error) {
            showAlert('خطأ', error, 'error');
        });
}

function closeSessionRegForm() {
    debugger;
    //clear session form
    clearSessionRegForm();
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

function getFileBytesAsArray(file) {  //convert form file to byte array
    debugger;
    console.log(file);
    var reader = new FileReader();
    var fileByteArray = [];
    reader.readAsArrayBuffer(file);
    reader.onload = function (evt) {
        debugger;
        var b = reader.result;
        console.log(b);
        //if (evt.target.readyState == FileReader.DONE) {
        //    var arrayBuffer = evt.target.result,
        //        array = new Uint8Array(arrayBuffer);
        //    for (var i = 0; i < array.length; i++) {
        //        fileByteArray.push(array[i]);
        //    }
        //}
    }
    console.log(reader);
}

function doSaveNewSession() {
    debugger;
    var EmpDgArr = [];
    var FileDgArr = [];
    var fileBytes = [];
    if (_LawEmpsSessionGridEditIndex != undefined)
        endEditingEmpsSessionRecordsData();

    if (validateSessionReqData()) {
        //var isNew = _LawCaseCurrentReqObj == null;
        var isNew = $('#txtSessionSerial').textbox('getValue') == "";
        var EmpRows = $('#tblEmpsInSession').datagrid('getRows');
        var FileRows = $('#tblFilesInSession').datagrid('getRows');
        for (var i = 0; i < EmpRows.length; i++) {
            EmpDgArr.push({ df: 'I', SESSION_ID: $('#txtSessionSerial').numberbox('getValue'), EMP_CODE: EmpRows[i].EMP_CODE.split(' - ')[0], EmpName: EmpRows[i].EMP_CODE.split(' - ')[1], SESSION_NOTE: EmpRows[i].SESSION_NOTE });
        }
        _LawEmpsSessionGrdData = EmpDgArr;
        var empChangesList = _.filter(_LawEmpsSessionGrdData, function (sessionEmpDet) { return sessionEmpDet.df == 'I' || sessionEmpDet.df == 'U' });
        for (var i = 0; i < FileRows.length; i++) {
            //fileBytes.push(FileRows[i].File);
            //file_name = new FormData(FileRows[i].File);
            FileDgArr.push({
                df: 'I', SESSION_ID: $('#txtSessionSerial').numberbox('getValue'), ATTACH_ID: FileRows[i].ATTACH_ID,
                ATTACH_DATE: $('#txtSessionDateG').datebox('getValue', 'yyyymmdd'), FileBytes: FileRows[i].File, ATTACH_FILE: null, ATTACH_NOTES: ''
            });
        }
        _LawFilesSessionGrdData = FileDgArr;
        var fileChangesList = _.filter(_LawFilesSessionGrdData, function (sessionFileDet) { return sessionFileDet.df == 'I' || sessionFileDet.df == 'U' });
        var LawSessionReqData = {
            LawCaseReqDet: {
                df: isNew ? 'I' : 'U',
                CASE_YEAR: $('#CaseYear').numberbox('getValue'),
                CASE_SERIAL: $('#CaseSerial').numberbox('getValue'),
                SESSION_ID: $('#txtSessionSerial').numberbox('getValue'),
                SESSION_DATE: $('#txtSessionDateG').datebox('getValue', 'yyyymmdd'),
                SESSION_STATUS: $('#SessionStatus').combobox('getValue'),
                BRANCH_SRL_ID: $('#ddlCaseBranch').combobox('getValue'),
                //THE_COURT: $('#txtCourtName').textbox('getValue'),    /* 6-12-2022 - Added Session time instead of the court */  
                SESSION_DAY: $('#txtSessionDay').textbox('getValue'),
                SESSION_TIME: $('#txtSessionTime').textbox('getValue'),
                SESSION_NOTES: $('#txtNotes').textbox('getValue'),
                IS_ATTEND: null
            },
            //fileBytes: fileBytes,
            LawSessionEmps: empChangesList
            //LawSessionReqAtt: fileChangesList
        }

        showProgress('#SessionRegLayout', 'جاري حفظ البيانات ...');
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
    }
}

function validateSessionReqData() {
    //do some session validation
    var isValid = true;
    if ($('#txtSessionDateH').hdatebox('getValue') == '' || $('#txtSessionDateG').datebox('getValue') == '') {
        showAlert('بيانات مطلوبة', ' الرجاء تحديد تاريخ الجلسة ', 'error', function () { $('#txtSessionDateH').hdatebox('textbox').focus(); });
        isValid = false;
    }
    return isValid;
}

function UploadFiles(_obj, file_ctrlname, guid_ctrlname, div_files) {
    debugger;
    var value = $("#" + file_ctrlname).filebox('getValue');
    var files = $("#" + file_ctrlname).next().find('input[type=file]')[0].files;
    //var file = value;
    // var form = $('#fForm');
    console.log(files);

    var guid = $("#" + guid_ctrlname).val();
    if (value && files[0]) {
        var file_name = new FormData(file_Form);
        //formData.append("folder", "Data import File");
        //formData.append("guid", guid);
        //file_name.append("Filedata", files[0]);

        console.log("formData: " + file_name);
        //for (var i = 0; i < files.length; i++) {
        //    formData.append('Filedata', files[i]);
        //}
        for(var value of file_name.values()) {
            console.log(value);
        //fileArr.push(value);
            file = value;
        }
        file_name.forEach((value, key) => {
            console.log("key %s: value %s", key, value);
        })
        //console.log(options);
        //doAjax to upload file 
        //doAjax('LAW/Register/UploadFile', 'post', JSON.stringify(file_name), 'application/json',
        //function (result) {
        //    console.log(result);
        //},
        //function (error) {
        //    showAlert('خطأ', error, 'error');
        //}
        //);
        $("#fileForm").form('submit', {
            type: 'post',
            dataType: 'json',
            data: file_name,
            url: 'LAW/Register/UploadFile',
            success: function (result) {
                var data = eval('(' + result + ')');
                console.log(result);
            }
        });
        //$('#fileForm').form({
        //    url: 'LAW/Register/UploadFile',
        //    ajax: 'true',
        //    iframe: 'false',
        //    //onProgress: function (percent) { },
        //    success: function (data) {
        //    },
        //    onLoadError: function (error) {
        //        showAlert('خطأ', error, 'error');
        //    }
        //});
        //console.log(fileArr);
        ShowUpFiles(file);
    }
}

function ShowUpFiles(file) {
    debugger;
    $('#FilesGrid').append("<th style='font-weight: normal;color:dimgray'>" + file.name + "</th>");
    //showFileProgressByProgressBar();
    ShowFileProgressByPercentage();
}

function showFileProgressByProgressBar() {  //show progress in easyui-progressbar
    debugger;
    var val = $('#progress').progressbar('getValue');
    if (val < 100) {
        val += Math.floor(Math.random() * 10);

        $('#progress').css('display', 'block');
        $('#progress').progressbar('setValue', val);

        setTimeout(arguments.callee, 200);
    } else if (val = 100) {  //after progress completion
        $('#progress').css('display', 'none');

        //$('#div_files').append("<a href='javascript:void(0)' id='removeBtn' class='easyui-linkbutton' iconCls='icon-clear' onclick='DeleteFile(this)'>"
        //    + "<img src='jqplugins/eui/themes/icons/clear.png' width='12' height='12'></a>");
        $('#FilesGrid').append(
            "<th>" +
            "<a href='javascript:void(0)' id='removeBtn' class='easyui-linkbutton' iconCls='icon-clear' onclick='DeleteFile(this)' style='font-size:15px;text-decoration:none;'>&times;</a>" +
            "</th>");
        //$('#fileName').after("<img src='jqplugins/eui/themes/icons/ok.png' width='12' height='12'>");

        $('#progress').progressbar('setValue', 0);  //reset progress bar to 0
    }
}

function ShowFileProgressByPercentage() {  //show progress by percentage %
    debugger;
    var xhr = new XMLHttpRequest();
    //var x = document.getElementById("progressCounter").innerHTML;
    var ctr = document.getElementById("progressCounter").innerHTML.split('%')[0];
    if (x < 100) {
        x += Math.floor(Math.random() * 10);
        $('#progressCounter').css('display', 'block');
        //if x exceeds 100 set it to 100
        x > 100 ? document.getElementById("progressCounter").innerHTML = 100 + "%" : document.getElementById("progressCounter").innerHTML = x + "%";
        setTimeout(arguments.callee, 200);
    } else if (x = 100) {  //after progress completion
        //$('#div_files').append("<a href='javascript:void(0)' id='removeBtn' class='easyui-linkbutton' iconCls='icon-clear' onclick='DeleteFile(this)'>"
        //    + "<img src='jqplugins/eui/themes/icons/clear.png' width='12' height='12'></a>");

        $('#FilesGrid').append(
            "<th>" +
            "<a href='javascript:void(0)' id='removeBtn' class='easyui-linkbutton' iconCls='icon-clear' onclick='DeleteFile(this)' style='font-size:15px;text-decoration:none;'>&times;</a>" +
            "</th>");

        var i = $('#FilesGrid').find('th').length; //get length of #FilesGrid th
        $($('#FilesGrid').find('th')[i - 2]).css('color', 'black')   //Set color of filename before "x" button
        $('#progressCounter').css('display', 'none');
        //document.getElementById("progressCounter").innerHTML = 0;
        x = 0;
    }
}

function DeleteFile(btn) {
    debugger;
    console.log(btn);
    showConfirm('حذف مرفق', 'سيتم حذف المرفق  <br/> ' + '<br/><div style="text-align:center;margin-top:10px;">هل أنت متأكد؟</div>',
        function (ok) {
            if (ok) { //delete file from server or database
                debugger;
                $('#FilesGrid').find(btn.parentNode.previousElementSibling).remove();
                $(btn.parentNode).remove();  //removes "x" button
                //showAlert('تنبيه', 'تم الحذف بنجاح.', 'success', function (r) {});
            }
        }
        );
}

function reloadPage() {
    showProgress('#divLawInfo', 'جاري تحميل البيانات ...');
    doAjax('LAW/Register/LawReg', 'post', {}, 'application/json', function (result) {
        console.log(result);
        hideProgress('#divLawInfo');
    },
    function (error) {
        showAlert('خطأ', error, 'error');
    });
}

function OnChangeFileBox(value) {
    debugger;
    var files = $('#fbSessionFiles').filebox();
    console.log(files);
}
//************Add & Delete Emps in Emps Datagrid******************
function doAddEmpInSession() {
    debugger;
    endEditingEmpsSessionRecordsData();
    if (_LawEmpsSessionGrdData.length == 0)
        $('#tblEmpsInSession').datagrid('loadData', _LawEmpsSessionGrdData);
    var erId = _LawEmpsSessionGrdData.length + 1;
    $('#tblEmpsInSession').datagrid('appendRow', { df: 'I', EMP_CODE: '', SESSION_NOTE: '' });

    $('#tblEmpsInSession').datagrid('selectRow', erId - 1).datagrid('beginEdit', erId - 1);
    _LawEmpsSessionGridEditIndex = erId - 1;

    var ed = $('#tblEmpsInSession').datagrid('getEditor', { index: _LawEmpsSessionGridEditIndex, field: 'EMP_CODE' });

    if (ed) {
        $(ed.target).textbox('textbox').focus();
    }
    _LawEmpsSessionChangesObj.hasChanges = true;
}

function doDeleteEmpInSession() {
    debugger;
    var currentRowData = $('#tblEmpsInSession').datagrid('getSelected');

    _LawEmpsSessionEditingList = _.reject(_LawEmpsSessionEditingList, function (er) { er.eId == currentRowData.EmpName; });
    _LawEmpsSessionChangesObj.hasChanges = true;

    $('#tblEmpsInSession').datagrid('deleteRow', _LawEmpsSessionGridEditIndex);
}
//***************************END**********************************

//************Add & Delete Files in Files Datagrid***************** 
function doAddAttachmentSession() {
    debugger;
    if ($('#txtSessionSerial').textbox('getValue') == "") {  //check if session is saved first
        showAlert('خطأ', 'الرجاء حفظ بيانات الجلسة أولا', 'error');
    } else if ($('#AttachmentType').combobox('getValue') == "") {
        showAlert('خطأ', 'الرجاء إختيار نوع المرفق', 'error');
    }
    else {
        $('#FileInput').unbind(); //removes event handlers so that .change() doesn't call itself again
        //var file;
        var fileByteArray = [];
        var bytes = [];
        $('#FileInput').click();
        $('#FileInput').change(function (e) {
            debugger;
            console.log(e);
            var file = $('#FileInput')[0].files[0];
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
                    SaveSessionFile(file, fileByteArray);
                }
            };
            reader.onerror = function () {
                console.log(reader.error);
            };

        });


        //var file = $('#FileInput')[0].files[0];

        //******************************************
        //endEditingFilesSessionRecordsData();
        //if (_LawFilesSessionGrdData.length == 0)
        //    $('#tblFilesInSession').datagrid('loadData', _LawFilesSessionGrdData);
        //var erId = _LawFilesSessionGrdData.length + 1;
        //$('#tblFilesInSession').datagrid('appendRow', { df: 'I' });

        //$('#tblFilesInSession').datagrid('selectRow', erId - 1).datagrid('beginEdit', erId - 1);
        //_LawFilesSessionGridEditIndex = erId - 1;

        //var ed = $('#tblFilesInSession').datagrid('getEditor', { index: _LawFilesSessionGridEditIndex, field: 'SRL_ID' });
        //if (ed) {
        //    $(ed.target).textbox('textbox').focus();
        //    $(ed.target).textbox('setText', file.name);
        //}
        //_LawFilesSessionChangesObj.hasChanges = true;
    }
}

function doDeleteAttachmentSession() {
    debugger;
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0');
    var yyyy = today.getFullYear();
    today = yyyy + '/' + mm + '/' + dd;

    var selRow = $('#tblFilesInSession').datagrid('getSelected');
    $("#SessionfileForm").form('submit', {
        type: 'post',
        dataType: 'json',
        data: {},
        url: 'LAW/Register/DeleteFile?df=' + 'D'
                                         + '&CaseYear=' + $('#CaseYear').numberbox('getValue')
                                         + '&CaseSerial=' + $('#CaseSerial').numberbox('getValue')/*4*/
                                         + '&SessionId=' + $('#txtSessionSerial').textbox('getValue')/*2*/
                                         + '&AttachId=' + selRow.ATTACH_ID
                                         + '&AttachDate=' + today,
        //+ '&AttachFileName=' + null
        //+ '&AttachFileType=' + null
        //+ '&AttachNote=' + null,
        //+ '&AttachType=' + 1/*$('#AttachmentTypeDet').combobox('getValue')*/,
        success: function (result) {
            var response = JSON.parse(result);
            if (response.res > 0 && response.msg == '') {
                var currentRowData = $('#tblFilesInSession').datagrid('getSelected');
                _LawEmpsSessionGrdDeletedRecords.push(currentRowData);
                _LawFilesSessionEditingList = _.reject(_LawFilesSessionEditingList, function (er) { er.eId == currentRowData.EMP_ID; });
                _LawFilesSessionChangesObj.hasChanges = true;

                $('#tblFilesInSession').datagrid('deleteRow', _LawFilesSessionGridEditIndex);

                showAlert('تنبيه', 'تم حذف المرفق بنجاح', 'success');
            } else {
                $('#FileInput').unbind();  //removes event handlers so that .change() doesn't call itself again
                showAlert('خطأ', response.msg, 'error');
            }
        }
    });


    //var currentRowData = $('#tblFilesInSession').datagrid('getSelected');
    //_LawEmpsSessionGrdDeletedRecords.push(currentRowData);
    //_LawFilesSessionEditingList = _.reject(_LawFilesSessionEditingList, function (er) { er.eId == currentRowData.EMP_ID; });
    //_LawFilesSessionChangesObj.hasChanges = true;

    //$('#tblFilesInSession').datagrid('deleteRow', _LawFilesSessionGridEditIndex);
}

function SaveSessionFile(file, fileByteArray) {
    debugger;
    var SessionFile = {
        CaseYear: $('#CaseYear').numberbox('getValue'),
        CaseSerial: $('#CaseSerial').numberbox('getValue'),
        SessionId: $('#txtSessionSerial').textbox('getValue'),
        FileInInt: fileByteArray,  //send array of int to convert it to byte[] in controller
        AttachFile: null,
        SessionAttach: {
            DF: 'I',
            ATTACH_ID: null,
            ATTACH_DATE: null,
            ATTACH_FILE_NAME: file.name,
            ATTACH_FILE_TYPE: file.type,
            ATTACH_NOTES: 'Testing2'
        }
    }
    //doAjax('LAW/Register/SaveSessionFile', 'post', JSON.stringify(SessionFile), 'application/json', function (result) {
    //    console.log(result);
    //    if (result.res > 0 && result.msg == '') {
    //        AddFileRow(file, fileByteArray);
    //        showAlert('تنبيه', 'تم حفظ المرفق بنجاح', 'success');
    //    } else {
    //        showAlert('خطأ', result.msg, 'error');
    //    }
    //},
    //function (error) {
    //    showAlert('خطأ', error, 'error');
    //});
    //###############################################################################
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0');
    var yyyy = today.getFullYear();
    today = yyyy + '/' + mm + '/' + dd;
    var fileData = new FormData(file_SessionForm);
    for(var value of fileData.values()) {
        console.log(value);
        file = value;
    }
    var sessionAtt = {
        DF: 'I',
        ATTACH_ID: null,
        ATTACH_DATE: null,
        ATTACH_NOTES: null
    }
    $("#SessionfileForm").form('submit', {
        type: 'post',
        dataType: 'json',
        data: fileData,
        url: 'LAW/Register/SaveFile?df=' + 'I'
                                         + '&CaseYear=' + $('#CaseYear').numberbox('getValue')
                                         + '&CaseSerial=' + $('#CaseSerial').numberbox('getValue')/*4*/
                                         + '&SessionId=' + $('#txtSessionSerial').textbox('getValue')/*2*/
                                         + '&AttachDate=' + today
                                         + '&AttachFileName=' + file.name
                                         + '&AttachFileType=' + file.type
                                         + '&AttachNote=' + null
                                         + '&AttachType=' + $('#AttachmentType').combobox('getValue'),
        success: function (result) {
            var response = JSON.parse(result);
            console.log(response);
            if (response.res > 0 && response.msg == '') {
                AddFileRow(file, fileByteArray, response.AttachmentId);
                showAlert('تنبيه', 'تم حفظ المرفق بنجاح', 'success');
            } else {
                $('#FileInput').unbind(); //removes event handlers so that .change() doesn't call itself again
                showAlert('خطأ', response.msg, 'error');
            }
        }
    });
}

function AddFileRow(file, fileByteArray, AttachId) {
    debugger;
    console.log(file);
    endEditingFilesSessionRecordsData();
    if (_LawFilesSessionGrdData.length == 0)
        $('#tblFilesInSession').datagrid('loadData', _LawFilesSessionGrdData);
    var erId = _LawFilesSessionGrdData.length + 1;
    $('#tblFilesInSession').datagrid('appendRow', { df: 'I', File: fileByteArray, ATTACH_ID: AttachId });  //fileByteArray

    $('#tblFilesInSession').datagrid('selectRow', erId - 1).datagrid('beginEdit', erId - 1);
    _LawFilesSessionGridEditIndex = erId - 1;

    var ed = $('#tblFilesInSession').datagrid('getEditor', { index: _LawFilesSessionGridEditIndex, field: 'File_Name' });
    var edFile = $('#tblFilesInSession').datagrid('getEditor', { index: _LawFilesSessionGridEditIndex, field: 'File' });
    if (ed) {
        $(ed.target).textbox('textbox').focus();
        $(ed.target).textbox('setText', file.name);
        //$(edFile.target).validatebox().val(file);
    }
    _LawFilesSessionChangesObj.hasChanges = true;
    //clear AttachmentType combobox
    $('#AttachmentType').combobox('clear');
    $('#FileInput').unbind(); //removes event handlers so that .change() doesn't call itself again
    $('#tblFilesInSession').datagrid('endEdit', _LawFilesSessionGridEditIndex);
}

function clearSessionRegForm() {
    debugger;
    var rows = $('#tblEmpsInSession').datagrid('getRows');
    if (rows.length > 0) {
        for (var i = rows.length - 1; i >= 0; i--) {
            var index = $('#tblEmpsInSession').datagrid('getRowIndex', rows[i]);
            $('#tblEmpsInSession').datagrid('deleteRow', index);
        }
    }
    $('#txtSessionSerial').textbox('clear');
    $('#txtSessionDateH').hdatebox('clear');
    $('#txtSessionDateG').datebox('clear');
    $('#numOfCase').numberbox('clear');
    $('#SessionStatus').combobox('clear');
    $('#txtCourtName').textbox('clear');
    $('#txtNotes').textbox('clear');
    //$('#tblEmpsInSession').datagrid('loadData', []);
    $('#AttachmentType').combobox('clear');
    $('#tblFilesInSession').datagrid('loadData', []);
    _LawEmpsSessionGrdData = [];  //empty session emp grid data
    _LawFilesSessionGrdData = []; //empty files grid data
}

//*************************END*****************************************
function endEditingEmpsSessionRecordsData() {
    debugger;
    if (_LawEmpsSessionGridEditIndex == undefined) { return true }
    if ($('#tblEmpsInSession').datagrid('validateRow', _LawEmpsSessionGridEditIndex)) {
        $('#tblEmpsInSession').datagrid('endEdit', _LawEmpsSessionGridEditIndex);
        _LawEmpsSessionGridEditIndex = undefined;
        return true;
    } else {
        return false;
    }
}

function endEditingFilesSessionRecordsData() {
    if (_LawFilesSessionGridEditIndex == undefined) { return true }
    if ($('#tblFilesInSession').datagrid('validateRow', _LawFilesSessionGridEditIndex)) {
        $('#tblFilesInSession').datagrid('endEdit', _LawFilesSessionGridEditIndex);
        _LawFilesSessionGridEditIndex = undefined;
        return true;
    } else {
        return true;
    }
}

function lawEmpDetEmpFormatter(value, row, index) {
    debugger;
    if (row.EMP_CODE)
        return row.EMP_CODE + ' - ' + row.EMPNAME;
    else
        return value;
}
//*************************Emps DataGrid Events**************************
function LawEmpInSessionDetGrdSelect(index, row) {
}

function LawEmpInSessionDetGrdOnBeginEdit(index, row) {
    debugger;
    _LawEmpsSessionCurrentGridRow = _.clone(row);
    _LawEmpsSessionGridEditIndex = index;

    _LawEmpsSessionGrdEditors = $(this).datagrid('getEditors', index);
    if (row.EMP_CODE) {
        $(_LawEmpsSessionGrdEditors[0].target).combobox('setText', row.EMP_CODE);
    }
    $(_LawEmpsSessionGrdEditors[0].target).combobox('textbox').focus();
    _LawEmpsSessionChangesObj.hasChanges = true;
}

function LawEmpInSessionDetGrdOnClickCell(index, field) {
    debugger;
    if (_LawEmpsSessionGridEditIndex != index) {
        if (endEditingEmpsSessionRecordsData()) {
            _LawEmpsSessionGridEditIndex = index;
            $(this).datagrid('selectRow', index).datagrid('beginEdit', index);
        } else {
            setTimeout(function () { $('#tblEmpsInSession').datagrid('selectRow', _LawEmpsSessionGridEditIndex) }, 0);
        }
    }
}

function LawEmpInSessionDetGrdOnEndEdit(index, row, changes) {
    debugger;
    if (changes && (row.EmpName != _LawEmpsSessionCurrentGridRow.EmpName)) {
        //_LawFilesSessionGrdData[index].df = 'U';
        _LawEmpsSessionChangesObj.hasChanges = true;
    }

    _LawEmpsSessionGrdEditors = [];
}

//***********************************************************************

//*************************Files DataGrid Events *************************
function LawFilesInSessionDetGrdSelect(index, row) {
}

function LawFilesInSessionDetGrdOnBeginEdit(index, row) {
    _LawFilesSessionCurrentGridRow = _.clone(row);
    _LawFilesSessionGridEditIndex = index;

    _LawFilesSessionGrdEditors = $(this).datagrid('getEditors', index);
    if (row.EMP_ID) {
        $(_LawFilesSessionGrdEditors[0].target).textbox('setText', row.EMP_ID);
    }
    $(_LawFilesSessionGrdEditors[0].target).textbox('textbox').focus();
    _LawFilesSessionChangesObj.hasChanges = true;
}

function LawFilesInSessionDetGrdOnClickCell(index, field) {
    if (_LawFilesSessionGridEditIndex != index) {
        if (endEditingFilesSessionRecordsData()) {
            $(this).datagrid('selectRow', index).datagrid('beginEdit', index);
        } else {
            setTimeout(function () { $('#tblFilesInSession').datagrid('selectRow', _LawFilesSessionGridEditIndex) }, 0);
        }
    }
}

function LawFilesInSessionDetGrdOnEndEdit(index, row, changes) {
    if (changes && (row.EMP_ID != _LawFilesSessionCurrentGridRow.EMP_ID)) {
        //_LawFilesSessionGrdData[index].df = 'U';
        _LawFilesSessionChangesObj.hasChanges = true;
    }

    _LawFilesSessionGrdEditors = [];
}
//*****************************************************************