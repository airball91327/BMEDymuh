$(function () {
    $('.combobox').combobox();

    $('.modal').on('shown.bs.modal', function () {
        //Make sure the modal and backdrop are siblings (changes the DOM)
        $(this).before($('.modal-backdrop'));
        //Make sure the z-index is higher than the backdrop
        $(this).css("z-index", parseInt($('.modal-backdrop').css('z-index')) + 1);
    });
    $('#modalFILES').on('hidden.bs.modal', function () {
        var docid = $("#DocId").val();
        $.ajax({
            url: '../AttainFiles/List2',
            type: "POST",
            data: { id: docid, typ: "2" },
            success: function (data) {
                $("#pnlFILES").html(data);
            }
        });
    });
    $('#modalVENDOR').on('hidden.bs.modal', function () {
        var vno = $("#Vno option:selected").val();
        var vname = $("#Vno option:selected").text();
        if ($('.nav-pills .active').text() === "簽核作業") {
            $("#FlowVendor").val(vno);
            $("#VendorName2").val(vname);
            $("#FlowVendor").trigger("change");
        }
        else {
            $("#VendorId").val(vno);
            $("#VendorName").val(vname);
        }
    });
   
})