var openedFrmsChanges = [], currentOpenedFrmId = null;
var confirm3BCBFunc, forceRedirect = false;

window.onbeforeunload = beforeUnloadPage;

function beforeUnloadPage(e) {
    if (!forceRedirect) {
        var _atc = openedFrmsChanges.filter(function (ae) { return ae.hasChanges == true; });
        if (_atc != null && _atc.length > 0) {
            if (!e) e = window.event;
            //e.cancelBubble is supported by IE - this will kill the bubbling process.
            e.cancelBubble = true;
            e.returnValue = 'توجد بعض التعديلات التي لم يتم حفظها, اغلاق ؟';
            //e.stopPropagation works in Firefox.
            if (e.stopPropagation) {
                e.stopPropagation();
                e.preventDefault();
            }

            //return works for Chrome and Safari
            return 'توجد بعض التعديلات التي لم يتم حفظها, اغلاق ؟';
        }
    }
}

if (typeof String.prototype.startsWith != 'function') {
    String.prototype.startsWith = function (str) {
        return this.indexOf(str) === 0;
    };
}

//function Sleep(time) {
//    return new Promise((resolve) => setTimeout(resolve, time));
//}

String.prototype.replaceAll = function (target, replacement) {
    return this.split(target).join(replacement);
};

function isEquivalent(a, b, compareType) {
    // Create arrays of property names
    var aProps = Object.getOwnPropertyNames(a);
    var bProps = Object.getOwnPropertyNames(b);

    // If number of properties is different,
    // objects are not equivalent
    if (aProps.length != bProps.length) {
        return false;
    }

    for (var i = 0; i < aProps.length; i++) {
        var propName = aProps[i];

        // If values of same property are not equal,
        // objects are not equivalent
        if (compareType) {
            if (a[propName] !== b[propName]) {
                return false;
            }
        }
        else {
            if (a[propName] != b[propName]) {
                return false;
            }
        }
    }

    // If we made it this far, objects
    // are considered equivalent
    return true;
}

function zeroPad(num, places) {
    var zero = places - num.toString().length + 1;
    return Array(+(zero > 0 && zero)).join("0") + num;
}

function blinkText(selector) {
    $(selector).fadeOut(700, function () {
        $(this).fadeIn(700, function () {
            blinkText(this);
        });
    });
}

function formatHijriDateInput(iDate) {
    var y, m, d;
    y = iDate.substr(iDate.length - 4, 4);
    var r = iDate.replace(y, '');
    if (r.length == 2) {
        d = zeroPad(r.substr(0, 1), 2);
        m = zeroPad(r.substr(0, 1), 2);
    }
    else {
        d = r.substr(0, 2);
        m = r.substr(2, 2);
    }
    return y + '/' + m + '/' + d;
}

function parsetHijriDateInput(formattedDate) {
    if (formattedDate.indexOf('/') > -1)
        return zeroPad(formattedDate.split('/')[2], 2) + zeroPad(formattedDate.split('/')[1], 2) + formattedDate.split('/')[0];
    else
        return formattedDate;
}
function formatterG(date) {

    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
   // return (d < 10 ? ('0' + d) : d) + '/' + (m < 10 ? ('0' + m) : m) + '/' + y;
    return y + '/' + (m < 10 ? ('0' + m) : m) + '/' + (d < 10 ? ('0' + d) : d);
}
function parserG(s) {
    if (!s) return new Date();
    var ss = (s.split('\/'));
    var d = parseInt(ss[2], 10);
    var m = parseInt(ss[1], 10);
    var y = parseInt(ss[0], 10);
    if (!isNaN(y) && !isNaN(m) && !isNaN(d)) {
        return new Date(y, m - 1, d);
    } else {
        return new Date();
    }
}

function doAjax(reqUrl, reqType, reqData, cType, sucesscb, failcb) {
    $.ajax({
        type: reqType,
        url: reqUrl,
        data: reqData,
        contentType: cType,
        //async: true,
        //dataType: 'json',
        success: function (data, status, xhr) {
            var xhrResponse = xhr.getResponseHeader('X-Responded-JSON');
            if (xhrResponse && JSON.parse(xhrResponse).status == 401) {
                forceRedirect = true;
                window.location.replace(window.location + 'User/Login');
            }
            sucesscb(data);
        },
        error: function (xhr, status, err) {
            if (failcb)
                failcb(err);
            //else
            //    alert(xhr + ' ' + status + ' ' + err);
        }
    });
}


function doSyncAjax(reqUrl, reqType, reqData, cType, sucesscb, failcb) {
    $.ajax({
        type: reqType,
        url: reqUrl,
        data: reqData,
        contentType: cType,
        async: false,
        //dataType: 'json',
        success: function (data, status, xhr) {
            var xhrResponse = xhr.getResponseHeader('X-Responded-JSON');
            if (xhrResponse && JSON.parse(xhrResponse).status == 401) {
                forceRedirect = true;
                window.location.replace(window.location + 'User/Login');
            }
            sucesscb(data);
        },
        error: function (xhr, status, err) {
            if (failcb)
                failcb(err);
            //else
            //    alert(xhr + ' ' + status + ' ' + err);
        }
    });
}

function showProgress(divElem, msgText) {
    $(divElem).block({
        message: msgText,
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });
}

function hideProgress(divElem) {
    $(divElem).unblock();
}

var appPath = location.pathname.split('/')[1];
var g_treeCtrlId = '#ulSysTree';
var g_tabCtrlId = '#apptbs';

function showMessage(code) {
    doAjax('App/GetSysMsg', 'get', { msgCode: code }, 'application/json', function (msg) {
        if (msg.charAt(0) === '"' && msg.charAt(msg.length - 1) === '"')
            msg = msg.slice(1, msg.length - 1);

        var msgType;
        switch (msg.split('|')[0]) {
            case 'خطأ' || 'خطا':
                msgType = "error";
                break;
            case 'تنبيه':
                msgType = "info";
                break;
        }

        $.messager.alert(msg.split('|')[0], msg.split('|')[1], msgType);
    });
}

function showAlert(title, msgText, msgType, cbFunc, wndWidth) {
    $.messager.alert({ title: title, msg: msgText, icon: msgType, fn: cbFunc, width: (wndWidth ? wndWidth : 300) });
    //if (cbFunc)

    //    $.messager.alert(title, msgText, msgType, cbFunc);
    //else
    //    $.messager.alert(title, msgText, msgType);
}

function showConfirm(title, msgText, cbFunc) {
    $.messager.confirm(title, msgText, cbFunc);
}

function showConfirm3B(title, msgText, cbFunc) {
    confirm3BCBFunc = cbFunc;
    $('#divGSysAlertText').html(msgText);
    $('#divGSysAlert').window({
        modal: true,
        title: title,
        collapsible: false,
        minimizable: false,
        maximizable: false,
        closable: false,
        iconCls: 'icon-save',
        footer: '#divGSysAlertFt'
    });
}

function closeConfirm3BYNCEH(arg) {
    $('#divGSysAlert').window('close');
    confirm3BCBFunc(arg);
}

function getCurrentFormChangesObj() {
    return openedFrmsChanges.filter(function (e) { return e.frmId == currentOpenedFrmId; })[0];
}

function initUILayout() {
    $('#txtGSysTreeFilter').textbox({
        icons: [{
            iconCls: 'icon-clear',
            handler: function (e) {
                $(e.data.target).textbox('clear');
                $('#ulSysTree').tree('doFilter', '');
                $('#ulSysTree').tree('collapseAll');
            }
        }]
    }).textbox('textbox').bind('keydown', function (e) {
        if ($(this).val() == '')
            $('#ulSysTree').tree('collapseAll');
        else {
            $('#ulSysTree').tree('doFilter', $(this).val());
            $('#ulSysTree').tree('expandAll');
        }
    });
}

function openRefUrl(fup) {
    doAjax('App/GetRefUrl', 'get', { fpu: fup }, 'application/json',
        function (result) {
            if (result.res == 1) {
                openFormFromTree(result.resCon, result.resUrl,
                    function () {
                        history.pushState ? history.replaceState({}, document.title, location.protocol + "//" + location.host + location.pathname) : location.hash = '';
                    }
                );
            }
            else {
                showAlert('عنوان خاطئ', 'العنوان الذي تحاول الوصول إليه خطأ او غير موجود بالنظام <br> Err Code :' + result.resCon, 'error');
            }
        }
    );
}

function initTabs() {
    $('#apptbs').tabs({
        //onLoad: function (panel) {
        //    var target = this;
        //    if (typeof onFormTabSelect == 'function') {
        //        var t = $(target).tabs('getSelected');
        //        var i = $(target).tabs('getTabIndex', t);
        //        onFormTabSelect(t, i);
        //    }
        //},
        //onLoad: function (pnl) {
        //    alert('loaded');
        //},
        onSelect: function (title, index) {
            document.title = title + ' - ' + document.title.split('-')[1];
            var _cst = openedFrmsChanges.filter(function (e) { return e.frmId == currentOpenedFrmId; });
            if (_cst != null && _cst.length > 0)
                currentOpenedFrmId = _cst[0].frmId;
        },
        onBeforeClose: function (title, index) {
            var _ct = openedFrmsChanges.filter(function (e) { return e.frmId == currentOpenedFrmId; });
            if (_ct != null && _ct.length > 0) {
                var target = this;
                if (_ct[0].hasChanges) {
                    showConfirm3B('تنبيه', 'توجد بعض التعديلات لم يتم حفظها في ' + title + '<br/><br/><div style="width:100%;text-align:center;">هل تريد حفظ التغيرات؟</div>', function (r) {
                        var opts, bc;
                        if (r == 'y' && _ct[0].beforeCloseSaveChangesEV) {
                            _ct[0].beforeCloseSaveChangesEV(function () {
                                opts = $(target).tabs('options');
                                bc = opts.onBeforeClose;
                                opts.onBeforeClose = function () { };  // allowed to close now
                                $(target).tabs('close', index);
                                opts.onBeforeClose = bc;  // restore the event function
                            });
                        }
                        else if (r == 'n') {
                            opts = $(target).tabs('options');
                            bc = opts.onBeforeClose;
                            opts.onBeforeClose = function () { };  // allowed to close now
                            $(target).tabs('close', index);
                            opts.onBeforeClose = bc;  // restore the event function
                        }
                    });
                    return false;	// prevent from closing
                }
            }
        },
        onLoad: function (panel) {
            $(panel).find('.easyui-textbox[data-options*="editable:false"], .easyui-numberbox[data-options*="editable:false"]').each(function () {
                $(this).next('span').first().skipOnTab();
            });

            //var afFunc = $(panel).find('div[data-afterload]').data('afterload');
            //if (afFunc) eval(afFunc)(panel);
        },
        onClose: function (title, index) {
            openedFrmsChanges = openedFrmsChanges.filter(function (e) { return e.frmId !== currentOpenedFrmId; });
            currentOpenedFrmId = null;
        }
        //onAdd: function (title, index) {
        //   alert('add ' + index);
        //},
    });
}

function initTree() {
    $(g_treeCtrlId).tree({
        url: 'App/UserSys',
        onClick: function (node) {
            openFormFromTree(node.id);
        }
    });
}

function openFormFromTree(nodeId, nurl, afterOpenLinkCBFunc) {
    var node = $(g_treeCtrlId).tree('find', nodeId);
    if (node) {
        var lisLeaf = $(g_treeCtrlId).tree('isLeaf', node.target);
        if (lisLeaf) {
            if (node.attributes != null) {
                if ($(g_tabCtrlId).tabs('tabs').length == 7) {
                    showMessage(33);
                }
                else {
                    var tp = $(g_tabCtrlId).tabs('getTab', node.text);
                    if (tp) {
                        $(g_tabCtrlId).tabs('select', node.text);
                        var tiVal = $(g_tabCtrlId).tabs('getTabIndex', $(g_tabCtrlId).tabs('getSelected'));
                        var _ti = openedFrmsChanges.filter(function (e) { return e.tIndex == tiVal; });
                        if (_ti != null && _ti.length > 0)
                            currentOpenedFrmId = _ti[0].frmId;
                        else
                            currentOpenedFrmId = null;
                        $('#grpAppBody').layout('collapse', 'east');
                    }
                    else {
                        if (node.attributes.toString().startsWith('ORA')) {
                            doAjax('App/GetOUrl', 'get', null, 'text/html', function (ourl) {
                                $(g_tabCtrlId).tabs('add', {
                                    title: node.text,
                                    content: '<iframe src="' + ourl.replace('frm.fmx', node.attributes.toString().split('|')[2]).replace('frmid', node.attributes.toString().split('|')[1]) + '" style="width:100%;height:98%;z-index:0;"></iframe>',
                                    selected: true,
                                    closable: true
                                });
                            });
                        }
                        else if (node.attributes.toString().startsWith('NET')) {
                            var arrItemVals = node.attributes.toString().split('|');
                            $('#grpAppBody').layout('collapse', 'east');
                            $(g_tabCtrlId).tabs('add', {
                                title: node.text,
                                href: nurl ? nurl : arrItemVals[3],
                                selected: true,
                                closable: true,
                                onLoadError: function (e) {
                                    alert('Error Loading Form');
                                },
                                onLoad: function (pnl) {
                                    if (afterOpenLinkCBFunc)
                                        afterOpenLinkCBFunc();
                                },
                                extractor: function (data) {
                                    // TODO : Check for the http 401 login error code in the above onLoadError method.
                                    if (data)
                                        return data;    // no data returned, then connection is missed meaning that session is expired.
                                    else {
                                        forceRedirect = true;
                                        window.location.replace(window.location + 'User/Login');
                                    }
                                    //var pattern = /<body[^>]*>((.|[\n\r])*)<\/body>/im;
                                    //var matches = pattern.exec(data);
                                    //if (matches) {
                                    //    return matches[1];	// only extract body content
                                    //} else {
                                    //    return data;
                                    //}
                                }
                            });
                            if (arrItemVals[4].toLowerCase() == 'frm') {
                                currentOpenedFrmId = arrItemVals[1];
                                openedFrmsChanges.push({
                                    frmId: arrItemVals[1],
                                    tIndex: $(g_tabCtrlId).tabs('getTabIndex', $(g_tabCtrlId).tabs('getSelected')),
                                    hasChanges: false
                                });
                            }
                        }
                        else if (node.attributes.toString().startsWith('DEV')) {
                            showMessage(32);
                        }
                    }
                }
            }
        }
        else {
            $(g_treeCtrlId).tree(node.state == 'closed' ? 'expand' : 'collapse', node.target);//.tree('select', node.target);
        }
    }
    else {
        if (nurl && afterOpenLinkCBFunc)
            afterOpenLinkCBFunc();
    }
}

function closeTab() {
    var tab = $(g_tabCtrlId).tabs('getSelected');
    if (tab) {
        var index = $(g_tabCtrlId).tabs('getTabIndex', tab);
        $(g_tabCtrlId).tabs('close', index);
    }
}

function resetObject(objVal) {
    for (var prop in objVal) {
        if (objVal.hasOwnProperty(prop)) {
            objVal[prop] = null;
        }
    }
}

function openFormWithId(formUrl, id) {    
    $(g_tabCtrlId).tabs('add', {
        title: 'Id number : ' + id,
        href: formUrl + id,
        selected: true,
        closable: true,
        method: 'post',
        queryParams: { reqId: id },
        extractor: function (data) {
            if (data)
                return data;    // no data returned, then connection is missed meaning that session is expired.
            else {
                forceRedirect = true;
                window.location.replace(window.location + 'User/Login');
            }
        }
    });
}

// Global  easyui extension and updates.

$.extend($.fn.combobox.defaults.inputEvents, {
    focus: function (e) {
        var target = this;
        var len = $(target).val().length;
        setTimeout(function () {
            if (target.setSelectionRange) {
                target.setSelectionRange(0, len);
            } else if (target.createTextRange) {
                var range = target.createTextRange();
                range.collapse();
                range.moveEnd('character', len);
                range.moveStart('character', 0);
                range.select();
            }
        }, 0);
    }
    //, keydown: function (e) {
    //$(target).val().length;
    //if ($(e.data.target).combobox('options').mode == 'local') {
    //        //$(e.data.target).combobox('getText');
    //        var pnl = $(e.data.target).combobox('panel');
    //        var valP = $(e.data.target).combobox('options').valueField;
    //        var txtP = $(e.data.target).combobox('options').textField;
    //        var ob = $(e.data.target).combobox('getData').filter(function (e) { return e[txtP] == pnl.find('div.combobox-item-hover').text(); });
    //        var ob = $(e.data.target).combobox('options').filter(function (e) { return e[txtP] == pnl.find('div.combobox-item-hover').text(); });
    //        if (ob.length == 1)
    //            $(e.data.target).combobox('setValue', ob[0][valP]);
    //    }
    //}
    //else {
    //    if ($(e.data.target).combobox('options').editable) {
    //        downHandler.call(this);
    //    }
    //}
});

// A workaround to fix when the mouse click event is fired on a selected item in the easyui combobox dropdown panel.
$.extend($.fn.combobox.defaults, {
    onLoadSuccess: function (comboInstance) {
        var thisCombo = null;
        if (comboInstance && !(comboInstance instanceof Array))
            thisCombo = comboInstance;
        else
            thisCombo = this;
        if ($(thisCombo).combobox('options').mode == 'remote') {
            var vp = $(thisCombo).combobox('options').valueField;
            var tp = $(thisCombo).combobox('options').textField;
            $('.combobox-item-selected').on('click', function () {
                //if ($(this).hasClass('combobox-item-selected')) {
                var selTxt = this.textContent;
                var ob = $(thisCombo).combobox('getData').filter(function (e) { return e[tp] == selTxt; });
                if (ob.length == 1) {
                    $(thisCombo).combobox('unselect', ob[0][vp]);
                    $(thisCombo).combobox('select', ob[0][vp]);
                }
                //}
            });
        }
    }
});

//(function($){
//    var datebox = $.fn.datebox.defaults.onChange;
//    $.fn.datebox.defaults.onChange = function(newValue, oldValue){
//        $(this).closest('form').trigger('change');
//        datebox.call(this, newValue, oldValue);
//    };
//    var combobox = $.fn.combobox.defaults.onChange;
//    $.fn.combobox.defaults.onChange = function(newValue, oldValue){
//        $(this).closest('form').trigger('change');
//        combobox.call(this, newValue, oldValue);
//    };
//    ;


$.extend($.fn.validatebox.defaults.rules, {
    onlyMobileNum: {
        validator: function (value, param) {
            return value.match(/[0-9]/) && value.length > 8;
        },
        message: 'ادخل رقم جوال صحيح'
    }
});

// easyui tree extension methods.
$.extend($.fn.tree.methods, {
    getLevel: function (jq, target) {
        return $(target).find('span.tree-indent,span.tree-hit').length;
    },
    unselect: function (jq, target) {
        return jq.each(function () {
            var opts = $(this).tree('options');
            $(target).removeClass('tree-node-selected');
            if (opts.onUnselect) {
                opts.onUnselect.call(this, $(this).tree('getNode', target));
            }
        });
    }
});

function treeGetAllParentsOfNode(tree) {
    var node = $(tree).tree('getSelected');
    //alert("The selected node is: " + node.text);
    var nodeLevel = $(tree).tree('getLevel', node.target);
    var parentArry = new Array();
    var parents = treeGetParentArray(tree, node, nodeLevel, parentArry);
    return parents;
    //if (parents.length > 0) {
    //    var parentStr = "";
    //    for (var i = 0; i < parents.length; i++) {
    //        parentStr += parents[i].text + " -> "; // after modify parents[i] it works :)
    //    }
    //}
    //alert("All parents of selected node:\n" + parentStr);
}

function treeGetParentArray(tree, selectedNode, nodeLevel, parentArry) {
    //end condition: level of selected node equals 1, means it's root
    if (nodeLevel == 1) {
        return parentArry;
    } else {//if selected node isn't root
        nodeLevel -= 1;
        //the parent of the node
        var parent = $(tree).tree('getParent', selectedNode.target);
        //record the parent of selected to a array
        parentArry.unshift(parent);
        //recursive, to judge whether parent of selected node has more parent
        return treeGetParentArray(tree, parent, nodeLevel, parentArry);
    }
}

//$.fn.enterkeytab = function () {
//    $(this).on('keydown', 'input, select', function (e) {
//        var self = $(this)
//          //, form = self.parents('form:eq(0)')
//          , focusable
//          , next;
//        if (e.keyCode == 13) {
//            focusable = self.find('input,a,select,button').filter(':visible'); //form.
//            next = focusable.eq(focusable.index(this) + 1);
//            if (next.length) {
//                next.focus();
//            } else {
//                alert("wd");
//                //form.submit();
//            }
//            return false;
//        }
//    });
//}

var _gsAppHDP; // general System App Hijri Date Processor.

function getTodayDatesText() {
    var tod = new Date();
    var weekday = new Array("الأحد", "الإثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة", "السبت");
    var gMonths = new Array("يناير", "فبراير", "مارس", "إبريل", "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر");
    var hMonths = new Array("محرم", "صفر", "ربيع الأول", "ربيع الثاني", "جمادى الأول", "جمادى الآخر", "رجب", "شعبان", "رمضان", "شوال", "ذو القعدة", "ذو الحجة");
    var gregDateTxt = tod.getDate() + " " + gMonths[tod.getMonth()] + " " + tod.getFullYear();
    var hDate = _gsAppHDP.gHDate(tod);
    var hDateTxt = hDate.day + " " + hMonths[hDate.month - 1] + " " + hDate.year;

    return weekday[tod.getDay()] + " " + hDateTxt + " هـ " + " - " + gregDateTxt + " م";
}

function initUISystem(userNtfs, hijriInfo) {
    if (hijriInfo == null)
        window.location.replace(window.location + 'User/Logout');
    else {
        initUILayout();
        initTabs();
        initTree();
        $('#sysClock').thooClock({ size:200, rtl: true });        
        showUserNotifications(userNtfs);
        initNotifications();
        _gsAppHDP = new HijriDateProcessor(hijriInfo.hyl, hijriInfo.chd);
        $('#spnGSTodayDatesTxt').text(getTodayDatesText())
        if (document.location.href.indexOf('#') > -1)
            openRefUrl(document.location.href.split('#')[1]);
    }
}

function HijriDateProcessor(hijriYears, currentDate) {
    this.hYears = hijriYears;
    this.cDate = currentDate;
    this.year = function () { return parseInt(this.cDate.substr(0, 4)); };
    this.month = function () { return parseInt(this.cDate.substr(4, 2)); };
    this.day = function () { return parseInt(this.cDate.substr(6, 2)); };
    this.isValidStrDate = function (dateStr) {
        if (dateStr) {
            if (dateStr.indexOf('/') == -1)
                return false;
            else {
                var dateArr = dateStr.split('/');
                if (dateArr.length == 3) {
                    if (dateArr[0].length == 4 && dateArr[1].length == 2 && dateArr[2].length == 2)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
        }
        else
            return false;
    };
    this.gDateFromStr = function (hDateStr) {
        var hDateArr = hDateStr.split('/');
        var gDate = this.gDate(parseInt(hDateArr[0]), parseInt(hDateArr[1]), parseInt(hDateArr[2]));
        return gDate.getFullYear() + '/' + zeroPad((gDate.getMonth() + 1), 2) + '/' + zeroPad(gDate.getDate(), 2);
    };
    this.gDate = function (y, m, d) {
        var thiso = this;
        var yo = $.grep(this.hYears, function (e) { return e.year == (y ? y : thiso.year()); })[0];
        if (yo) {
            var pmdc = 0; var i = 1;
            var mv = parseInt(m); var dv = parseInt(d);
            while (i < (m ? mv : thiso.month())) {
                pmdc += yo['mon' + zeroPad(i, 2)];
                i++;
            }
            pmdc += (d ? dv : thiso.day());
            var dt = new Date(parseInt(yo.jDate.replace('/Date(', '')));
            dt.setDate(dt.getDate() + (pmdc - 1));
            return dt;
        }
        else {
            return new Date();
        }
    };
    this.gDay = function (y, m, d) {
        var d = this.gDate(y, m, d);
        return d.getDay();
    }
    this.isYearExist = function (y) {
        return $.grep(this.hYears, function (e) { return e.year == y; }).length > 0;
    }
    this.gMonthDays = function (y, m) {
        if (y.length != 4)
            y = this.year();
        return $.grep(this.hYears, function (e) { return e.year == y; })[0]['mon' + zeroPad(m, 2)];
    }
    this.gHDateString = function (y, m, d) {
        var dt = new Date(y, m, d);
        var thiso = this;
        var yo; var oyo = $.grep(thiso.hYears, function (e) {
            return g_checkDateIn(new Date(parseInt(e.jDate.replace('/Date(', ''))), g_addDays(new Date(parseInt(e.jDate.replace('/Date(', ''))), e.totalHDays), dt);
        });
        if (oyo)
            yo = oyo[0];
        else
            yo = $.grep(thiso.hYears, function (e) { return e.year == (y ? y : thiso.year()); })[0];

        var days = Math.floor((dt - new Date(parseInt(yo.jDate.replace('/Date(', '')))) / 86400000) + 1;
        var i = 1;
        while (i <= 12) {
            if (days <= yo['mon' + zeroPad(i, 2)])
                break;
            else
                days -= yo['mon' + zeroPad(i, 2)];
            i++;
        }
        return yo.year + '' + zeroPad(i, 2) + '' + zeroPad(days, 2);
    }
    this.gHDate = function (gDate) {
        var thiso = this;
        var yo; var oyo = $.grep(thiso.hYears, function (e) {
            return g_checkDateIn(new Date(parseInt(e.jDate.replace('/Date(', ''))), g_addDays(new Date(parseInt(e.jDate.replace('/Date(', ''))), e.totalHDays), gDate);
        });
        if (oyo)
            yo = oyo[0];
        else
            yo = $.grep(thiso.hYears, function (e) { return e.year == (y ? y : thiso.year()); })[0];

        var days = Math.floor((gDate - new Date(parseInt(yo.jDate.replace('/Date(', '')))) / 86400000) + 1;
        var i = 1;
        while (i <= 12) {
            if (days <= yo['mon' + zeroPad(i, 2)])
                break;
            else
                days -= yo['mon' + zeroPad(i, 2)];
            i++;
        }
        return { year: yo.year, month: i, day: days };
    }
    this.getArabicDayText = function (hDateVal) {
        var dayName = '';
        var dayNumber = this.gDay(hDateVal.split('/')[0], hDateVal.split('/')[1], hDateVal.split('/')[2]);
        switch (parseInt(dayNumber)) {
            case 0:
                dayName = 'الأحد';
                break;
            case 1:
                dayName = 'الإثنين';
                break;
            case 2:
                dayName = 'الثلاثاء';
                break;
            case 3:
                dayName = 'الأربعاء';
                break;
            case 4:
                dayName = 'الخميس';
                break;
            case 5:
                dayName = 'الجمعة';
                break;
            case 6:
                dayName = 'السبت';
                break;
        }
        return dayName;
    }
}

function g_addDays(stDate, noDays) {
    return new Date(stDate.getFullYear(), stDate.getMonth(), stDate.getDate() + noDays, stDate.getHours(), stDate.getMinutes(), stDate.getSeconds());
}

function g_checkDateIn(from, to, check) {
    if ((check <= to && check >= from)) {
        return true;
    }
    return false;
}