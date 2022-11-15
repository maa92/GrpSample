function addHijriIcon() {
    $('.easyui-hdatebox').next().find("a").addClass("hdateboxicon");
}

$(document).ready(function () {
    var wndId = this.id;

    setTimeout(addHijriIcon, 1);
    $.fn.datebox.defaults.formatter = formatterG;
    $.fn.datebox.defaults.parser = parserG;

    var lawEmpLoader = function (param, success, error) {
        var q = param.q || '';
        if (q.length < 2) { return false }

        doAjax('HR/Common/SearchEmp?q=' + encodeURIComponent(q), 'post', {}, 'application/json',
            function (result) { success(result); }, function (error) { error.apply(this, arguments); }
        );
    }

    $()
});

function TxtYearGrgChange(newVal, oldVal) {
    debugger;
    var wndId = this.id.split('_')[1];

    if (newVal == "") {
        $('#txtCaseYearHj' + '_' + wndId).numberbox('clear');
    } else {
        //var GregorianYear = (new Date()).getFullYear();
        var HijriYear = Math.round((newVal - 622) * (33 / 32));
        $('#txtCaseYearHj' + '_' + wndId).numberbox('setValue', HijriYear);
    }
}

function TxtYearHjChange(newVal, oldVal) {
    debugger;
    var wndId = this.id.split('_')[1];
    
    if (newVal == "") {
        $('#txtCaseYear' + '_' + wndId).numberbox('clear');
    } else {
        //var GrgYear = Math.round((newVal * 0.97) + 622);  //* (33 / 32)
        var GrgYear = Math.floor(newVal * 0.97) + 622;
        $('#txtCaseYear' + '_' + wndId).numberbox('setValue', GrgYear);
    }
}

function TxtCaseSearchYearHjChange(newVal, oldVal) {
    var wndId = this.id;
    if (_gsAppHDP.isValidStrDate(newVal)) {
        var gDateArr = _gsAppHDP.gDateFromStr(newVal).split('/');
        $('#txtLawSearchDateG').textbox('setValue', gDateArr[0] + '/' + gDateArr[1] + '/' + gDateArr[2])
        //$('#txtLawSearchDateG').textbox('setValue', _gsAppHDP.gDateFromStr(newVal));            
    }
}

function TxtCaseSearchYearGrgChange(nv, ov) {
    var wndId = this.id;
    if (nv != '' && nv != null) {
        //debugger;
        doAjax('Opr/Common/GetHijriDate?gDate=' + $('#txtLawSearchDateG').hdatebox('getValueF', 'yyyymmdd') + '&flag=' + 2, 'get', null, 'application/json',
              function (hDt) {
                  var y = hDt.substr(0, 4);
                  var m = hDt.substr(4, 2);
                  var d = hDt.substr(6, 2);
                  var hijDT = y + '/' + m + '/' + d;
                  //hideProgress('#divParticipationReq');
                  $('#txtLawSearchDateH').hdatebox('setValue', hijDT);
              },
              function (error) {
                  //hideProgress('#divParticipationReq');
                  showAlert('خطأ', error, 'error');
              }
           );
    }
}

function openLawSrchForm() {
    var wndId = this.id;

    $('#txtCaseYear' + '_' + wndId).numberbox('setValue', new Date().getFullYear());
    //Court District radio button onclick events 
    $('#rbCourtDistrictType1' + '_' + wndId).click(function () {  //محاكم ديوان المظالم
        $('#ddlCourtName1' + '_' + wndId).combobox({ editable: true });
        $('#ddlCourtName1' + '_' + wndId).combobox('enable');
        $('#ddlCourtCity1' + '_' + wndId).combobox({ editable: true });
        $('#ddlCourtCity1' + '_' + wndId).combobox('enable');

        $('#ddlCourtName2' + '_' + wndId).combobox({ editable: false });
        $('#ddlCourtName2' + '_' + wndId).combobox('disable');

        $('#ddlCourtCity2' + '_' + wndId).combobox({ editable: false });
        $('#ddlCourtCity2' + '_' + wndId).combobox('disable');
    });

    $('#rbCourtDistrictType2' + '_' + wndId).click(function () {  //محاكم وزارة العدل 
        $('#ddlCourtName2' + '_' + wndId).combobox({ editable: true });
        $('#ddlCourtName2' + '_' + wndId).combobox('enable');
        $('#ddlCourtCity2' + '_' + wndId).combobox({ editable: true });
        $('#ddlCourtCity2' + '_' + wndId).combobox('enable');

        $('#ddlCourtName1' + '_' + wndId).combobox({ editable: false });
        $('#ddlCourtName1' + '_' + wndId).combobox('disable');

        $('#ddlCourtCity1' + '_' + wndId).combobox({ editable: false });
        $('#ddlCourtCity1' + '_' + wndId).combobox('disable');
    });
    //*********************************************
    doAjax('LAW/Register/LoadSearchFormLookups', 'get', null, 'application/json',
        function (lookupData) {
            debugger;
            console.log(lookupData);
            //$('#txtCaseYear' + '_' + wndId).numberbox('setValue', lookupData.GrgYear);
            //$('#txtCaseYearHj' + '_' + wndId).numberbox('setValue', lookupData.hijriYear);
            $('#ddlCaseStatus' + '_' + wndId).combobox('loadData', lookupData.cStatus);
            $('#ddlCaseRuling' + '_' + wndId).combobox('loadData', lookupData.JStatus);
            $('#ddlBranch' + '_' + wndId).combobox('loadData', lookupData.branches);
            $('#ddlCourtName1' + '_' + wndId).combobox('loadData', lookupData.GCourts);
            $('#ddlCourtName2' + '_' + wndId).combobox('loadData', lookupData.JCourts);
            $('#ddlCourtCity1' + '_' + wndId).combobox('loadData', lookupData.cCity);
            $('#ddlCourtCity2' + '_' + wndId).combobox('loadData', lookupData.cCity);
        },
        function (error) {
            showAlert('خطأ', error, 'error');
        }
        );
}

function doCaseSearchFromSearchWnd(btnObj) {
    var wndId = $(btnObj).closest('div.easyui-window')[0].id;
    debugger;
    $('#tblLawSearchFrmResult' + '_' + wndId).datagrid('load', {
        CASE_NO: $('#txtCaseNo' + '_' + wndId).textbox('getValue'),
        CASE_DATE: $('#txtLawSearchDateG').hdatebox('getValueF', 'yyyy/mm/dd'),
        CASE_YEAR: $('#txtCaseYear' + '_' + wndId).numberbox('getValue'),
        CASE_TYPE: $('input[name="rbCaseSrchType' + '_' + wndId + '"]:checked').val(),
        CASE_SERIAL: $('#numCaseSerial' + '_' + wndId).numberbox('getValue'),
        //COURT_TYPE: $('input[name="rbCourtDistrictType' + '_' + wndId + '"]:checked').val(),
        CASE_STATUS: $('#ddlCaseStatus' + '_' + wndId).combobox('getValue'),
        CASE_SECTOR: $('input[name="rbCourtDistrictType' + '_' + wndId + '"]:checked').val(),
        SECTOR_COURT: ($('input[name="rbCourtDistrictType' + '_' + wndId + '"]:checked').val() == "1") && ($('input[name="rbCourtDistrictType' + '_' + wndId + '"]:checked').val() != "3") ?
            $('#ddlCourtName1' + '_' + wndId).combobox('getValue') : $('#ddlCourtName2' + '_' + wndId).combobox('getValue'),
        COURT_CITY: ($('input[name="rbCourtDistrictType' + '_' + wndId + '"]:checked').val() == "1") && ($('input[name="rbCourtDistrictType' + '_' + wndId + '"]:checked').val() != "3") ?
            $('#ddlCourtCity1' + '_' + wndId).combobox('getValue') : $('#ddlCourtCity2' + '_' + wndId).combobox('getValue'),
        EMP_NO_AUTHORITY: $('#ddlSrcaRep' + '_' + wndId).combobox('getValue'),
        JUDGEMENT_STATUS: $('#ddlCaseRuling' + '_' + wndId).combobox('getValue'),
        DEFENDANT_TYPE: null,
        //PAGENUMBER
        //PAGESIZE
        loadManual: true
    });
}

function tblLawResultBrforeLoad(param) {
    $('#btnSearchForCases' + '_' + this.id.split('_')[1]).linkbutton('enable');
}

function tblLawResultLoadSuccess(data) {
    $('#btnSearchForCases' + '_' + this.id.split('_')[1]).linkbutton('enable');
}

function tblLawResultLoadError() {
    $('#btnSearchForCases' + '_' + this.id.split('_')[1]).linkbutton('enable');
}