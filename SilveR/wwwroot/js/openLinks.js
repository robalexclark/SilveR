
function openItem(url) {
    $.ajax({
        type: "POST",
        url: "/Values/OpenUrl",
        data: { url: url }
    });
}