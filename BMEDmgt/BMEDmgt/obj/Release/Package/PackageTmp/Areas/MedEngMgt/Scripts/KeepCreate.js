function showmsg(data) {
    if (data.success != null) {
        if (!data.success)
            alert(data.error);
        else {
            alert("送出成功!");
            //window.open('./Print?id=' + $("#DocId").val());
            location.replace("../Home/Index");
        }

    }
}

$(function () {
    $("#btnQtyChecker").click(function () {
        var keynam = $("#CheckerKeyName").val();
        if (keynam == "") {
            $("#AssetNo").trigger("change");
        }
        else {
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
                    $('#CheckerName').html(appenddata);
                },
                error: function (msg) {
                    alert(msg);
                }
            });
        }
    });

    $("#btnQtyAsset").click(function () {
        var keynam = $("#AssetKeyName").val();
        var accDptId = $('#AccDpt').val();
        if (keynam != "") {
            $.ajax({
                contentType: "application/json; charset=utf-8",
                url: '../Assets/GetAssetsByKeynameAndAcc',
                type: "GET",
                data: { keyname: keynam, accDpt: accDptId },
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

    //$("#AssetNo").change(function () {
    //    $("#AssetName").val($("#AssetNo option:selected").text());
    //});

    $("#AccDpt").change(function () {
        $("#AccDptName").val($("#AccDpt option:selected").text());
        /*
        $.ajax({
            contentType: "application/json; charset=utf-8",
            url: '../AppUsers/GetUsersInDpt',
            type: "GET",
            data: { id: $(this).val()},
            dataType: "json",
            success: function (data) {
                //var s = '[{"ListKey":"44","ListValue":"test1"},{"ListKey":"87","ListValue":"陳奕軒"}]';
                var jsdata = JSON.parse(data);
                var appenddata;
                appenddata += "<option value = ''>請選擇</option>";
                $.each(jsdata, function (key, value) {
                    appenddata += "<option value = '" + value.uid + " '>" + value.uname + " </option>";
                });
                $('#CheckerName').html(appenddata);
            },
            error: function (msg) {
                alert(msg);
            }
        });
        */
    });

    $("#AssetNo").change(function () {
        //
        $.ajax({
            contentType: "application/json; charset=utf-8",
            url: '../AppUsers/GetUsersByAssetNo',
            type: "GET",
            data: { id: $(this).val() },
            dataType: "json",
            success: function (data) {
                //var s = '[{"ListKey":"44","ListValue":"test1"},{"ListKey":"87","ListValue":"陳奕軒"}]';
                var jsdata = JSON.parse(data);
                var appenddata;
                appenddata += "<option value = ''>請選擇</option>";
                $.each(jsdata, function (key, value) {
                    appenddata += "<option value = '" + value.uid + "'>" + value.uname + " </option>";
                });
                $('#CheckerName').html(appenddata);
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
})