$(function() {
    $('#container a').click(function(event) {
        event.preventDefault();
        // get the url of the page link
        var url = this.href;
        var pagination = url.split('/').slice(-1)[0]
        var str = pagination.split('_');
        // update a hidden field inside the search form with this value
        $('#PageNumber').val(parseInt(str[0]));
        $('#PageSize').val(parseInt(str[1]));
        // trigger the search
        $('#Check').click();
    });
});