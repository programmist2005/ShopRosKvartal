//превью загружаемого изображения (фото)
function readImgURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#prePhoto').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    }
}

$("#PhotoFile").change(function () {
    $("#previewPhoto").removeAttr("hidden");
    readImgURL(this);
});

$("#uploadPhotoCancel").click(function () {
    $("#PhotoFile").val('');
    $("#previewPhoto").attr("hidden", "hidden");
});