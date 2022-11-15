var lawCaseSessionApprovalStatusFormatter = function (value, row, index) {
    debugger;
    var parentId = $('.' + this.cellClass).closest('.datagrid').find('[id^="lawTblSessionReqApprvContainer_"]').attr('id').split('_')[1];
    var col = '';
    var yes = 'Y';
    var no = 'N';
    if (row.SESSION_ID) {
        col = '<input type="radio" name="rbIsAttnd' + row.SESSION_ID + '" id="rbIsAttnd' + row.SESSION_ID + 'Yes" value="Y" class="lawapprvBigRadioBtn" ' + (row.IS_ATTEND == 'Y' ? 'checked' : '') + ' onclick="changeIsAttendStatus(' + index + ',&quot;' + parentId + '&quot;,1);" /><label class="lawapprvBigRadioBtn" for="rbIsAttnd' + row.SESSION_ID + 'Yes">حضرت</label>'; //'&quot;rbA&quot;,'
        col += '<input type="radio" name="rbIsAttnd' + row.SESSION_ID + '" id="rbIsAttnd' + row.SESSION_ID + 'No" value="N" class="lawapprvBigRadioBtn" ' + (row.IS_ATTEND == 'N' ? 'checked' : '') + ' onclick="changeIsAttendStatus(' + index + ',&quot;' + parentId + '&quot;,0);" /><label class="lawapprvBigRadioBtn" for="rbIsAttnd' + row.SESSION_ID + 'No">لم أحضر</label>';
    }
    return col;
}

$(document).ready(function () {
    getCurrentFormChangesObj().beforeCloseSaveChangesEV = lawSaveSessionAccept;

    $.fn.datebox.defaults.formatter = formatterG;
    $.fn.datebox.defaults.parser = parserG;
});

function changeIsAttendStatus(rowIndex, gridUniqueId, stsVal) {
    var gridData = $('#lawTblSessionReqApprvContainer_' + gridUniqueId).datagrid('getData');
    gridData.rows[rowIndex].IS_ATTEND = stsVal == 1 ? 'Y' : 'N';
    gridData.rows[rowIndex].df = 'U';
    getCurrentFormChangesObj().hasChanges = true;
}

function lawSaveSessionAcceptBtn(frmId) {
    debugger;
    var gridData = $('#lawTblSessionReqApprvContainer_' + frmId).datagrid('getData');
    if (getCurrentFormChangesObj().hasChanges) {
        if (endEditingLawSessionList(frmId)) {
            showProgress('#lawTabContainer_' + frmId, 'جاري حفظ البيانات ...');

            var changedData = _.filter(gridData.rows, function (sa) { return sa.df == 'U' });
            doAjax('LAW/Register/SaveSessionAcceptReq?SessionNote=' + $('#txtSessionAttndNotes_' + frmId).textbox('getValue'), 'post', JSON.stringify(changedData), 'application/json',
                function (result) {
                    hideProgress('#lawTabContainer_' + frmId);
                    console.log(result);
                    if (result.res > 0 && result.msg == "") {
                        showAlert('تنبيه', 'تم الحفظ بنجاح.', 'success');
                        getCurrentFormChangesObj().hasChanges = false;
                        if (frmId.startsWith('lawAtt')) {
                            $('#lawTblSessionReqApprvContainer_' + frmId).datagrid('loadData', []);
                        }
                        else {
                            lawReloadSessionAccept(frmId);
                        }
                    }
                    else {
                        showAlert('تنبيه', result.msg, 'error');
                    }
                },
                function (error) {
                    hideProgress('#lawTabContainer_' + frmId);
                    showAlert('خطأ', error, 'error');
                });
        }
    }
}

function lawSaveSessionAccept(afterSaveCB) {
    var frmId = getCurrentFormChangesObj().frmId;
    var gridData;
    if (frmId.startsWith('lawAtt'))
        gridData = $('#lawTblSessionReqApprvContainer_' + frmId).datagrid('getData');
    else
        gridData = $('#lawTblSessionReqApprvContainer_0').datagrid('getData');

    if (getCurrentFormChangesObj().hasChanges) {
        if (endEditingLawSessionList(frmId)) {
            showProgress('#lawTabContainer_' + frmId, 'جاري حفظ البيانات ...');

            doAjax('LAW/Register/SaveSessionAcceptReq', 'post', JSON.stringify(gridData.rows), 'application/json',
                function (result) {
                    hideProgress('#lawTabContainer_' + frmId);
                    if (result.res > 0 && result.msg == "") {
                        showAlert('تنبيه', 'تم الحفظ بنجاح.', 'success');

                        if (afterSaveCB)
                            afterSaveCB();
                        else {
                            getCurrentFormChangesObj().hasChanges = false;
                            if (frmId.startsWith('lawAtt')) {
                                $('#lawTblSessionReqApprvContainer_' + frmId).datagrid('loadData', []);
                            }
                            else {
                                lawReloadSessionAccept(frmId);
                            }
                        }
                    }
                    else {
                        showAlert('تنبيه', result.msg, 'error');
                    }
                },
                function (error) {
                    hideProgress('#lawTabContainer_' + frmId);
                    showAlert('خطأ', error, 'error');
                });
        }
    }
}

function lawReloadSessionAccept(fId) {
    showProgress('#lawTabContainer_' + fId, 'جاري حفظ البيانات ...');
    doAjax('LAW/Register/ReloadApproveRequests', 'get', null, 'application/json',
        function (sessionLst) {
            hideProgress('#lawTabContainer_' + fId);
            $('#lawTblSessionReqApprvContainer_' + fId).datagrid('loadData', sessionLst);
        },
        function (error) {
            hideProgress('#lawTabContainer_' + fId);
            showAlert('خطأ', error, 'error');
        });
}

function onLawSessionReqListSelect(index, row) {
    var id = this.id.split('_')[1];
    $('#txtSessionApprvCBy' + id).textbox('setValue', row.CBY);
    $('#txtSessionApprvCDt' + id).textbox('setValue', row.CDT);
    $('#txtSessionApprvUBy' + id).textbox('setValue', row.UBY);
    $('#txtSessionApprvUDt' + id).textbox('setValue', row.UDT);

    $('#txtSessionAttndNotes_' + id).textbox('setValue', row.SESSION_NOTE);
}

function onLawSessionListClickCell(index, field) {
    debugger;
    var id = this.id.split('_')[1];
    var curEditIndex = $('#lawTblSessionReqApprvContainer_' + id).attr('data-editRowIndex');
    if (curEditIndex != index) {
        if (endEditingLawSessionList(id)) {
            $(this).datagrid('selectRow', index).datagrid('beginEdit', index);
            $(this).attr('data-editRowIndex', index);
        }
    }
}

function endEditingLawSessionList(gridId) {
    var gcId = '#lawTblSessionReqApprvContainer_' + gridId;
    var curEditIndex = $(gcId).attr('data-editRowIndex');
    if (!curEditIndex) { return true }
    if ($(gcId).datagrid('validateRow', curEditIndex)) {
        $(gcId).datagrid('endEdit', curEditIndex);
        $(gcId).removeAttr('data-editRowIndex');
        return true;
    } else {
        return false;
    }
}