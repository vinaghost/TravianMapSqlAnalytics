$(function () {
    $('#Check').click(function (event) {
        var AllianceInput = $('#AllianceInput');
        if (AllianceInput) {
            var alliances = $('#AllianceInput').val().toString().split(',');
            $('#Alliances').val(alliances);
        }

        var PlayerInput = $('#PlayerInput');
        if (PlayerInput) {
            var players = $('#PlayerInput').val().toString().split(',');
            $('#Players').val(players);
        }
        var VillageInput = $('#VillageInput');
        if (VillageInput) {
            var villages = $('#VillageInput').val().toString().split(',');
            $('#Villages').val(villages);
        }
    });
});