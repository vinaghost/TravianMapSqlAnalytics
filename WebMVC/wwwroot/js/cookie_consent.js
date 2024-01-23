$(function () {
    $("#cookieConsent button[data-cookie-string]").on("click", function () {
        document.cookie = $("#cookieConsent button").attr("data-cookie-string");
        $("#cookieConsent").hide();
    });
});    