$(function () {
    $(".ui .dropdown").dropdown();

    $("#downloadImageLink").off("click");
    $("#downloadImageLink").on("click", function () {
        var a = document.createElement("a");
        a.href = $("#stampImage").attr("src");
        a.download = "Stamp.png";
        a.click();
    });
});