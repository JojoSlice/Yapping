document.addEventListener("DOMContentLoaded", function () {
    const changePicButton = this.getElementById("changeProfileImg");
    const changeWindow = this.getElementById("changeimg");

    changePicButton.addEventListener("click", function () {
        changeWindow.style.display = "block";
    })

    window.addEventListener('click', function (event) {
        if (event.target === changeWindow) {
            changeWindow.style.display = "none";
        }
    });
});
