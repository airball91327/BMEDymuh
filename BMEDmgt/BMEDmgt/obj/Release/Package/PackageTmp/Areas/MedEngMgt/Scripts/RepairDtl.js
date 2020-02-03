function showmsg2(data) {
    if (!data.success) {
        alert(data.error);
    }
    else {
        alert("儲存成功!");

        $.ajax({
            url: "../../RepairDtls/DealDesList",
            type: "GET",
            data: { 'id': $("#DocId").val() },
            async: true,
            success: function (data) {
                //console.log(data);
                $("#pnlREPDESLIST").html(data);
            }
        });
    }
}

$(function () {
    $("#pnlPURCHASE").hide();
    $(".datefield").datepicker({
        format: "yyyy/mm/dd"
    });
    var showREPCOST = function (item) {
        if (item === "Y") {
            $("#pnlPURCHASE").show();
            $('a[href="#repaircost"]').show();
        }
        else if (item === "N") {
            $("#pnlPURCHASE").hide();
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