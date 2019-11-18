$(function () {
    $("#btnASSETKEEP").click(function () {
        $("#imgLOADING2").show();

        var s = $("#fmASSETKEEP").serialize();
        $.ajax({
            url: "../AssetKeeps/Edit",
            type: "POST",
            data: s,
            async: true,
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert('儲存成功!!');
                }
                else {
                    alert(data.error);
                }

            }
        });
        $("#imgLOADING2").hide();
    });
})