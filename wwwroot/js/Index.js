document.addEventListener("DOMContentLoaded", function () {
    let isLoading = false;


    const postBtn = document.getElementById("newPost");
    const newPostWindow = document.getElementById("postModal");

    postBtn.addEventListener("click", function () {
        newPostWindow.style.display = "block";
    });
     window.addEventListener('click', function (event) {
        if (event.target === changeWindow) {
            newPostWindow.style.display = "none";
        }
    });


    async function loadMorePosts() {
        if (isLoading) return;
        isLoading = true;

        const lastPost = document.querySelector(".post:last-of-type");
        const lastCreatedAt = lastPost?.getAttribute("data-createdat");

        if (!lastCreatedAt) return;

        const response = await fetch(`/Index?handler=MorePosts&lastCreatedAt=${lastCreatedAt}`);
        const newPosts = await response.json();

        const container = document.getElementById("postContainer");

        for (const post of newPosts) {
            const response = await fetch(`/Index?handler=Category&id=${post.id}`);
            const postUserresponse = await fetch(`/Index?handler=PostUser&id=${post.userid}`)
            const category = await response.json();
            const user = await postUserresponse.json();

            const row = document.createElement("div");
            row.classList.add("row");
            row.setAttribute("data-createdat", post.createdAt);
            container.appendChild(row);

            const profilecard = document.createElement("div");
            profilecard.classList.add("col-2 m-1 card");
            row.appendChild(profilecard)

            const img = document.createElement("img");
            img.classList.add("img-thumbnail m-1");
            img.src = user.profileimg;
            profilecard.appendChild(img);

            const nameTitle = document.createElement("h6");
            nameTitle.classList.add("card-title");
            nameTitle.textContent = user.username;
            profilecard.appendChild(nameTitle);

            const viewUser = document.createElement("button");
            viewUser.classList.add("btn btn-secondary m-1 p-1");
            viewUser.textContent = "View profie";
            profilecard.appendChild(viewUser);

            const messUser = document.createElement("button");
            messUser.classList.add("btn btn-secondary m-1 p-1");
            messUser.textContent = "Message";
            profilecard.appendChild(messUser);

            const postCard = document.createElement("div");
            postCard.classList.add("col-9 m-1 card");
            row.appendChild(postCard);

            const header = document.createElement("div");
            header.classList.add("card-header m-1");
            const title = document.createElement("label");
            title.classList.add("card-title");
            title.textContent = post.title;
            header.appendChild(title);
            postCard.appendChild(header);

            const body = document.createElement("div");
            body.classList.add("card-body");
            const img = document.createElement("img");
            img.classList.add("img-thumbnail m-1");
            img.src = post.imgpath;

            if (post.imgpath != "") {
                body.appendChild(img);
            }

            const text = document.createElement("p");
            text.classList.add("card-text");
            text.textContent = post.content;
            body.appendChild(text);
            postCard.appendChild(body);

            const footer = document.createElement("div");
            footer.classList.add("card-footer");
            const commentbtn = document.createElement("button");
            commentbtn.classList.add("btn btn-secondary m-1");
            commentbtn.textContent = "Comment";
            commentbtn.innerHTML = <span class="badge">0</span>;
            const likebtn = document.createElement("button");
            likebtn.classList.add("btn btn-secondary m-1");
            liketbtn.textContent = "Like";
            likebtn.innerHTML = <span class="badge">0</span>;
            footer.appendChild(commentbtn);
            footer.appendChild(likebtn);
            postCard.appendChild(footer);
        };

        isLoading = false;
    }

    window.addEventListener("scroll", () => {
        if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 200) {
            loadMorePosts();
        }
    });
});
