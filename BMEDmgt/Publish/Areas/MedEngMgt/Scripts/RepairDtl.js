function showmsg2(data) {
    if (!data.success) {
        alert(data.error);
    }
    else {
        alert("儲存成功!");
    }
}

$(function () {
    $(".datefield").datepicker({
        format: "yyyy/mm/dd"
    });
    var showREPCOST = function (item) {
        if (item == "Y") {
            $('a[href="#repaircost"]').show();
        }
        else if (item == "N") {
            $('a[href="#repaircost"]').hide();
        }
    };
    //
    showREPCOST($('input:checked:radio[name="IsCharged"]').val());
    //

    $('input:radio[name="IsCharged"]').click(function () {
        var item = $(this).val();
        showREPCOST(item);
    });
});