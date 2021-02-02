function showmsg(data) {
    $("#btnGO").attr("disabled",false);
    if (!data.success) {
        alert(data.error);
    }
    else {
        //location.replace("../../Home");
        //window.opener.jQuery("#btnQTY").trigger("click");
        //window.opener.jQuery("#btnQTY4").trigger("click");
        alert("送出成功!");
        self.close();
    }
}

function presend() {
    //alert('test');
    document.getElementById('btnGO').disabled = true;
}

$.fn.addItems = function (data) {

    return this.each(function () {
        var list = this;
        $.each(data, function (val, text) {

            var option = new Option(text.Text, text.Value);
            list.add(option);
        });
    });

};

$(function () {
    $('#btnSelUsr').hide();
    $('#pnlFLOWVENDOR').hide();
    $('#pnlCANCLOSE').hide();
    $('#pnlASSET').hide();

    if ($('#ClsNow').val() === "維修工程師" || $('#ClsNow').val() === "設備工程師") {
        $('#pnlCANCLOSE').show();
        $('#pnlASSET').show();
    }
    if ($('#ClsNow').val() === "申請人") {
        $('#pnlASSET').show();
    }

    $('#FlowCls').change(function () {
        $('#btnSelUsr').hide();
        $('#pnlFLOWVENDOR').hide();
        var select = $('#FlowUid');
        $('option', select).remove();
        if ($(this).val() === "維修工程師") {
            $('#pnlFLOWVENDOR').show();
        }
        //else if ($(this).val() === "驗收人") {
        //    $('#btnSelUsr').show();
        //}
        else if ($(this).val() === "結案" || $(this).val() === "廢除") {
            var appenddata;
            appenddata += "<option value = '0' selected=true></option>";
            select.html(appenddata);
        }
        else {
            if ($(this).val() === "驗收人") {
                $('#btnSelUsr').show();
            }
            $('#FlowVendor').val('');
            $('#imgLOADING').show();
            var docid = $('#DocId').val();
            $.ajax({
                url: '../../RepairFlows/GetNextEmp',
                type: "POST",
                dataType: "json",
                data: "cls=" + $(this).val() + "&docid=" + docid,
                success: function (data) {
                    $('#imgLOADING').hide();
                    if (data.success === false) {
                        $('#FlowCls').val('請選擇');
                        alert(data.error);
                    }
                    else {
                        var select = $('#FlowUid');
                        $('option', select).remove();
                        select.addItems(data);
                    }
                }
            });
        }
    });

    $('#FlowVendor').change(function () {
        $('#imgLOADING').show();
        $.ajax({
            url: '../../RepairFlows/GetNextEmp',
            type: "POST",
            dataType: "json",
            data: "cls=維修工程師&docid=" + $('#DocId').val() + "&vendor=" + $(this).val(),
            success: function (data) {
                $('#imgLOADING').hide();
                if (data.success === false) {
                    $('#FlowCls').val('請選擇');
                    alert(data.error);
                }
                else {
                    var select = $('#FlowUid');
                    $('option', select).remove();
                    select.addItems(data);
                }
            }
        });
    });
    $("#btnQtyAsset").click(function () {
        var keynam = $("#AssetKeyName").val();
        if (keynam !== "") {
            $.ajax({
                contentType: "application/json; charset=utf-8",
                url: '../../Assets/GetAssetsByKeyname',
                type: "GET",
                data: { keyname: keynam },
                dataType: "json",
                success: function (data) {
                    //var s = '[{"ListKey":"44","ListValue":"test1"},{"ListKey":"87","ListValue":"陳奕軒"}]';
                    var jsdata = JSON.parse(data);
                    var appenddata;
                    appenddata += "<option value = ''>請選擇</option>";
                    $.each(jsdata, function (key, value) {
                        appenddata += "<option value = '" + value.AssetNo + "'>" + value.Cname + " </option>";
                    });
                    $('#AssetNo').html(appenddata);
                },
                error: function (msg) {
                    alert(msg);
                }
            });
        }
    });
    $('.modal').on('shown.bs.modal', function () {
        //Make sure the modal and backdrop are siblings (changes the DOM)
        $(this).before($('.modal-backdrop'));
        //Make sure the z-index is higher than the backdrop
        $(this).css("z-index", parseInt($('.modal-backdrop').css('z-index')) + 1);
    });
    $('#modalSELUSER').on('hidden.bs.modal', function () {
        var select = $('#FlowUid');
        var selitem = $('#Suserid option:selected');
        if (selitem.val() !== "") {
            $('option', select).remove();
            var appenddata;
            appenddata += "<option value = ''>請選擇</option>";
            appenddata += "<option value = '" + selitem.val() + "' selected=true>" + selitem.text() + " </option>";
            select.html(appenddata);
        }
    });
   
    $('input[name="AssignCls"]:radio').change(function () {
        if ($(this).val() === "同意") {
            $("#FlowCls option").each(function () {
                if ($(this).val() === "結案") {
                    $(this).prop('disabled', false);
                }
            });
        }
        else {
            $("#FlowCls option").each(function () {
                if ($(this).val() === "結案") {
                    if ($(this).is(":selected"))
                    {
                        $('#FlowCls option[value=""]').prop('selected', true);
                    }
                    $(this).prop('disabled', true);
                }
            });
        }
    }); 
});
