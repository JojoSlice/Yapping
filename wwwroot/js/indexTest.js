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
