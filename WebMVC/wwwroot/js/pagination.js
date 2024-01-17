$(function () {
    $('#pagination a').on("click", function (event) {
        event.preventDefault();
        // get the url of the page link
        var page = this.getAttribute('data-num');
        // update a hidden field inside the search form with this value
        $('#PageNumber').val(parseInt(page));
        // trigger the search
        $('#Check').trigger("click");
    });
});