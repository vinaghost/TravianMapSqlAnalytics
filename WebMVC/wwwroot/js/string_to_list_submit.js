$(function () {
    $('#Check').on("click", function () {
        function getVal(element) {
            return element.val().toString().split(',');
        }

        $('.list-parameter').each(function () {
            let input = $(this).find('.input');
            if (input.length !== 1) return;

            let name = input.prop('name');
            input.prop('name', '');
            let value = getVal(input);

            if (value[0] === '') return;


            let text = "";
            for (let i = 0; i < value.length; i++) {
                text += `<input type="hidden" name="${name}" value="${value[i]}" />`;
            }
            const newDiv = document.createElement('div');
            newDiv.innerHTML = text;
            this.append(newDiv);
        });
    });
});