function showmsg3(data) {
    if (data.success == null) {
        alert("儲存成功!");
        $("#pnlKEEPCOSTLIST").html(data);
    }
    else {
        if (data.success === false) {
            alert(data.error);
        }
    }
}

$(function () {
    $("#Price").attr('readonly', true);
    $("#TotalCost").attr('readonly', true);
    $(".datefield").datepicker({
        format: "yyyy/mm/dd"
    });
    //$("#AccountDate").datepicker({format:"yyyy/mm/dd"});
    $('.modal').on('shown.bs.modal', function () {
        //Make sure the modal and backdrop are siblings (changes the DOM)
        $(this).before($('.modal-backdrop'));
        //Make sure the z-index is higher than the backdrop
        $(this).css("z-index", parseInt($('.modal-backdrop').css('z-index')) + 1);
    });
    $('#modalSTOK').on('hidden.bs.modal', function () {
        var $obj = $('input:radio[name="rblSELECT"]:checked').parents('tr').children();
        if ($obj.length > 0) {
            $("#PartNo").val($obj.get(0).innerText.trim());
            $("#PartName").val($obj.get(1).innerText.trim());
            $("#Unite").val($obj.get(2).innerText.trim());
            $("#Price").val($obj.get(3).innerText.trim());
            var v1 = $("#Price").val();
            var v2 = $("#Qty").val();
            if (v1 != null && v2 != null) {
                $("#TotalCost").val(v1 * v2);
            }
            $("#VendorId").val('000');
            //$("#VendorName").val($obj.get(4).innerText.trim());
        }               
    })

    $('input:radio[name="StockType"]').click(function () {
        $('#PartName').attr('readonly',false);
        $('#Price').attr('readonly',false);
        var item = $(this).val();
        if (item == "2") {
            $('#btnQtyStok').hide();
            $("#pnlSIGN").hide();
            $("#pnlACCDATE").show();
            $("#CVendor").show();
            $("#pnlTICKET").show();
            $('label[for="AccountDate"]').text("發票日期");
        }
        else if (item == "1") {
            $("#pnlTICKET").hide();
            $("#pnlACCDATE").show();
            $("#pnlSIGN").show();
            $('label[for="AccountDate"]').text("簽單日期");
            $('input:radio[name="IsPetty"]')
                .prop("disabled", true);
        }
        else {
            $('#btnQtyStok').show();
            $('#PartName').attr('readonly', true);
            $('#Price').attr('readonly', true);
            $("#CVendor").hide();
            $("#pnlTICKET").hide();
            $("#pnlSIGN").hide();
            $("#pnlACCDATE").hide();
        }
    });

    $('#Price').change(function () {
        var v1 = $(this).val();
        var v2 = $('#Qty').val();
        if (v1 != null && v2 != null) {
            $('#TotalCost').val(v1 * v2);
        }
    });
    $('#Qty').change(function () {
        var v1 = $(this).val();
        var v2 = $('#Price').val();
        if (v1 != null && v2 != null) {
            $('#TotalCost').val(v1 * v2);
        }
    });
    
    $("#btnGETSEQ").click(function () {
        $.ajax({
            url: '../../Tickets/GetTickeSeq',
            type: "POST",
            async: true,
            success: function (data) {
                $("#TicketDtl_TicketDtlNo").val("AA" + data);
            }
        });

    });

});