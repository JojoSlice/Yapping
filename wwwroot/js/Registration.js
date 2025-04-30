document.addEventListener("DOMContentLoaded", function () {

    const email = document.getElementById("email");

    email.addEventListener("change", function () {
        emailRegEx = /^[a-zA-Z0-9]+(?:\.[a-zA-Z0-9]+)*@[a-zA-Z0-9]+(?:\.[a-zA-Z0-9]+)*$/;

        if (emailRegEx.test(email.value)) {
            email.classList.remove("form-inputWrong");
            email.classList.add("form-inputPass");
        } else {
            email.classList.remove("form-inputPass");
            email.classList.add("form-inputWrong");
        }
    });


    const password = document.getElementById("password");
    const showHideButton = document.getElementById("togglePassword");
    const eyeIcon = document.getElementById("eyeIcon");

    showHideButton.addEventListener('click', function (event) {
        event.preventDefault();
        const type = password.type === 'password' ? 'text' : 'password';
        password.type = type;

        if (password.type === "password") {
            eyeIcon.src ="/img/eye-closed.svg";
        } else {
            eyeIcon.src ="/img/eye-open.svg";
        }
    });


    const rePassword = document.getElementById("repassword");

    rePassword.addEventListener("change", function () {
        if (rePassword.value === password.value) {
            rePassword.classList.remove("form-inputWrong");
            rePassword.classList.add("form-inputPass");
        } else {
            rePassword.classList.remove("form-inputPass");
            rePassword.classList.add("form-inputWrong");
        }
    });
})