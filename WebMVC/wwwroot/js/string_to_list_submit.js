$(function() {
    $('#Check').click(function(event) {
        var AllianceInput = $('#AllianceInput');
        if (AllianceInput){
            var alliances = $('#AllianceInput').val().toString().split(',');
            $('#Alliances').val(alliances);
        }

        var PlayerInput = $('#PlayerInput');
        if (PlayerInput){
            var players = $('#PlayerInput').val().toString().split(',');
            $('#Players').val(players);
        }
    });
});