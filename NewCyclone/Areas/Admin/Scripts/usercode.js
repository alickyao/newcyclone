//通用提示
function showmsg(json) {
    if (json.code == 0) {
        //$.messager.alert('已完成', json.msg, 'info');
        toastr.info(json.msg);
    }
    else {
        $.messager.alert('发生异常', json.msg, 'error');
    }
}

//通用加载网格
function gridLoadData(Grid, json) {
    if (json.code == 0) {
        Grid.datagrid("loadData", json.result);
    }
    else {
        Grid.datagrid({
            columns: [[
                { field: 'msg', title: '发生错误' }
            ]]
        });
        Grid.datagrid("loadData", { total: 1, rows: [{ msg: json.msg }] });
    }
}

//播放系统提示音
function playSysNotice() {
    $("#sysNoticePlayer")[0].play();
}