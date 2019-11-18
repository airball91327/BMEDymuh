function showmsg(data) {
    $("#btnGO").attr("disabled",false);
    if (!data.success) {
        alert(data.error);
    }
    else {
        alert("送出成功!");
        location.replace("../../Home");
    }
};

function presend() {
    //alert('test');
    document.getElementById('btnGO').disabled = true;
};

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

    $('#FlowCls').change(function () {
        $('#btnSelUsr').hide();
        $('#pnlFLOWVENDOR').hide();
        var select = $('#FlowUid');
        $('option', select).remove();
        if ($(this).val() == "維修工程師") {
            $('#pnlFLOWVENDOR').show();
        }
        else if ($(this).val() == "驗收人")
        {
            $('#btnSelUsr').show();
        }
        else if ($(this).val() == "結案" || $(this).val() == "廢除")
        {
            var appenddata;
            appenddata += "<option value = '0' selected=true></option>";
            select.html(appenddata);
        }
        else {
            $('#FlowVendor').val('');
            $('#imgLOADING').show();
            var docid = $('#DocId').val();
            $.ajax({
                url: '../../KeepFlows/GetNextEmp',
                type: "POST",
                dataType: "json",
                data: "cls=" + $(this).val() + "&docid=" + docid,
                success: function (data) {
                    $('#imgLOADING').hide();
                    if (data.success == false) {
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
            url: '../../KeepFlows/GetNextEmp',
            type: "POST",
            dataType: "json",
            data: "cls=維修工程師&docid=" + $('#DocId').val() + "&vendor=" + $(this).val(),
            success: function (data) {
                $('#imgLOADING').hide();
                if (data.success == false) {
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

    $('#modalSELUSER').on('hidden.bs.modal', function () {
        var select = $('#FlowUid');
        var selitem = $('#Suserid option:selected');
        if (selitem.val() != "") {
            $('option', select).remove();
            var appenddata;
            appenddata += "<option value = ''>請選擇</option>";
            appenddata += "<option value = '" + selitem.val() + "' selected=true>" + selitem.text() + " </option>";
            select.html(appenddata);
        }
    });

    $('input[name="AssignCls"]:radio').change(function () {
        if ($(this).val() == "同意") {
            $("#FlowCls option").each(function () {
                if ($(this).val() == "結案") {
                    $(this).prop('disabled', false);
                }
            });
        }
        else {
            $("#FlowCls option").each(function () {
                if ($(this).val() == "結案") {
                    if ($(this).is(":selected")) {
                        $('#FlowCls option[value=""]').prop('selected', true);
                    }
                    $(this).prop('disabled', true);
                }
            });
        }
    });
});