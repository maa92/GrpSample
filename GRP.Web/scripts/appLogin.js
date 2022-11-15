var _focusPwdText;
function isIE() {
    var myNav = navigator.userAgent.toLowerCase();
    return (myNav.indexOf('msie') != -1) ? parseInt(myNav.split('msie')[1]) : false;
}

function checkRTL(str) {
    var ltrChars = 'A-Za-z\u00C0-\u00D6\u00D8-\u00F6\u00F8-\u02B8\u0300-\u0590\u0800-\u1FFF' + '\u2C00-\uFB1C\uFDFE-\uFE6F\uFEFD-\uFFFF',
        rtlChars = '\u0591-\u07FF\uFB1D-\uFDFD\uFE70-\uFEFC';
    return (new RegExp('^[^' + ltrChars + ']*[' + rtlChars + ']')).test(str);
};

function initUI(focusPwd) {
    focusPwd.toString().toLowerCase() == 'true' ? $('#Password').textbox('textbox').focus() : $('#UserName').textbox('textbox').focus();

    $('.textbox-text').keydown(function (e) {
        if (e.keyCode == 13) {
            submitForm();
            return false;
        }
    });

    $('[name="UserName"]').prev().on('keypress', userNameKP);
}

function userNameKP(e) {
    var ew = event.which;
    if (ew == 32 || ew == 13)
        return true;
    if (48 <= ew && ew <= 57)
        return true;
    if (65 <= ew && ew <= 90)
        return true;
    if (97 <= ew && ew <= 122)
        return true;
    return false;
    //if (checkRTL(String.fromCharCode(e.charCode))) {
    //    e.preventDefault();
    //}
}

function showMessageContainsValue(msg) {
    if (msg != '') {
        $.messager.alert(msg.split('|')[0], msg.split('|')[1], 'error', function () {
            if(_focusPwdText == 'true')
                $('#Password').textbox('textbox').focus();
        });
    }
}

function showErrDetails(btnObj) {
    $('#wndLgErrDetails').text($(btnObj).attr('data-err'));
    $('#wndLgErrDetails').window({
        minimizable: false,
        collapsible: false,
        title : 'تفاصيل الخطأ',
        width: 600,
        height: 400,
        modal: true
    });
}

function submitForm() {
    if ($('#frmLogin').form('enableValidation').form('validate'))
        $('#frmLogin').submit();
    else
        return false;
}