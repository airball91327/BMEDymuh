$(function () {

    $("#FileLink").fileinput({
        showUpload: false,
        showPreview: false,
        uploadUrl: '../AttainFiles/UpdAssetExcel', //处理上传文件的url
        uploadAsync: false,
        language: "zh-TW",
        ajaxSettings: {            //上传成功后,接收后台的json值            
            success: function (data) {
                if (!data.success)
                    alert(data.error);
                else
                    alert('上傳成功');
            }
        }
    });

    $("#btnUPLOAD").click(function () {

        $("#FileLink").fileinput("upload");
    });

})