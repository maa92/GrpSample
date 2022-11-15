var _LawCaseDetGridData = [], _LawCaseDetGridDeletedData = [], _LawCaseEditingList = [];
var _isIREmpDetGridInEdit = false, _gridLawCaseDetEditIndex = undefined, _LawCaseDataEditors;
var _isLawCaseDetDataLoading = _LawCaseLoadingDataFromSearch = false;
var _LawCaseChangesObj, _LawCaseCurrentReqObj;
var _PrevCaes;

var lawEmpLoader = function (param, success, error) {
    var q = param.q || '';
    if (q.length < 2) { return false }

    doAjax('LAW/Register/law_SearchEmp?q=' + encodeURIComponent(q), 'post', {}, 'application/json',
        function (result) { success(result); }, function (error) { error.apply(this, arguments); }
    );
}

function addHijriIcon() {
    $('.easyui-hdatebox').next().find("a").addClass("hdateboxicon");
}

function LawSessionDetailsFormatter(value, row, index) {
    return '<a href="javascript:{}" class="easyui-linkbutton" onclick="showLawSessionDetails(this);">تفاصيل</a>';
}

function SessionDetailsPrintSessionFormatter() {
    return '<a href="javascript:{}" class="easyui-linkbutton" data-options="iconCls:"icon-add",width:100,height:50" onclick="doPrintSessionDetailsReport(this);">طباعة</a>';
}

function SessionDetailsSendEmailToMgrFormatter() {
    return '<a href="javascript:{}" class="easyui-linkbutton" data-options="iconCls:"icon-add",width:100,height:50" onclick="doSendSessionDetailsToMgr(this);">إرسال</a>';
}

function showLawSessionDetails(rowBtn) {
    debugger;
    $('#tblCaseSessions').datagrid('selectRow', $(rowBtn).closest('tr').index());
    var selRow = $('#tblCaseSessions').datagrid('getSelected');
    if (selRow) {
        $('#dSessionDetailsForm').window({ title: 'تفاصيل الجلسة' + ' : ' + selRow.SESSION_ID });
        $('#dSessionDetailsForm').window('open');
        //$('#dSessionDetailsForm').window('setTitle', 'تفاصيل الجلسة: ' + selRow.SESSION_ID);
        //$('#dSessionDetailsForm').window('options').title = 'تفاصيل الجلسة' + ' - ' + selRow.SESSION_ID;
        //$('#wndLawSessionDetailInfo').window('refresh',
        //                                    'LAW/Register/SessionDetails?CaseYear='
        //                                    + $('#CaseYear').numberbox('getValue') 
        //                                    + '&CaseSerial=' + 4/*$('#txtLawCaseSerial').textbox('getValue')*/
        //                                    + '&SessionId=' + 2/*selRow.SessionId*/);
    }
}

function doPrintSessionDetailsReport(rowBtn) {
    //TODO: print Session Details report
}

function doSendSessionDetailsToMgr(rowBtn) {
    //TODO: send email to mgr
}

function LawCaseSaveChangesBeforeClose(afterSaveCB) {
    SaveLawCaseMasterData(afterSaveCB);
}

$(document).ready(function () {
    setTimeout(addHijriIcon, 1);
    $.fn.datebox.defaults.formatter = formatterG;
    $.fn.datebox.defaults.parser = parserG;

    _PrevCaes = [];
    _LawCaseChangesObj = getCurrentFormChangesObj();
    _LawCaseChangesObj = false;
    //if ($('#CaseSerial').numberbox('getValue') == "") {
    //    $('#btnAddSession').linkbutton('disable');
    //}


    $('#divJudgmentStatusOthers').hide(); //hide others textbox for others in judgment status


    //*******نوع المحكمة - events************
    $('#rbCourtType1').click(function () {  //إبتدائية
        $('#txtLawCaseAppealSerial').textbox({ editable: false, required: false });
        $('#txtLawCaseHighCSerial').textbox({ editable: false, required: false });
        $('#txtLawCaseSerial').textbox({ editable: true, required: true });
    });

    $('#rbCourtType2').click(function () {  //إستئنافية
        $('#txtLawCaseHighCSerial').textbox({ editable: false, required: false });
        $('#txtLawCaseAppealSerial').textbox({ editable: true, required: true });
        $('#txtLawCaseSerial').textbox({ editable: true, required: true });
    });

    $('#rbCourtType3').click(function () {  //عليا
        $('#txtLawCaseAppealSerial').textbox({ editable: true, required: true });
        $('#txtLawCaseHighCSerial').textbox({ editable: true, required: true });
        $('#txtLawCaseSerial').textbox({ editable: true, required: true });
    });

    //*********END******************************

    //*************قطاع المحكمة - events*******
    $('#rbLawCourtDistrict1').click(function () {  //ديوان المظالم
        $('#ddlCourtName1').combobox({ editable: true, required: true });
        $('#ddlCourtName1').combobox('enable');
        $('#ddlCourtCity1').combobox({ editable: true, required: true });
        $('#ddlCourtCity1').combobox('enable');
        $('#txtDistrictName1').textbox({editable: true});


        $('#ddlCourtName2').combobox({ editable: false, required: false });
        $('#ddlCourtName2').combobox('disable');

        $('#ddlCourtCity2').combobox({ editable: false, required: false });
        $('#ddlCourtCity2').combobox('disable');

        //$('#txtDistrictName').textbox({ editable: false, required: false });
        $('#txtDistrictName2').textbox({ editable: false });
        $('#txtCourtOthers').textbox({ editable: false, required: false });

    });

    $('#rbLawCourtDistrict2').click(function () {  //وزارة العدل
        $('#ddlCourtName2').combobox({ editable: true, required: true });
        $('#ddlCourtName2').combobox('enable');
        $('#ddlCourtCity2').combobox({ editable: true, required: true });
        $('#ddlCourtCity2').combobox('enable');
        $('#txtDistrictName2').textbox({ editable: true });

        $('#ddlCourtName1').combobox({ editable: false, required: false });
        $('#ddlCourtName1').combobox('disable');

        $('#ddlCourtCity1').combobox({ editable: false, required: false });
        $('#ddlCourtCity1').combobox('disable');

        //$('#txtDistrictName').textbox({ editable: false, required: false });
        $('#txtDistrictName1').textbox({ editable: false });
        $('#txtCourtOthers').textbox({ editable: false, required: false });
    });

    $('#rbLawCourtDistrict3').click(function () {  //أخرى
        $('#txtCourtOthers').textbox({ editable: true, required: true });

        $('#ddlCourtName1').combobox({ editable: false, required: false });
        $('#ddlCourtName2').combobox('disable');

        $('#ddlCourtCity1').combobox({ editable: false, required: false });
        $('#ddlCourtCity1').combobox('disable');

        $('#ddlCourtName2').combobox({ editable: false, required: false });
        $('#ddlCourtName2').combobox('disable');

        $('#ddlCourtCity2').combobox({ editable: false, required: false });
        $('#ddlCourtCity2').combobox('disable');

        //$('#txtDistrictName').textbox({ editable: false, required: false });
        $('#txtDistrictName1').textbox({ editable: false });
        $('#txtDistrictName2').textbox({ editable: false });

    });

    $('#rbLawCourtDistrict4').click(function () {   //الدائرة
        $('#txtDistrictName').textbox({ editable: true, required: true });

        $('#ddlCourtName1').combobox({ editable: false, required: false });
        $('#ddlCourtName2').combobox('disable');

        $('#ddlCourtCity1').combobox({ editable: false, required: false });
        $('#ddlCourtCity1').combobox('disable');

        $('#ddlCourtName2').combobox({ editable: false, required: false });
        $('#ddlCourtName2').combobox('disable');

        $('#ddlCourtCity2').combobox({ editable: false, required: false });
        $('#ddlCourtCity2').combobox('disable');

        $('#txtCourtOthers').textbox({ editable: false, required: false });
    });
    //******************END**************************

    $('#txtNameOfSrcaRep').combobox({
        mode: 'remote',
        loader: lawEmpLoader,
        selectOnNavigation: false,
        valueField: 'eId',
        textField: 'eNm',
        icons: [{
            iconCls: 'icon-clear',
            handler: function (e) {
                $(e.data.target).textbox('clear');
                if (_LawCaseDetGridData)
                    _LawCaseChangesObj.hasChanges = true;
            }
        }]
    });

    $('#txtNameOfAccusedOrDefendant').combobox({
        mode: 'remote',
        loader: lawEmpLoader,
        valueField: 'eId',
        textField: 'eNm',
        icons: [{
            iconCls: 'icon-clear',
            handler: function (e) {
                $(e.data.target).textbox('clear');
                if (_LawCaseDetGridData)
                    _LawCaseChangesObj.hasChanges = true;
            }
        }]
    });

    //************حالة الحكم**************
    $('#ddlRulingStatus').combobox({
        onSelect: function () {
            if ($('#ddlRulingStatus').combobox('getValue') == '28') {  //show others textbox
                $('#lblJudgmentStatusOthers').show();
                $('#divJudgmentStatusOthers').show();
            } else {
                $('#lblJudgmentStatusOthers').hide();
                $('#divJudgmentStatusOthers').hide();
            }
        }
    });
    //************END*********************

    //***********إختيار قضية سابقة******
    doAjax('LAW/Register/law_GetPrevCases', 'get', {}, 'application/json',
        function (result) {
            _PrevCaes = result;
            console.log(_PrevCaes);
            $('#ddlCgPrevCases').combogrid('grid').datagrid('loadData', result);
        },
        function (error) {
            showAlert('خطأ', result.msg, 'error');
        });
    $('#ddlCgPrevCases').combogrid({
        required: true,
        selectOnNavigation: true,
        idField: 'id',
        textField: 'cAndt',
        columns: [[
            { field: 'CaseYr', title: 'السنة', width: 50 },
            { field: 'cSerial', title: 'المسلسل', width: 60 },
            { field:'CaseNo', title:'رقم القضية', width: 155 }
        ]],
        onSelect: function (record) { 
            var caseSrl = $('#ddlCgPrevCases').combogrid('grid').datagrid('getSelected')['id'].split('/')[0];
            var caseYear = $('#ddlCgPrevCases').combogrid('grid').datagrid('getSelected')['CaseYr'];
            var caseNo = $('#ddlCgPrevCases').combogrid('grid').datagrid('getSelected')['CaseNo'];

            doAjax('LAW/Register/law_GetPrevCaseBySerial?caseSrl=' + caseSrl + '&caseYr=' + caseYear, 'post', {}, 'application/json',
                function (result) {
                    $('#txtLawCaseSerial').textbox('setValue', result[0]);
                    $('#txtLawCaseAppealSerial').textbox('setValue', result[1]);
                    $('#txtLawCaseHighCSerial').textbox('setValue', result[2]);

                    $('#txtCPrevCaseYr').textbox('setValue', caseYear);
                    $('#txtCPrevSrl').textbox('setValue', caseSrl);
                    $('#txtCPrevCaseNo').textbox('setValue', caseNo);

                },
                function (error) {
                    showAlert('خطأ', result.msg, 'error');
                });
        }
    });

    $('#ddlCgPrevCases').combogrid().combo('textbox').bind('keydown', function (e) {
        var ifv = $(this).val();
        if (ifv == '')
            $('#ddlCgPrevCases').combogrid('grid').datagrid('loadData', _PrevCaes);
        else {
            var filteredData = _PrevCaes.filter(function (e) { return e.cAndt.indexOf(ifv) > -1 });
            $('#ddlCgPrevCases').combogrid('grid').datagrid('loadData', filteredData);
        }

    });

    //************END*********************
});

function TxtCaseReqYearHjChange(newVal, oldVal) {
    if (!_LawCaseLoadingDataFromSearch) {
        if (_gsAppHDP.isValidStrDate(newVal)) {
            var gDateArr = _gsAppHDP.gDateFromStr(newVal).split('/');
            $('#txtLAWDateG').textbox('setValue', gDateArr[0] + '/' + gDateArr[1] + '/' + gDateArr[2]);
            //$('#txtLAWDateG').textbox('setValue', _gsAppHDP.gDateFromStr(newVal));            
        }
    }
}

function TxtCaseReqYearGrgChange(nv, ov) {
    if (!_LawCaseLoadingDataFromSearch) {
        if (nv != '' && nv != null) {
            //debugger;
            doAjax('Opr/Common/GetHijriDate?gDate=' + $('#txtLAWDateG').hdatebox('getValueF', 'yyyymmdd') + '&flag=' + 2, 'get', null, 'application/json',
                 function (hDt) {
                     var y = hDt.substr(0, 4);
                     var m = hDt.substr(4, 2);
                     var d = hDt.substr(6, 2);
                     var hijDT = y + '/' + m + '/' + d;
                     //hideProgress('#divParticipationReq');
                     $('#txtLAWDateH').hdatebox('setValue', hijDT);
                 },
                 function (error) {
                     //hideProgress('#divParticipationReq');
                     showAlert('خطأ', error, 'error');
                 }
             );
        }

    }
}

function LinkPrevCase() {
    var isChecked = $('#chkLinkCase').is(':checked');
    if (isChecked == true) {
        //show cases combobox
        //$('#divCaseLinkCombogrid').show();
        //$('#lblPrevLinkCase, #ddlCgPrevCases').show();

        $('#ddlCgPrevCases').combogrid('enable');
    } else {
        //hide cases combobox
        //$('#divCaseLinkCombogrid').hide();
        //$('#lblPrevLinkCase, #ddlCgPrevCases').hide();

        $('#ddlCgPrevCases').combogrid('disable');
    }
}

function SaveLawCaseMasterData(afterSaveCBFunc) {
    if (validateLawReqData()) {
        debugger;
        var isNew = _LawCaseCurrentReqObj == null;
        var isLinkCaseChecked = $('#chkLinkCase').is(':checked');
        var LawCaseReqData = {
            LawCaseReqMstr: {
                df: isNew ? 'I' : 'U',
                CASE_YEAR: $('#CaseYear').numberbox('getValue'),
                CASE_SERIAL: isNew ? null : $('#CaseSerial').numberbox('getValue'), 
                COURT_TYPE: $('input[name="rbCourtType"]:checked').val(),
                CASE_NO: $('#txtLawCaseSerial').textbox('getValue'),
                CASE_NO_PLEA: $('#txtLawCaseAppealSerial').textbox('getValue'),
                CASE_NO_HIGH: $('#txtLawCaseHighCSerial').textbox('getValue'),
                CASE_DATE: $('#txtLAWDateG').datebox('getValue', 'yyyymmdd'),
                CASE_TYPE: $('input[name="rbLawType"]:checked').val(),
                BRANCH_SRL_ID: $('#ddlCaseBranch').combobox('getValue'),
                CASE_STATUS: $('#ddlLawStatusType').combobox('getValue'),
                CASE_SECTOR: $('input[name="rLawCourtDistrict"]:checked').val(),
                SECTOR_COURT: $('input[name="rLawCourtDistrict"]:checked').val() == "1" ? $('#ddlCourtName1').combobox('getValue') : $('#ddlCourtName2').combobox('getValue'),
                COURT_CITY: $('input[name="rLawCourtDistrict"]:checked').val() == "1" ? $('#ddlCourtCity1').combobox('getValue') : $('#ddlCourtCity2').combobox('getValue'),
                DISTRICT_NAME: $('input[name="rLawCourtDistrict"]:checked').val() == "1" ? $('#txtDistrictName1').textbox('getValue') : $('#txtDistrictName2').textbox('getValue'),
                COURT_OTHERS: $('#txtCourtOthers').textbox('getValue'),
                EMP_NO_AUTHORITY: $('#txtNameOfSrcaRep').combobox('getValue').split('-')[0],
                JUDGEMENT_STATUS: $('#ddlRulingStatus').combobox('getValue'),
                JUDGMENT_STATUS_OTHERS: $('#txtJudgmentSatusOthers').textbox('getValue'),
                DEFENDANT_TYPE: $('#ddlTypeOfAccusedOrDefendant').combobox('getValue'),
                DEFENDANT_NAME: $('#txtNameOfAccusedOrDefendant').textbox('getText'),
                CASE_NOTES: $('#txtCaseDesc').textbox('getValue'),
                CONNECTED_SERIAL: isLinkCaseChecked ? $('#txtCPrevSrl').textbox('getValue') : null,
                CONNECTED_CASE_YEAR: isLinkCaseChecked ? $('#txtCPrevCaseYr').textbox('getValue') : null
            }

        }

        showProgress('#divLawInfo', 'جاري حفظ البيانات ...');
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
    }
}

function getCaseInfoForRequest(caseYear, caseSrl) {
    //for QUR
    //NewCaseFormRequest();
    //showProgress('#divLawInfo', 'جاري تحميل البيانات ...');
    doAjax('LAW/Register/law_GetCaseInfoForReq?caseYear=' + caseYear + '&caseSrl=' + caseSrl, 'post', {}, 'application/json',
        function (result) {
            hideProgress('#divLawInfo');
            console.log(result);
            _LawCaseCurrentReqObj = result.LawReqMstr;
            fillLawCaseReqForm(result, caseSrl);
        },
        function (error) {
            hideProgress('#divLawInfo');
            showAlert('خطأ', error, 'error');
        }
        );
}

function NewCaseFormRequest() {
    var comboArr = [];
    clearCaseRegForm();  //clear form fields first.
    //disable court comboboxes - ديوان المظالم | وزارة العدل | أخرى
    $('#ddlCourtName1').combobox({ editable: false, required: false });  //المحكمة - ديوان المظالم
    $('#ddlCourtName1').combobox('disable');
    $('#ddlCourtCity1').combobox({ editable: false, required: false });  //المدينة - ديوان المظالم
    $('#ddlCourtCity1').combobox('disable');
    $('#ddlCourtName2').combobox({ editable: false, required: false });  //المحكمة - وزارة العدل
    $('#ddlCourtName2').combobox('disable');
    $('#ddlCourtCity2').combobox({ editable: false, required: false });  //المدينة - وزارة العدل
    $('#ddlCourtCity2').combobox('disable');
    $('#txtDistrictName1, #txtDistrictName2').textbox({ editable: false });
    $('#txtCourtOthers').textbox({ editable: false, required: false });  //أخرى
    //إبتدائية | إستئنافية | عليا
    $('#txtLawCaseAppealSerial').textbox({ editable: false, required: false });
    $('#txtLawCaseHighCSerial').textbox({ editable: false, required: false });
    $('#txtLawCaseSerial').textbox({ editable: false, required: false });

    showProgress('#divLawInfo', 'جاري تحميل البيانات ...');
    doAjax('LAW/Register/LAW_newRequest', 'get', {}, 'application/json',  //LAW_newRequest  - LawReg
        function (newCaseReq) {
            debugger;
            hideProgress('#divLawInfo');
            //clearCaseRegForm();
            console.log(newCaseReq);
            $('#CaseYear').numberbox('setValue', newCaseReq.caseYear);
        },
        function (error) {
            showAlert('خطأ', error, 'error');
        }
        );
    _LawCaseCurrentReqObj = null;  //empty _LawCaseCurrentReqObj for new case
}

function clearCaseRegForm() {
    debugger;
    $('#CaseSerial').numberbox('clear');
    $('input[name="rbLawType"]').prop('checked', false);
    $('#txtLAWDateH').hdatebox('clear');
    $('#txtLAWDateG').datebox('clear');
    $('#ddlCaseBranch').combobox('clear');
    $('input[name="rLawCourtDistrict"]').prop('checked', false);
    $('#ddlCourtName1').combobox('clear');
    $('#ddlCourtCity1').combobox('clear');
    $('#ddlCourtName2').combobox('clear');
    $('#ddlCourtCity2').combobox('clear');
    $('#txtCourtOthers').textbox('clear');
    $('#txtDistrictName1, #txtDistrictName2').textbox('clear');
    $('input[name="rbCourtType"]').prop('checked', false);
    $('#txtLawCaseSerial').textbox('clear');
    $('#txtLawCaseAppealSerial').textbox('clear');
    $('#txtLawCaseHighCSerial').textbox('clear');
    $('#chkLinkCase').prop('checked', false);
    $('#ddlCgPrevCases').combogrid('disable');
    $('#ddlCgPrevCases').combogrid('clear');
    $('#txtCPrevCaseYr').textbox('clear');
    $('#txtCPrevSrl').textbox('clear');
    $('#txtCPrevCaseNo').textbox('clear');
    $('#ddlLawStatusType').combobox('clear');
    $('#ddlRulingStatus').combobox('clear');
    $('#txtNameOfSrcaRep').combobox('clear');
    $('#ddlTypeOfAccusedOrDefendant').combobox('clear');
    $('#txtNameOfAccusedOrDefendant').combobox('clear'); //maybe setText?
    $('#txtCaseDesc').textbox('clear');

    $('#btnAddSession').linkbutton('disable');
    $('#tblCaseSessions').datagrid('loadData', []);

    $('#txtLAWRegCBy').textbox('clear');
    $('#txtLAWRegCDt').textbox('clear');
    $('#txtLAWRegUBy').textbox('clear');
    $('#txtLAWRegUDt').textbox('clear');
}

function fillLawCaseReqForm(dataObj, caseSrl) {
    debugger;
    //enable Add session linkbutton
    $('#btnAddSession').linkbutton('enable');

    $('#CaseYear').numberbox('setValue', dataObj.LawReqMstr.CASE_YEAR);
    $('#CaseSerial').numberbox('setValue', caseSrl);
    //fill Case Form with dataObj
    $('input[name="rbLawType"][value=' + dataObj.LawReqMstr.CASE_TYPE + ']').prop('checked', true);
    $('#txtLAWDateH').hdatebox('setValue', dataObj.LawReqMstr.CASE_DATE_H);
    $('#txtLAWDateG').datebox('setValue', dataObj.LawReqMstr.CASE_DATE_G);
    $('#ddlCaseBranch').combobox('setValue', dataObj.LawReqMstr.BRANCH_SRL_ID);
    $('input[name="rLawCourtDistrict"][value=' + dataObj.LawReqMstr.CASE_SECTOR + ']').prop('checked', true);
    if (dataObj.LawReqMstr.CASE_SECTOR == "1") {
        //enable combobox
        $('#ddlCourtName1').combobox({ editable: true, required: true });
        $('#ddlCourtName1').combobox('enable');
        $('#ddlCourtCity1').combobox({ editable: true, required: true });
        $('#ddlCourtCity1').combobox('enable');
        $('#txtDistrictName1').textbox({ editable: true });


        $('#ddlCourtName2').combobox({ editable: false, required: false });
        $('#ddlCourtName2').combobox('disable');

        $('#ddlCourtCity2').combobox({ editable: false, required: false });
        $('#ddlCourtCity2').combobox('disable');

        //$('#txtDistrictName').textbox({ editable: false, required: false });
        $('#txtDistrictName2').textbox({ editable: false });
        $('#txtCourtOthers').textbox({ editable: false, required: false });


        $('#ddlCourtName1').combobox('setValue', dataObj.LawReqMstr.SECTOR_COURT);
        $('#ddlCourtCity1').combobox('setValue', dataObj.LawReqMstr.COURT_CITY);
        $('#txtDistrictName1').textbox('setValue', dataObj.LawReqMstr.DISTRICT_NAME);

    }
    if (dataObj.LawReqMstr.CASE_SECTOR == "2") {
        //enable combobox
        $('#ddlCourtName2').combobox({ editable: true, required: true });
        $('#ddlCourtName2').combobox('enable');
        $('#ddlCourtCity2').combobox({ editable: true, required: true });
        $('#ddlCourtCity2').combobox('enable');
        $('#txtDistrictName2').textbox({ editable: true });


        $('#ddlCourtName1').combobox({ editable: false, required: false });
        $('#ddlCourtName1').combobox('disable');

        $('#ddlCourtCity1').combobox({ editable: false, required: false });
        $('#ddlCourtCity1').combobox('disable');

        //$('#txtDistrictName').textbox({ editable: false, required: false });
        $('#txtDistrictName1').textbox({ editable: false });
        $('#txtCourtOthers').textbox({ editable: false, required: false });

        $('#ddlCourtName2').combobox('setValue', dataObj.LawReqMstr.SECTOR_COURT);
        $('#ddlCourtCity2').combobox('setValue', dataObj.LawReqMstr.COURT_CITY);
        $('#txtDistrictName2').textbox('setValue', dataObj.LawReqMstr.DISTRICT_NAME);


    }
    if (dataObj.LawReqMstr.CASE_SECTOR == "3") {
        //enable textbox
        //$('#txtDistrictName').textbox({ editable: false, required: false });
        $('#txtDistrictName1').textbox({ editable: false });
        $('#txtDistrictName2').textbox({ editable: false });


        $('#txtCourtOthers').textbox({ editable: true, required: true });
        $('#txtCourtOthers').textbox('setValue', dataObj.LawReqMstr.COURT_OTHERS);

        $('#ddlCourtName1').combobox({ editable: false, required: false });
        $('#ddlCourtName2').combobox('disable');

        $('#ddlCourtCity1').combobox({ editable: false, required: false });
        $('#ddlCourtCity1').combobox('disable');

        $('#ddlCourtName2').combobox({ editable: false, required: false });
        $('#ddlCourtName2').combobox('disable');

        $('#ddlCourtCity2').combobox({ editable: false, required: false });
        $('#ddlCourtCity2').combobox('disable');
    }
    //if (dataObj.LawReqMstr.CASE_SECTOR == "4") {
    //    //enable textbox
    //    $('#txtDistrictName').textbox({ editable: true, required: true });
    //    $('#txtDistrictName').textbox('setValue', dataObj.LawReqMstr.DISTRICT_NAME);

    //    $('#txtCourtOthers').textbox({ editable: false, required: false });

    //    $('#ddlCourtName1').combobox({ editable: false, required: false });
    //    $('#ddlCourtName2').combobox('disable');

    //    $('#ddlCourtCity1').combobox({ editable: false, required: false });
    //    $('#ddlCourtCity1').combobox('disable');

    //    $('#ddlCourtName2').combobox({ editable: false, required: false });
    //    $('#ddlCourtName2').combobox('disable');

    //    $('#ddlCourtCity2').combobox({ editable: false, required: false });
    //    $('#ddlCourtCity2').combobox('disable');
    //}
    $('input[name="rbCourtType"][value=' + dataObj.LawReqMstr.COURT_TYPE + ']').prop('checked', true);
    if (dataObj.LawReqMstr.COURT_TYPE == "1") {
        $('#txtLawCaseAppealSerial').textbox({ editable: false, required: false });
        $('#txtLawCaseHighCSerial').textbox({ editable: false, required: false });
        $('#txtLawCaseSerial').textbox({ editable: true, required: true });

        $('#txtLawCaseSerial').textbox('setValue', dataObj.LawReqMstr.CASE_NO);

    }
    if (dataObj.LawReqMstr.COURT_TYPE == "2") {
        $('#txtLawCaseHighCSerial').textbox({ editable: false, required: false });
        $('#txtLawCaseAppealSerial').textbox({ editable: true, required: true });
        $('#txtLawCaseSerial').textbox({ editable: true, required: true });

        $('#txtLawCaseSerial').textbox('setValue', dataObj.LawReqMstr.CASE_NO);
        $('#txtLawCaseAppealSerial').textbox('setValue', dataObj.LawReqMstr.CASE_NO_PLEA);
    }
    if (dataObj.LawReqMstr.COURT_TYPE == "3") {
        $('#txtLawCaseAppealSerial').textbox({ editable: true, required: true });
        $('#txtLawCaseHighCSerial').textbox({ editable: true, required: true });
        $('#txtLawCaseSerial').textbox({ editable: true, required: true });

        $('#txtLawCaseSerial').textbox('setValue', dataObj.LawReqMstr.CASE_NO);
        $('#txtLawCaseAppealSerial').textbox('setValue', dataObj.LawReqMstr.CASE_NO_PLEA);
        $('#txtLawCaseHighCSerial').textbox('setValue', dataObj.LawReqMstr.CASE_NO_HIGH);


    }

    if (dataObj.LawReqMstr.CONNECTED_SERIAL != null) {
        $('#chkLinkCase').prop('checked', true);
        $('#ddlCgPrevCases').combogrid('enable');
        $('#ddlCgPrevCases').combogrid('setValue', dataObj.LawReqMstr.CONNECTED_CASE_YEAR + ' - ' + dataObj.LawReqMstr.CONNECTED_SERIAL + ' - ' + dataObj.LawReqMstr.CONNECTED_CASE_NO);
        $('#txtCPrevCaseYr').textbox('setValue', dataObj.LawReqMstr.CONNECTED_CASE_YEAR);
        $('#txtCPrevSrl').textbox('setValue', dataObj.LawReqMstr.CONNECTED_SERIAL);
        $('#txtCPrevCaseNo').textbox('setValue', dataObj.LawReqMstr.CONNECTED_CASE_NO);

    } else {
        $('#chkLinkCase').prop('checked', false);
        $('#ddlCgPrevCases').combogrid('disable');
        $('#ddlCgPrevCases').combogrid('clear');
        $('#txtCPrevCaseYr').textbox('clear');
        $('#txtCPrevSrl').textbox('clear');
        $('#txtCPrevCaseNo').textbox('clear');
    }
    $('#ddlLawStatusType').combobox('setValue', dataObj.LawReqMstr.CASE_STATUS);
    $('#ddlRulingStatus').combobox('setValue', dataObj.LawReqMstr.JUDGEMENT_STATUS);
    $('#txtNameOfSrcaRep').combobox('setValue', dataObj.LawReqMstr.EMP_NO_AUTHORITY + ' - ' + dataObj.LawReqMstr.EMP_NAME_AUTHORITY); //maybe setText?
    $('#ddlTypeOfAccusedOrDefendant').combobox('setValue', dataObj.LawReqMstr.DEFENDANT_TYPE);
    $('#txtNameOfAccusedOrDefendant').combobox('setValue', dataObj.LawReqMstr.DEFENDANT_NAME); //maybe setText?
    $('#txtCaseDesc').textbox('setValue', dataObj.LawReqMstr.CASE_NOTES);

    //Session DataGrid
    $('#tblCaseSessions').datagrid('loadData', dataObj.LawReqDetail);

    $('#txtLAWRegCBy').textbox('setValue', dataObj.LawReqMstr.CBY);
    $('#txtLAWRegCDt').textbox('setValue', dataObj.LawReqMstr.CDT);
    $('#txtLAWRegUBy').textbox('setValue', dataObj.LawReqMstr.UBY);
    $('#txtLAWRegUDt').textbox('setValue', dataObj.LawReqMstr.UDT);
}

function LawRegSearchResult(index, row) {  //SearchForm onDbClick will trigger this function 
    debugger;
    $('#dLawSearchForm').window('close');
    clearCaseRegForm();
    getCaseInfoForRequest(row.CASE_YEAR, row.CASE_SERIAL);
}

function validateLawReqData() {
    var isValid = true;
    if ($('input[name="rbLawType"]:checked').val() == undefined) {
        showAlert('بيانات مطلوبة', ' الرجاء إختيار النوع', 'error', function () { $('input[name="rbLawType"]:checked').focus(); });
        isValid = false;
    }
    else if ($('#txtLAWDateH').hdatebox('getValue') == "" || $('#txtLAWDateG').datebox('getValue') == "") {
        showAlert('بيانات مطلوبة', ' الرجاء تحديد تاريخ القضية ', 'error', function () { $('#txtLAWDateH').hdatebox('textbox').focus(); });
        isValid = false;
    }
    else if ($('input[name="rLawCourtDistrict"]:checked').val() == undefined) {
        showAlert('بيانات مطلوبة', ' الرجاء إختيار قطاع المحكمة ', 'error', function () { $('input[name="rLawCourtDistrict"]:checked').focus(); });
        isValid = false;
    }
    else if ($('input[name="rLawCourtDistrict"]:checked').val() == "1") {
        if ($('#ddlCourtName1').combobox('getValue') == "") {
            showAlert('بيانات مطلوبة', ' الرجاء إختيار المحكمة', 'error', function () { $('#ddlCourtName1').combobox('textbox').focus(); });
            isValid = false;
        } else if ($('#ddlCourtCity1').combobox('getValue') == "") {
            showAlert('بيانات مطلوبة', ' الرجاء إختيار المدينة', 'error', function () { $('#ddlCourtCity1').combobox('textbox').focus(); });
            isValid = false;
        }
    }
    else if ($('input[name="rLawCourtDistrict"]:checked').val() == "2") {
        if ($('#ddlCourtName2').combobox('getValue') == "") {
            showAlert('بيانات مطلوبة', ' الرجاء إختيار المحكمة', 'error', function () { $('#ddlCourtName2').combobox('textbox').focus(); });
            isValid = false;
        } else if ($('#ddlCourtCity2').combobox('getValue') == "") {
            showAlert('بيانات مطلوبة', ' الرجاء إختيار المدينة', 'error', function () { $('#ddlCourtCity2').combobox('textbox').focus(); });
            isValid = false;
        }
    }
    else if ($('input[name="rLawCourtDistrict"]:checked').val() == "3") {
        if ($('#txtCourtOthers').textbox('getValue') == "") {
            showAlert('بيانات مطلوبة', ' الرجاء إدخال حقل أخرى', 'error', function () { $('#txtCourtOthers').textbox('textbox').focus(); });
            isValid = false;
        }
    }
    //else if ($('input[name="rLawCourtDistrict"]:checked').val() == "4") {
    //    if ($('#txtDistrictName').textbox('getValue') == "") {
    //        showAlert('بيانات مطلوبة', ' الرجاء إدخال رقم الدائرة', 'error', function () { $('#txtDistrictName').textbox('textbox').focus(); });
    //        isValid = false;
    //    }
    //}
    else if ($('input[name="rbCourtType"]:checked').val() == undefined) {
        showAlert('بيانات مطلوبة', ' الرجاء إختيار نوع المحكمة ', 'error', function () { $('input[name="rbCourtType"]:checked').focus(); });
        isValid = false;
    }
    else if ($('input[name="rbCourtType"]:checked').val() == "1") {
        if ($('#txtLawCaseSerial').textbox('getValue') == "") {
            showAlert('بيانات مطلوبة', ' الرجاء إدخال رقم القضية', 'error', function () { $('#txtLawCaseSerial').textbox('textbox').focus(); });
            isValid = false;
        }
    }
    else if ($('input[name="rbCourtType"]:checked').val() == "2") {
        if ($('txtLawCaseSerial').textbox('getValue') == "") {
            showAlert('بيانات مطلوبة', ' الرجاء إدخال رقم القضية', 'error', function () { $('#txtLawCaseSerial').textbox('textbox').focus(); });
            isValid = false;
        } else if ($('txtLawCaseAppealSerial').textbox('getValue') == "") {
            showAlert('بيانات مطلوبة', ' الرجاء إدخال رقم القضية بالإستئناف', 'error', function () { $('#txtLawCaseAppealSerial').textbox('textbox').focus(); });
            isValid = false;
        }
    }
    else if ($('input[name="rbCourtType"]:checked').val() == "3") {
        if ($('txtLawCaseSerial').textbox('getValue') == "") {
            showAlert('بيانات مطلوبة', ' الرجاء إدخال رقم القضية', 'error', function () { $('#txtLawCaseSerial').textbox('textbox').focus(); });
            isValid = false;
        } else if ($('txtLawCaseAppealSerial').textbox('getValue') == "") {
            showAlert('بيانات مطلوبة', ' الرجاء إدخال رقم القضية بالإستئناف', 'error', function () { $('#txtLawCaseAppealSerial').textbox('textbox').focus(); });
            isValid = false;
        } else if ($('txtLawCaseHighCSerial').textbox('getValue') == "") {
            showAlert('بيانات مطلوبة', ' الرجاء إدخال رقم القضية بالمحكمة العليا', 'error', function () { $('#txtLawCaseHighCSerial').textbox('textbox').focus(); });
            isValid = false;
        }
    }
    else if ($('#txtNameOfSrcaRep').combobox('getValue') == "") {
        showAlert('بيانات مطلوبة', ' الرجاء إدخال ممثل الهيئة ', 'error', function () { $('#txtNameOfSrcaRep').combobox('textbox').focus(); });
        isValid = false;
    }
    else if ($('#ddlTypeOfAccusedOrDefendant').combobox('getValue') == "") {
        showAlert('بيانات مطلوبة', ' الرجاء إختيار طبيعة المدعي / المدعي عليه ', 'error', function () { $('#ddlTypeOfAccusedOrDefendant').combobox('textbox').focus(); });
        isValid = false;
    }
    else if ($('#txtJudgmentSatusOthers').textbox('getValue') == "") {
        showAlert('بيانات مطلوبة', ' الرجاء إدخال حالة الحكم أخرى ', 'error', function () { $('#txtJudgmentSatusOthers').combobox('textbox').focus(); });
        isValid = false;
    }
    else if ($('#txtNameOfAccusedOrDefendant').combobox('getValue') == "") {
        showAlert('بيانات مطلوبة', ' الرجاء أدخال إسم المدعي / المدعي عليه ', 'error', function () { $('#txtNameOfAccusedOrDefendant').combobox('textbox').focus(); });
        isValid = false;
    }

    return isValid;
}

function LawSessionListOnLoadSuccess(data) {
    $(this).datagrid('getPanel').find('.datagrid-view a.easyui-linkbutton').linkbutton();
}

function doLawRegSearch() {
    $('#dLawSearchForm').window('open');
}

function DisplayNewLawSessionScreen() {
    $('#dAddNewSessionForm').window('open');
}

function closeLawRegForm() {
    closeTab();
}