//function showmsg4(data) {
//    if (data.success != null) {
//        if(!data.success)
//            alert(data.error);
//        else
//            alert("儲存成功!");
//    }
//}

$(function () {

    //$("#pnlKEEPEMPLIST a").click(function () {

    //    if (confirm("確定要刪除此資料?")) {
    //        var id = $("#DocId").val();
    //        var tr = $(this).parents('tr');
    //        var seq = $(this).parents('tr').children();
    //        $.ajax({
    //            url: "../../KeepEmps/Delete",
    //            type: "POST",
    //            data: { 'docid': id, 'uid': seq.get(0).innerText.trim() },
    //            async: true,
    //            dataType: "json",
    //            success: function (data) {
    //                if (data.success) {
    //                    tr.remove();
    //                }
    //                else {                        
    //                    alert(data.error);
    //                }                    
    //            }
    //        });
    //        return false;
    //    }
    //    else
    //        return false;
    //});
    $("#btnKPEMP").click(function () {
        $("#imgLOADING4").show();
        var docid = $("#DocId").val();
        var s = $("#fmKEEPEMP").serialize();
        $.ajax({
            url: "../KeepEmps/UpdCases",
            type: "POST",
            data: s,
            async: true,
            dataType: "json",
            success: function (data) {
               
                if (data.success) {
                    $.ajax({
                        url: '../KeepEmps/UpdCasesList',
                        type: "POST",
                        data: { id: docid },
                        success: function (data) {
                            $('#pnlKEEPEMPLIST').html(data);
                        }
                    });
                }
                else {
                    alert(data.error);
                }
                
            }
        });
        $("#imgLOADING4").hide();
    });
});