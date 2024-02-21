const xKey = "tmsa_x"
const yKey = "tmsa_y"
const xField = document.getElementById("X")
const yField = document.getElementById("Y")
const isUserInputField = document.getElementById("IsUserInput")

function saveCoords() {
    let x = xField.value
    let y = yField.value
    localStorage.setItem(xKey, x);
    localStorage.setItem(yKey, y);
}

function loadCoords() {
    if (localStorage.getItem(xKey) && localStorage.getItem(yKey)) {
        xField.value = localStorage.getItem(xKey)
        yField.value = localStorage.getItem(yKey)
    }

    isUserInputField.value = "true";
}

$("#Check").on("click", saveCoords);

loadCoords()