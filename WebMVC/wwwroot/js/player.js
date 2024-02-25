$(function () {
    let alliance = $('#player_alliance');
    alliance.select2({
        theme: "bootstrap-5",
        ajax: {
            dataType: 'json',
            type: 'Get',
            data: function (params) {
                var query = {
                    searchTerm: params.term,
                    page: params.page || 1,
                    pageSize: params.pageSize || 30
                }
                return query;
            },
            cache: true
        }
    });

    let player = $('#player_player');

    player.select2({
        theme: "bootstrap-5",
    })

    alliance.on('select2:select', function (e) {
        player.prop("disabled", true);

        const url = "/alliances/searchplayer?allianceId=:allaianceId:"
        $.getJSON(url.replace(':allaianceId:', $(this).val()), function (result) {
            let newOptions = '<option value="">-- Select --</option>';

            let items = result.results;
            
            for (var key in items) {
                var item = items[key];
                newOptions += '<option value="' + item['id'] + '">' + item['text'] + '</option>';
            }

            console.log(newOptions);

            player.val(null);

            player.html(newOptions);

            player.trigger('change');

            player.prop("disabled", false);
        });
    });
});
