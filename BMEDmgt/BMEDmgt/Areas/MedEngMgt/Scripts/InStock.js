function showmsg3(data) {
    if (data.success == null) {
        alert("儲存成功!");
        $("#pnlREPCOSTLIST").html(data);
    }
    else {
        if (data.success === false) {
            alert(data.error);
        }
    }
}

$(function () {
    $("#TotalAmt").attr('readonly', true);
    $('.modal').on('shown.bs.modal', function () {
        //Make sure the modal and backdrop are siblings (changes the DOM)
        $(this).before($('.modal-backdrop'));
        //Make sure the z-index is higher than the backdrop
        $(this).css("z-index", parseInt($('.modal-backdrop').css('z-index')) + 1);
    });
    $('#modalSTOK').on('hidden.bs.modal', function () {
        var $obj = $('input:radio[name="rblSELECT"]:checked').parents('tr').children();
        if ($obj.length > 0) {
            $("#StokNo").val($obj.get(0).innerText.trim());
            $("#StokNam").val($obj.get(1).innerText.trim());
            //$("#Unite").val($obj.get(2).innerText.trim());
            //$("#Price").val($obj.get(3).innerText.trim());
            $("#TotalAmt").val($obj.get(4).innerText.trim());
            }
        });

    $('input:radio[name="RecordType"]').click(function () {
        $('#DocId').hide();
        var item = $(this).val();
        if (item === "借調") {
            $('#DocId').hide();
            $('#DocId').val('');
        }
        else {
            $('#DocId').show();
        }
    });

});