// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", function () {
    const authbutton = document.getElementById("authButton");
    const signinmodal = document.getElementById("signInModal");
    const closeBtn = document.querySelector(".close");

    fetch("/api/isauthenticated/get", {
    credentials: "include" 
    })
        .then(response => response.json())
        .then(data => {
        if (data.authenticated) {
            authButton.innerText = "Sign out";
            authButton.addEventListener("click", signOut);
        } else {
            authButton.innerText = "Sign in";
            authButton.addEventListener("click", signIn);
        }
    })
    .catch(error => {
        console.error("Error checking authentication:", error);
    });
        
    closeBtn.addEventListener('click', function() {
        signinmodal.style.display = "none";
    });

    window.addEventListener('click', function(event) {
        if (event.target === signinmodal) {
            signinmodal.style.display = "none";
        }
    });
    function signIn() {
        signinmodal.style.display = "block";
    };

    function signOut() {
        fetch('/LogOut')
            .then(respons => {
                if (respons.redirected) {
                    window.location.href = respons.url;
                }
            });
    };
});


