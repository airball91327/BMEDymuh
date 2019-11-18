function showmsg(data) {
    if (!data.success) {
        alert(data.error);
    }
    else {
        alert("儲存成功!");
    }
}