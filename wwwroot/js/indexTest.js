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

        const likeResponse = await fetch(`/Index?handler=LikePost&postid=${postid}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ postId: postid })
        });

        if (!likeResponse.ok) {
            throw new Error("Failed to like post");
        }

        const getLikesResponse = await fetch(`/Index?handler=GetLikesOnPost&postid=${postid}`);

        if (!getLikesResponse.ok) {
            throw new Error("Failed to get updated likes");
        }

        const likes = await getLikesResponse.text();

        const badge = button.querySelector('.badge');
        badge.textContent = likes;
    } catch (err) {
        console.error(err);
    }
}

