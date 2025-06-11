document.addEventListener("DOMContentLoaded", function () {
  const commentBtn = document.getElementById("commentBtn");
  const newPostWindow = document.getElementById("postModal");
const path = window.location.pathname;
    console.log(path);

  commentBtn.addEventListener("click", function () {
    newPostWindow.style.display = "block";
  });
  window.addEventListener("click", function (event) {
    if (event.target === newPostWindow) {
      newPostWindow.style.display = "none";
    }
  });

});

async function commentPost(commentid, button) {
    const newCommentWindow = document.getElementById("commentModal");
    newCommentWindow.style.display = "block";

    document.getElementById("commentid").value = commentid;

    window.addEventListener("click", function (event) {
        if (event.target === newCommentWindow) {
            newCommentWindow.style.display = "none";
        }
    });

    document.getElementById("commentForm").addEventListener("submit", function (event) {
        newCommentWindow.style.display = "none";
    });
}


async function likePost(postid, button) {
    try {
        var response = await fetch("/antiforgery/token", {
            method: "GET"
        });

        if (response.ok) {
            const xsrfToken = document.cookie
                .split("; ")
                .find(row => row.startsWith("XSRF-TOKEN="))
                .split("=")[1];

            const likeResponse = await fetch(`/Index?handler=LikePost`, {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                    'Content-Type': 'application/json',
                    "X-XSRF-TOKEN": xsrfToken,
                },
                body: JSON.stringify({ postId: postid })
            });

            if (!likeResponse.ok) {
                throw new Error("Failed to like post");
            }

            const getLikesResponse = await fetch(`/Index?handler=LikesOnPost&postid=${postid}`);

            if (!getLikesResponse.ok) {
                throw new Error("Failed to get updated likes");
            }

            const likes = await getLikesResponse.text();

            const badge = button.querySelector('.badge');
            badge.textContent = likes;
        } else throw new Error("No authToken");

    } catch (err) {
        console.error(err);
    }
}
async function reply(receiveId, button) {
    const window = document.getElementById("replyModal");
    window.style.display = "block";

    document.getElementById("ReplyId").value = receiveId;

    window.addEventListener("click", function (event) {
        if (event.target === window) {
            window.style.display = "none";
        }
    });

};

async function report(postId) {
    try {
        const response = await fetch("https://yappingapi-c6fkeubydcaycdgn.northeurope-01.azurewebsites.net/api/report", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                postid: postId
            })
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error("Fel från API: " + errorText);
        }

        console.log("Rapport skapad!");
    } catch (error) {
        console.error("Fel vid rapportering:", error);
    }
}


async function reportComment(commentId) {
    try {
        const response = await fetch("https://yappingapi-c6fkeubydcaycdgn.northeurope-01.azurewebsites.net/api/report", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                commentid: commentId
            })
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error("Fel från API: " + errorText);
        }

        console.log("Rapport skapad!");
    } catch (error) {
        console.error("Fel vid rapportering:", error);
    }
}


