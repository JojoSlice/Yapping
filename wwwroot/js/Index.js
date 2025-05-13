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

  window.addEventListener("scroll", function () {
    if (window.innerHeight + window.scrollY >= document.body.offsetHeight) {
      const lastCreatedAt = lastCreatedAt();
      loadMorePosts(lastCreatedAt);
    }
  });

  function loadMorePosts(lastCreatedAt) {
    fetch(`/Index?handler=GetMorePosts&lastCreatedAt=${lastCreatedAt}`)
      .then((response) => response.json())
      .then((posts) => {
        posts.forEach((post) => {
          fetchUserAndCategory(post);
        });
      })
      .catch((error) => console.log("Error loading posts:", error));
  }

  async function fetchUserAndCategory(post) {
    try {
      const userResponse = await fetch(
        `/Index?handler=GetPostUser&id=${post.userid}`,
      );
      const user = await userResponse.json();

      const categoryResponse = await fetch(
        `/Index?handler=GetCategory&id=${post.categoryid}`,
      );
      const category = await categoryResponse.json();

      const postElement = createPostElement(post, user, category);
      document.getElementById("postContainer").appendChild(postElement);
    } catch (error) {
      console.error("Error fetching user or category:", error);
    }
  }

  function createPostElement(post, user, category) {
    const postElement = document.createElement("div");
    postElement.classList.add("row");
    postElement.innerHTML = `
            <div class="col-2 m-1 card">
                <img class="img-thumbnail m-1" src="${user.imgpath}" />
                <h6 class="card-title">${user.username}</h6>
                <button class="btn btn-secondary m-1 p-1 text-wrap">View user</button>
                <button class="btn btn-secondary m-1 p-1 text-break">Send message</button>
            </div>
            <div class="col-9 m-1 card">
                <div class="card-header m-1">
                    <label class="card-title">${category.name}/${post.title}</label>
                </div>
                <div class="card-body">
                    <p class="card-text">${post.text}</p>
                </div>
                <div class="card-footer">
                    <button class="btn btn-secondary m-1">Comment <span class="badge">0</span></button>
                    <button class="btn btn-info m-1">Like <span class="badge">0</span></button>
                </div>
            </div>
        `;
    return postElement;
  }

  document.getElementById("submit").addEventListener("click", function () {
    const newPostData = new FormData(document.querySelector("form"));
    fetch("/Index?handler=Post", {
      method: "POST",
      body: newPostData,
    })
      .then((response) => response.json())
      .then((data) => {
        if (data.success) {
          const newPost = data.post;
          const newPostElement = createPostElement(newPost);
          postContainer.prepend(newPostElement);
        } else {
          console.error("Failed to post:", data.message);
        }
      });
  });
});
