function showmsg(data) {
    if (!data.success) {
        alert(data.error);
    }
    else {
        alert("儲存成功!");
        location.replace('../Index');
    }
}

$(function () {
    $(".datefield").datepicker({
        format: "yyyy/mm/dd"
    });
    $('.combobox').combobox();
   

})