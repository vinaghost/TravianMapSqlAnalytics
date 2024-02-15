$(function () {
    let el = $('.select2-tmsa');
    el.select2({
        theme: "bootstrap-5",
        ajax: {
            data: function (params) {
                var query = {
                    searchTerm: params.term,
                }
                return query;
            },
            processResults: function (data) {
                return {
                    results: data.items
                }
            },

            cache: true
        }
    });
});