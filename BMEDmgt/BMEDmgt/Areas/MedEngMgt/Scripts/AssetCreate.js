function showmsg(data) {
    if (!data.success) {
        alert(data.error);
    }
    else {
        alert("儲存成功!");
        location.replace('./Edit?ano='+ data.id);
    }
}

$(function () {
    $(".datefield").datepicker({
        format: "yyyy/mm/dd"
    });

    $('input:radio[name="AssetClass"][value="醫工"]')
        .prop("checked", true);

    $("#DelivDpt").change(function () {
        var s = $(this).val();
        $.ajax({
            contentType: "application/json; charset=utf-8",
            url: '../AppUsers/GetUsersInDpt',
            type: "GET",
            data: "id=" + s,
            dataType: "json",
            success: function (data) {
                //var s = '[{"ListKey":"44","ListValue":"test1"},{"ListKey":"87","ListValue":"陳奕軒"}]';
                var jsdata = JSON.parse(data);
                var appenddata;
                appenddata += "<option value = ''>請選擇</option>";
                $.each(jsdata, function (key, value) {
                    appenddata += "<option value = '" + value.uid + "'>" + value.uname + " </option>";
                });
                $('#DelivUid').html(appenddata);
            },
            error: function (msg) {
                alert(msg);
            }
        });
    });

    $("#btnQtyDelivUid").click(function () {
        var keynam = $("#DelivUidKeyName").val();
        if (keynam == "") {
            $("#DelivDpt").trigger("change");
        }
        else {
            $.ajax({
                contentType: "application/json; charset=utf-8",
                url: '../AppUsers/GetUsersByKeyname',
                type: "GET",
                data: { id: "", keyname: keynam },
                dataType: "json",
                success: function (data) {
                    //var s = '[{"ListKey":"44","ListValue":"test1"},{"ListKey":"87","ListValue":"陳奕軒"}]';
                    var jsdata = JSON.parse(data);
                    var appenddata;
                    appenddata += "<option value = ''>請選擇</option>";
                    $.each(jsdata, function (key, value) {
                        appenddata += "<option value = '" + value.uid + "'>" + value.uname + " </option>";
                    });
                    $('#DelivUid').html(appenddata);
                },
                error: function (msg) {
                    alert(msg);
                }
            });
        }
    });
    $("#btnQtyEngId").click(function () {
        var keynam = $("#EngIdKeyName").val();
        if (keynam == "") {
        }
        else {
            $.ajax({
                contentType: "application/json; charset=utf-8",
                url: '../AppUsers/GetUsersByKeyname',
                type: "GET",
                data: { id: "", keyname: keynam },
                dataType: "json",
                success: function (data) {
                    //var s = '[{"ListKey":"44","ListValue":"test1"},{"ListKey":"87","ListValue":"陳奕軒"}]';
                    var jsdata = JSON.parse(data);
                    var appenddata;
                    appenddata += "<option value = ''>請選擇</option>";
                    $.each(jsdata, function (key, value) {
                        appenddata += "<option value = '" + value.uid + "'>" + value.uname + " </option>";
                    });
                    $('#EngId').html(appenddata);
                },
                error: function (msg) {
                    alert(msg);
                }
            });
        }
    });
    $("#btnQtyBmedNo").click(function () {
        var keynam = $("#BmedNoKeyName").val();
        $.ajax({
            contentType: "application/json; charset=utf-8",
            url: '../DeviceClassCodes/GetDataByKeyname',
            type: "GET",
            data: { id: "", keyname: keynam },
            dataType: "json",
            success: function (data) {
                //var s = '[{"ListKey":"44","ListValue":"test1"},{"ListKey":"87","ListValue":"陳奕軒"}]';
                var jsdata = JSON.parse(data);
                var appenddata;
                appenddata += "<option value = ''>請選擇</option>";
                $.each(jsdata, function (key, value) {
                    appenddata += "<option value = '" + value.M_code + "'>" + value.M_name + " </option>";
                });
                $('#BmedNo').html(appenddata);
            },
            error: function (msg) {
                alert(msg);
            }
        });
    });

    $("#btnQtyAccdpt").click(function () {
        var keynam = $("#AccdptKeyName").val();
        $.ajax({
            contentType: "application/json; charset=utf-8",
            url: '../Departments/GetDptsByKeyname',
            type: "GET",
            data: { keyname: keynam },
            dataType: "json",
            success: function (data) {
                //var s = '[{"ListKey":"44","ListValue":"test1"},{"ListKey":"87","ListValue":"陳奕軒"}]';
                var jsdata = JSON.parse(data);
                var appenddata;
                appenddata += "<option value = ''>請選擇</option>";
                $.each(jsdata, function (key, value) {
                    appenddata += "<option value = '" + value.dptid + "'>" + value.dptname + " </option>";
                });
                $('#AccDpt').html(appenddata);
            },
            error: function (msg) {
                alert(msg);
            }
        });
    });

    $("#btnQtyDelivdpt").click(function () {
        var keynam = $("#DelivdptKeyName").val();
        $.ajax({
            contentType: "application/json; charset=utf-8",
            url: '../Departments/GetDptsByKeyname',
            type: "GET",
            data: { keyname: keynam },
            dataType: "json",
            success: function (data) {
                //var s = '[{"ListKey":"44","ListValue":"test1"},{"ListKey":"87","ListValue":"陳奕軒"}]';
                var jsdata = JSON.parse(data);
                var appenddata;
                appenddata += "<option value = ''>請選擇</option>";
                $.each(jsdata, function (key, value) {
                    appenddata += "<option value = '" + value.dptid + "'>" + value.dptname + " </option>";
                });
                $('#DelivDpt').html(appenddata);
            },
            error: function (msg) {
                alert(msg);
            }
        });
    });
    $('.modal').on('shown.bs.modal', function () {
        //Make sure the modal and backdrop are siblings (changes the DOM)
        $(this).before($('.modal-backdrop'));
        //Make sure the z-index is higher than the backdrop
        $(this).css("z-index", parseInt($('.modal-backdrop').css('z-index')) + 1);
    });
    $('#modalVENDOR').on('hidden.bs.modal', function () {
        var vno = $("#Vno option:selected").val();
        var vname = $("#Vno option:selected").text();
        $("#VendorId").val(vno);
        $("#VendorName").val(vname);
        ChangeBtnUrl(vno);
    });

});