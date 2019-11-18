$(function () {
    $("#DocType").prop('readonly', true);
    $("#DocId").prop('readonly', true);

    $("#FileLink").fileinput({
        showUpload: false,
        showPreview: false,
        uploadUrl: '../../AttainFiles/Upload2', //处理上传文件的url
        uploadAsync: false,
        language: "zh-TW",
        ajaxSettings: {            //上传成功后,接收后台的json值            
            success: function (data) {
                if (!data.success)
                    alert(data.error);
                else
                    alert('上傳成功');
            }
        },
        uploadExtraData: function () {

            return {
                DocType: $("#DocType").val(),
                Title: $("#Title").val(),
                DocId: $("#DocId").val(),
                SeqNo: $("#SeqNo").val(),
                IsPublic: $("#IsPublic").val()
            };
        }
    });

    $("#btnUPLOAD").click(function () {

        $("#FileLink").fileinput("upload");
    });

})