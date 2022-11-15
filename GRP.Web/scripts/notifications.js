function showUserNotifications(ntfsLst) {
    if (ntfsLst) {
        for (var i in ntfsLst) {
            addNotificationUI(ntfsLst[i]);
        }
    }
}

function initNotifications() {
    var hubCon = $.hubConnection('signalr', { useDefaultPath: false });

    /*hubCon.disconnected(function () {
        alert('disconnected');
        //setTimeout(function () {
        //    $.connection.hub.start().done(function () {
        //        $("#mySignalRConnectionIdHidden").val($.connection.hub.id);
        //    });
        //}, 3000);
    });*/

    hubCon.error(function (err) {
        //showAlert('SignalR Error', err, 'error');
        console.log(err);
    });
    var hubProxy = hubCon.createHubProxy('reqNotifyHub');

    hubProxy.on('addRequestNotification', function (notification) {
        addNotificationUI(notification);
    });
    hubProxy.on('removeRequestNotification', function (notification) {
        removeNotificationUI(notification);
    });
    hubCon.start();
    //hubCon.start(function () { hub.invoke('RecordHit'); });
}

function addNotificationUI(notificationObj) {
    var nCount = 0;
    var nContainer = $('.layout-expand-west .panel-body .layout-expand-title span.m-badge'); 
    if (nContainer.length > 0) {
        nCount = parseInt(nContainer.text()) + 1;
        nContainer.text(nCount);
    }
    else
        $('.layout-expand-west .panel-body .layout-expand-title').append('<span class="m-badge">1</span>');
    addNotificationDetailsUI(notificationObj);
}

function addNotificationDetailsUI(nObject) {
    var nChildCount = 0;
    var pnlGroup = _.find($('#dSysNotificationsWest').accordion('panels'), function (panel) { return panel.panel('options').id == 'g_' + nObject.GroupName; });
    if (pnlGroup) {
        nChildCount = parseInt($('#g_' + nObject.GroupName).prev().find('.panel-title span.m-badge').text()) + 1;
        $('#lst_' + nObject.GroupName).datagrid('appendRow', { text: nObject.Text });
        $('#lst_' + nObject.GroupName).append('<li id="' + nObject.Id + '" data-ourl="' + nObject.Params['url'] + '" data-opId="' + nObject.Params['rId'] + '" data-tbTxt="' + nObject.Params['tabTxt'] + '"></li>');
        $('#g_' + nObject.GroupName).prev().find('.panel-title span.m-badge').text(nChildCount);
    }
    else {
        $('#dSysNotificationsWest').accordion('add', {
            id: 'g_' + nObject.GroupName, title: nObject.GroupText,
            content: '<ul id="lst_' + nObject.GroupName + '" class="easyui-datalist" data-options="onClickCell:notificationItemClick" lines="true" style="width:100%;">\
                      <li id="' + nObject.Id + '" data-ourl="' + nObject.Params['url'] + '" data-opId="' + nObject.Params['rId'] + '" data-tbTxt="' + nObject.Params['tabTxt'] + '">' + nObject.Text + '</li></ul>',
            selected: true
        });
        $('#g_' + nObject.GroupName).prev().find('.panel-title').append('<span class="m-badge">1</span>');
    }
}

function notificationItemClick(index, field, value) {
    var ntItem = $('#' + this.id).children().eq(index);

    var tp = $('#apptbs').tabs('getTab', ntItem.attr('data-tbTxt'));
    if (tp) {
        $('#apptbs').tabs('select', ntItem.attr('data-tbTxt'));
        var tiVal = $('#apptbs').tabs('getTabIndex', $('#apptbs').tabs('getSelected'));
        var _ti = openedFrmsChanges.filter(function (e) { return e.tIndex == tiVal; });
        if (_ti != null && _ti.length > 0)
            currentOpenedFrmId = _ti[0].frmId;
        else
            currentOpenedFrmId = null;
    }
    else {
        $('#apptbs').tabs('add', {
            title: ntItem.attr('data-tbTxt'),
            href: ntItem.attr('data-ourl'),
            selected: true,
            closable: true,
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
    //if (arrItemVals[4].toLowerCase() == 'frm') {
    currentOpenedFrmId = ntItem.attr('data-opId');
    openedFrmsChanges.push({
        frmId: ntItem.attr('data-opId'),
        tIndex: $('#apptbs').tabs('getTabIndex', $('#apptbs').tabs('getSelected')),
        hasChanges: false
    });
    //}

    $('#grpAppBody').layout('collapse', 'west');
    //$('#grpAppBody').layout('collapse', 'east');
}

function removeNotificationUI(notificationObj) {
    var nCount = 0;
    var nContainer = $('.layout-expand-west .panel-body .layout-expand-title span.m-badge');
    var removed = removeNotificationDetailsUI(notificationObj);
    if (removed) {
        nCount = parseInt(nContainer.text()) - 1;
        if (nCount == 0)
            nContainer.remove();
        else
            nContainer.text(nCount);
    }
}

function removeNotificationDetailsUI(nObject) {
    var nCount = parseInt($('#g_' + nObject.GroupName).prev().find('.panel-title span.m-badge').text()) - 1;

    var nItem = $('#lst_' + nObject.GroupName + ' > li[data-opId="' + nObject.Params['rId'] + '"]');
    if (nItem.length == 0)
        return false;

    $('#lst_' + nObject.GroupName).datagrid('deleteRow', nItem.index());
    nItem.remove();

    if (nCount == 0) {
        $('#lst_' + nObject.GroupName).datagrid('getPanel').panel('destroy'); // remove list
        $('#dSysNotificationsWest').accordion('remove',
            $('#dSysNotificationsWest').accordion('getPanelIndex',
                _.find($('#dSysNotificationsWest').accordion('panels'), function (panel) { return panel.panel('options').id == 'g_' + nObject.GroupName; }))); // remove accordion panel.
    }
    else
        $('#g_' + nObject.GroupName).prev().find('.panel-title span.m-badge').text(nCount);

    return true;
}