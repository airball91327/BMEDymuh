$(function () {
    //$("#DelivDpt").attr('disabled', 'disabled');
    //$("#DelivEmp").attr('disabled', 'disabled');
    //$("#DelivUid").attr('disabled', 'disabled');

    $("#DelivDpt").change(function () {
        var s = $(this).val();
        $.ajax({
            contentType: "application/json; charset=utf-8",
            url: '../../AppUsers/GetUsersInDpt',
            type: "GET",
            data: "id=" + s,
            dataType: "json",
            success: function (data) {
                //var s = '[{"ListKey":"44","ListValue":"test1"},{"ListKey":"87","ListValue":"陳奕軒"}]';
                var jsdata = JSON.parse(data);
                var appenddata;
                appenddata += "<option value = ''>請選擇</option>";
                $.each(jsdata, function (key, value) {
                    appenddata += "<option value = '" + value.uid + " '>" + value.uname + " </option>";
                });
                $('#DelivUid').html(appenddata);
            },
            error: function (msg) {
                alert(msg);
            }
        });
    });
});