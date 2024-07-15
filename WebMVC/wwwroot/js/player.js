$(function () {
    let alliance = $('#player_alliance');
    if (alliance) {
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
    }

    let server = $('#server-select');
    if (server) {
        server.on('select2:select', function (e) {
            alliance.val(null).trigger('change');
        });
    }

    let player = $('#player_player');

    if (player) {
        player.select2({
            theme: "bootstrap-5",
        })

        const newOptions = '<option value="">-- Select --</option>';

        function loadPlayer() {
            player.prop("disabled", true);

            const url = "/players/searchAlliance?allianceId=:allaianceId:"
            var val = alliance.val() | "-1";
            $.getJSON(url.replace(':allaianceId:', val), function (result) {
                var options = newOptions;
                for (var key in result.results) {
                    var item = result.results[key];
                    options += '<option value="' + item['id'] + '">' + item['text'] + '</option>';
                }

                player.val(null);
                player.html(options);
                player.trigger('change');
                player.prop("disabled", false);
            });
        }

        alliance.on('select2:select', function (e) {
            loadPlayer();
        });

        loadPlayer();
    }
});