document.addEventListener("DOMContentLoaded", function () {
  const messageBtn = document.getElementById("newMessage");
  const newMessageWindow = document.getElementById("MessageModal");

    messageBtn.addEventListener("click", function () {
    newMessageWindow.style.display = "block";
  });
  window.addEventListener("click", function (event) {
    if (event.target === newMessageWindow) {
      newMessageWindow.style.display = "none";
    }
  });

});

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