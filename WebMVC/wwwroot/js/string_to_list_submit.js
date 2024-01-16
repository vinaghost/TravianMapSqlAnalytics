$(function () {
    $('#Check').on("click", function () {
        function getVal(element) {
            return element.val().toString().split(',');
        }

        var elementsId = [
            ["#AllianceInput", "#Alliances"],
            ["#PlayerInput", "#Players"],
            ["#VillageInput", "#Villages"],
        ]

        elementsId.forEach(ids => {
            var input = $(ids[0]);
            if (input.length != 1) return;
            var value = getVal(input);
            $(ids[1]).val(value);
        });
    });
});