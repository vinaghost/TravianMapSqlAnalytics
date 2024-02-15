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
        }
    });
});