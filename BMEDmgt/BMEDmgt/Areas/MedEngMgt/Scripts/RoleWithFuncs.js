function showmsg(data) {
    if (!data.success) {
        alert(data.error);
    }
    else {
        alert("儲存成功!");
    }
}

$(function () {
    
    $("#btnQRY").click(function () {
        var roleid = $("#RoleId").val();
        var module = $("#Module").val();
        document.location.href = "./RoleWithFuncs?roleid=" + roleid + "&module=" + module;
    });

    $("#Module").change(function () {
        $("#btnSEND").prop("disabled", true);
    });

    $("#RoleId").change(function () {
        $("#btnSEND").prop("disabled", true);
    });

});