function showmsg(data) {
    if (!data.success) {
        alert(data.error);
    }
    else {
        alert("儲存成功!");
        location.replace('./Index');
    }
}

$(function () {
    $("#NewFileType").prop('readonly', true);
    $("#FileType").change(function () {
        var s = $(this).val();
        if (s == "新增") {
            $("#NewFileType").prop('readonly', false);
        }
        else {
            $("#NewFileType").val('');
            $("#NewFileType").prop('readonly', true);
        }
    });
    $("#FileLink").fileinput({
        showUpload: false,
        showPreview: false,
        uploadUrl: '../Manuals/Upload', //处理上传文件的url
        uploadAsync: false,
        language: "zh-TW",
        maxFileSize: 0,
        ajaxSettings: {            //上传成功后,接收后台的json值            
            success: function (data) {
                if (!data.success) {
                    $("#FileLink").fileinput("reset");
                    alert(data.error);
                }
                else {
                    alert('新增成功');
                    location.replace('./Index');
                }
            }
        },
        uploadExtraData: function () {

            return {
                ManualName: $("#ManualName").val(),
                AssetBrand: $("#AssetBrand").val(),
                ManualClass: $("#ManualClass").val(),
                AssetType: $("#AssetType").val(),
                FileType: $("#FileType").val(),
                NewFileType: $("#NewFileType").val(),
                FileLanguage: $("#FileLanguage").val(),
                FileName: $("#FileName").val(),
                FileGuid: $("#FileGuid").val(),
                FileAuth: $("#FileAuth").val(),
                Remark: $("#Remark").val()
            };
        }
    });

    $("#btnSEND").click(function () {

        $("#FileLink").fileinput("upload");
    });

})