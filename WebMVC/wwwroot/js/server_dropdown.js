$(function () {
    let el = $('#server-select');

    el.prop('name', '');
    el.on('change', async function () {
        const rawResponse = await fetch(`/servers/change?server=${this.value}`,
            {
                method: "Get",
            });
        if (rawResponse.ok) {
            $("#currentServer").text(this.value);
            Toastify({
                text: "Your selected server is saved",
                duration: 3000,
                gravity: "top", // `top` or `bottom`
                position: "right", // `left`, `center` or `right`
                stopOnFocus: true, // Prevents dismissing of toast on hover
                //style: {
                //    background: "linear-gradient(to right, #00b09b, #96c93d)",
                //},
                onClick: function () { } // Callback after click
            }).showToast();
        }
    });
});