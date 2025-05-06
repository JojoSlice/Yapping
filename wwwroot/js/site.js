// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", function () {
    const authButton = document.getElementById("authButton");
    const signinmodal = document.getElementById("signInModal");
    const closeBtn = document.querySelector(".close");
    const wlcText = document.getElementById("welcomeText");
    const profileImg = document.getElementById("profileimg");
    const profileLink = document.getElementById("profileLink");

    fetch("/api/isauthenticated/get", {
    credentials: "include" 
    })
        .then(response => response.json())
        .then(data => {
        if (data.authenticated) {
            authButton.value = "Sign out";
            authButton.addEventListener("click", signOut);
            welcome();
            profileLink.style.display = "block";
            profileImg.style.display = "block";
                    } else {
            authButton.value = "Sign in";
            authButton.addEventListener("click", signIn);
            profileLink.style.display = "none";
            profileImg.style.display = "none";
            wlcText.style.display = "none";
            wlcText.innerText = "";
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

    function welcome() {
        fetch("api/userApi/user", {
            credentials: "include"
        })
            .then(respons => {
                if (!respons.ok) {
                    throw new Error("Not authenticated");
                } return respons.json();
            })
            .then(data => {
                console.log("data loggas");
                console.log(data);
                console.log(data.id);

                const username = data.username;
                const welcomeMessage = `Welcome ${username}`;
                console.log(welcomeMessage);
                wlcText.style.display = "block";
                wlcText.innerText = welcomeMessage;
                let path = data.profileimg.replace("~", "");
                profileImg.src = path;
                console.log(path);
            })
            .catch(error => {
                console.error("Failed to fetch user: ", error);
            });
    }

    function signIn() {
        signinmodal.style.display = "block";
    };

    function signOut() {
        console.log("logout");
        fetch('/LogOut')
            .then(respons => {
                if (respons.redirected) {
                    window.location.href = respons.url;
                }
            });
    };
});


