function showmsg(data) {
    if (data.success === false) {
        alert(data.error);
    }
}

$(function () {
    $(".datefield").datepicker({
        format: "yyyy/mm/dd"
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
                $('#qtyACCDPT').html(appenddata);
            },
            error: function (msg) {
                alert(msg);
            }
        });
    });

    $("#btnQtyApplydpt").click(function () {
        var keynam = $("#ApplydptKeyName").val();
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
                $('#qtyDPTID').html(appenddata);
            },
            error: function (msg) {
                alert(msg);
            }
        });
    });

    $("#btnQtyFlow").click(function () {
        var keynam = $("#FlowKeyName").val();
        $.ajax({
            contentType: "application/json; charset=utf-8",
            url: '../AppUsers/GetUsersByKeyname',
            type: "GET",
            data: { keyname: keynam },
            dataType: "json",
            success: function (data) {
                //var s = '[{"ListKey":"44","ListValue":"test1"},{"ListKey":"87","ListValue":"陳奕軒"}]';
                var jsdata = JSON.parse(data);
                var appenddata;
                appenddata += "<option value = ''>請選擇</option>";
                $.each(jsdata, function (key, value) {
                    appenddata += "<option value = '" + value.uid + "'>" + value.uname + " </option>";
                });
                $('#qtyFLOW').html(appenddata);
            },
            error: function (msg) {
                alert(msg);
            }
        });
    });

})