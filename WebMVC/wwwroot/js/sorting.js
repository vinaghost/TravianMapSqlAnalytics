$(function () {
    $('.sort').on("click", function (event) {
        event.preventDefault();
        // get the url of the page link
        let sortfield = this.getAttribute('data-sortfield');
        // update a hidden field inside the search form with this value
        let elSortField = $('#SortField')
        let oldField = elSortField.val()

        if (sortfield.localeCompare(oldField) === 0) {
            let el = $('#SortOrder')
            if (+el.val() == 0) {
                el.val(1)
            }
            else {
                el.val(0)
            }
        }
        elSortField.val(sortfield);
        // trigger the search
        $('#Check').trigger("click");
    });
});