document.addEventListener("DOMContentLoaded", function () {
    const changePicButton = this.getElementById("changeProfileImg");
    const uploadButton = this.getElementById("upload");
    const changeWindow = this.getElementById("changeWindow");

    changePicButton.addEventListener("click", function () {
        changeWindow.style.display = "block";
    })

    uploadButton.addEventListener("click", function () {
        changeWindow.style.display = "none";
    })
   
    window.addEventListener('click', function (event) {
        if (event.target === changeWindow) {
            changeWindow.style.display = "none";
        }
    });
});
