document.addEventListener("DOMContentLoaded", function () {
  const chatBtn = document.getElementById("newChat");
  const newWindow = document.getElementById("chatModal");

  chatBtn.addEventListener("click", function () {
    newWindow.style.display = "block";
  });
  window.addEventListener("click", function (event) {
    if (event.target === newWindow) {
      newWindow.style.display = "none";
    }
  });

  const addBtn = document.getElementById("addUser");
  const addWindow = document.getElementById("addModal");

  addBtn.addEventListener("click", function () {
    addWindow.style.display = "block";
  });
  window.addEventListener("click", function (event) {
    if (event.target === addWindow) {
      addWindow.style.display = "none";
    }
  });

  const removeBtn = document.getElementById("removeUser");
  const removeWindow = document.getElementById("removeModal");

  removeBtn.addEventListener("click", function () {
    removeWindow.style.display = "block";
  });
  window.addEventListener("click", function (event) {
    if (event.target === removeWindow) {
      removeWindow.style.display = "none";
    }
  });


});