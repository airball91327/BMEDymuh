function showmsg4(data) {
    if (data.success !== null) {
        if (!data.success)
            alert(data.error);
        else
            alert("儲存成功!");
    }
}

$(function () {

    $("#pnlREPEMPLIST a").click(function () {

        if (confirm("確定要刪除此資料?")) {
            var id = $("#DocId").val();
            var tr = $(this).parents('tr');
            var seq = $(this).parents('tr').children();
            $.ajax({
                url: "../../RepairEmps/Delete",
                type: "POST",
                data: { 'id': id, 'uid': seq.get(0).innerText.trim() },
                async: true,
                dataType: "json",
                success: function (data) {
                    if (data.success) {
                        tr.remove();
                    }
                    else {
                        alert(data.error);
                    }
                }
            });
            return false;
        }
        else
            return false;
    });
});