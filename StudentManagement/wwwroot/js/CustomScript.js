function confirmDelete(unqiueId, isDeleteClicked) {
    var deleteSpan = "deleteSpan_" + unqiueId;
    var confirmDeleteSpan = "confirmDeleteSpan_" + unqiueId;
    if (isDeleteClicked) {
        $("#" + deleteSpan).hide();
        $("#" + confirmDeleteSpan).show();
    } else {
        $("#" + deleteSpan).show();
        $("#" + confirmDeleteSpan).hide();
    }
}