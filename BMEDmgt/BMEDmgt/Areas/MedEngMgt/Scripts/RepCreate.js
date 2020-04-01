function showmsg(data) {
    if (data.success !== null) {
        if (!data.success)
            alert(data.error);
        else {
            alert("申請單送出成功!! [關閉後頁面回到首頁]");
            window.open('./Print?id=' + $("#DocId").val());
            location.replace("../Home/Index");
        }

    }
}

$(function () {

    $("#ChkAssetNo").click(function () {
        if ($(this).prop("checked")) {
            $("#pnlASSET").hide();
            $("#pnlAssetName").show();
            $("#assetNameList").trigger('change');

            var plantClass = $("#PlantClass:checked").val();
            var keynam = '000';
            if (plantClass !== "醫療儀器") {
                keynam = '001';
            }
            var accDptId = $('#AccDpt').val();
            if (keynam !== "") {
                $.ajax({
                    contentType: "application/json; charset=utf-8",
                    url: '../Assets/GetAssetsByKeynameAndAcc',
                    type: "GET",
                    data: { keyname: keynam, accDpt: accDptId },
                    dataType: "json",
                    async: false,
                    success: function (data) {
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
            if (keynam == '000') {
                $("#AssetNo").find('option[value="000"]').attr('selected', 'selected');
            }
            else {
                $("#AssetNo").find('option[value="001"]').attr('selected', 'selected');
            }
            //$("#TroubleDes").val("設備名稱: \n\n故障描述: ");
        }
        else {
            $("#pnlASSET").show();
            $("#pnlAssetName").hide();
            $("#AssetName").val("");
            $("#AssetNo").find('option[value=""]').attr('selected', 'selected');
            //$("#TroubleDes").val('');
        }
    });

    $("#btnQtyChecker").click(function () {
        var keynam = $("#CheckerKeyName").val();
        if (keynam === "") {
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
        if (keynam !== "") {
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
        var dp = $("#AccDpt option:selected").val();
        $.ajax({
            contentType: "application/json; charset=utf-8",
            url: '../Assets/GetAssetsByAccDpt',
            type: "GET",
            data: { dpt: dp },
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
        var assetno = $(this).val();
        $.ajax({
            contentType: "application/json; charset=utf-8",
            url: '../AppUsers/GetUsersByAssetNo',
            type: "GET",
            data: { id: assetno },
            dataType: "json",
            success: function (data) {
                //var s = '[{"ListKey":"44","ListValue":"test1"},{"ListKey":"87","ListValue":"陳奕軒"}]';
                var jsdata = JSON.parse(data);
                var appenddata;
                appenddata += "<option value = ''>請選擇</option>";
                $.each(jsdata, function (key, value) {
                    appenddata += "<option value = '" + value.uid + "'>" + value.uname + " </option>";
                });
                //$('#CheckerName').html(appenddata);
                //
                $.ajax({
                    contentType: "application/json; charset=utf-8",
                    url: '../Assets/GetAccDptByAsset',
                    type: "GET",
                    data: { ano: assetno },
                    dataType: "json",
                    success: function (data) {
                        //var s = '[{"ListKey":"44","ListValue":"test1"},{"ListKey":"87","ListValue":"陳奕軒"}]';
                        var jsdata = JSON.parse(data);
                        var appenddata = "";
                        //appenddata += "<option value = ''>請選擇</option>";
                        $.each(jsdata, function (key, value) {
                            appenddata += "<option value = '" + value.dptid + "'>" + value.dptname + " </option>";
                        });
                        $('#AccDpt').html(appenddata);
                    },
                    error: function (msg) {
                        alert(msg);
                    }
                });
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
            data: { id: docid, typ: "1" },
            success: function (data) {
                $("#pnlFILES").html(data);
            }
        });
    });

    $("#AccDpt").trigger('change');

    $("#troubleDesList").change(function () {
        var originText = $("#TroubleDes").val();
        $("#TroubleDes").val($(this).val() + originText);
    });

    $("#assetNameList").change(function () {
        var selectedValue = $(this).val();
        if (selectedValue === '000') {
            $("#otherAssetName").show();
            $("#otherAssetName").attr("required", "required");
        }
        else {
            $("#otherAssetName").hide();
            $("#otherAssetName").val("");
            $("#otherAssetName").removeAttr("required");
            $("#AssetName").val(selectedValue);
        }
    });

    $("#otherAssetName").change(function () {
        $("#AssetName").val($(this).val());
    });

    $('input:radio[name="RepType"]').click(function () {
        var repType = $(this).val();
        if (repType === "送修") {
            $('#PlaceLoc').removeAttr('data-val-required');
        } else {
            $('#PlaceLoc').attr('data-val-required', '放置地點 欄位是必要項。');
        }
        // 讓validator重新parse需要的欄位
        $('form').removeData('validator');
        $('form').removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse('form');
    });

});