$(function () {
    let el = $('.select2-tmsa');
    if (!el) {
        return;
    }
    el.select2({
        theme: "bootstrap-5",
        ajax: {
            dataType: 'json',
            type: 'Get',
            data: function (params) {
                var query = {
                    searchTerm: params.term,
                    pageNumber: params.page || 1,
                    pageSize: params.pageSize || 30
                }
                return query;
            },
            cache: true
        }
    });
});