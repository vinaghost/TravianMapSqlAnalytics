$(function () {
    let el = $('#server-select');
    el.select2({
        theme: "bootstrap-5",
        ajax: {
            url: '/servers/ServerList',
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

    el.prop('name', '');
    el.on('change', async function () {
        const rawResponse = await fetch(`/servers/change?server=${this.value}`,
            {
                method: "Get",
            });
        if (rawResponse.ok) {
            $("#currentServer").text(this.value);
        }
    });
});