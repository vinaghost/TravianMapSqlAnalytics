$(function () {
    $('#servers button').on("click", async function (event) {
        const rawResponse = await fetch("servers/change?server=" + this.id,
            {
                method: "Get",
            });
        if (rawResponse.ok) {
            document.cookie = await rawResponse.text;
            $("#currentServer").text(this.id);
            alert("Changed your current server to " + this.id)
        }
    })
})