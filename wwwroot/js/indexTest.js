document.addEventListener("DOMContentLoaded", function () {
  const postBtn = document.getElementById("newPost");
  const newPostWindow = document.getElementById("postModal");

  postBtn.addEventListener("click", function () {
    newPostWindow.style.display = "block";
  });
  window.addEventListener("click", function (event) {
    if (event.target === newPostWindow) {
      newPostWindow.style.display = "none";
    }
  });

  const postContainer = document.getElementById("postContainer");

  //window.addEventListener("scroll", function () {
  //  if (window.innerHeight + window.scrollY >= document.body.offsetHeight) {
  //      lastCreatedAt().then(createdAt => {
  //          if (createdAt) {
  //              loadMorePosts(createdAt);
  //          } else {
  //              console.log("No post found");
  //          }
  //      })
  //  }
  //});

});

async function likePost(postid, button) {
    try {
        console.log("likePost körs");


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

            console.log(likes);

            const badge = button.querySelector('.badge');
            badge.textContent = likes;
        } else throw new Error("No authToken");

    } catch (err) {
        console.error(err);
    }
}

