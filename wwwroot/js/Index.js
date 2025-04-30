document.addEventListener("DOMContentLoaded", function () {
    let isLoading = false;

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
            const category = await response.json();

            const div = document.createElement("div");
            div.classList.add("post");
            div.setAttribute("data-createdat", post.createdAt);
            div.innerHTML = `<h3>${post.title}</h3><h6>${category.name}</h6><p>${post.content}</p>`;
            container.appendChild(div);
        };

        isLoading = false;
    }

    window.addEventListener("scroll", () => {
        if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 200) {
            loadMorePosts();
        }
    });
});
