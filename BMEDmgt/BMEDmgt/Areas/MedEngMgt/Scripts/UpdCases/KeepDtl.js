﻿function showmsg2(data)
{
    $("btnKEEPDTL").attr("disabled", false);
    if (!data.success)
    {
        alert(data.error);
    }
    else
    {
        alert("儲存成功!");
    }
};

function BeforeSend() {
    //alert('test');
    document.getElementById('btnKEEPDTL').disabled = true;
};

$(function () {
    
    var showREPCOST = function(item){
        if (item == "Y") {
            $('a[href="#keepcost"]').show();
        }
        else if (item == "N") {
            $('a[href="#keepcost"]').hide();
        }
    };
    //
    showREPCOST($('input:checked:radio[name="IsCharged"]').val());
    //
    $(".datefield").datepicker({
        format: "yyyy/mm/dd"
    });
    //$("#EndDate").datepicker({
    //    format: "yyyy/mm/dd"
    //});

    $('input:radio[name="IsCharged"]').click(function () {
        var item = $(this).val();
        showREPCOST(item);
    });
});