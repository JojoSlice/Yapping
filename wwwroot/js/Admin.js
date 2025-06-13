
/*const url = "https://yappingapi-c6fkeubydcaycdgn.northeurope-01.azurewebsites.net/api/categories/"*/
const url = "https://localhost:7188/"

function markReport(reportId, btn) {
        fetch(`${url}api/report`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ id: reportId })
        })
        .then(response => {
            if (response.ok) {
                alert("Report marked as handled");
                // Du kan lägga till visuell feedback, t.ex. inaktivera knappen:
                btn.disabled = true;
                btn.innerText = "Handled";
                btn.classList.remove("btn-primary");
                btn.classList.add("btn-success");
            } else {
                return response.text().then(text => { throw new Error(text); });
            }
        })
        .catch(error => {
            console.error("Error marking report:", error);
            alert("Failed to mark report: " + error.message);
        });
}

function deleteCommentReport(raportId, commentId, btn) {
        if (!confirm("Are you sure you want to delete this report and comment?")) return;

        fetch(`${url}api/report/comment`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                commentId: commentId,
                raportId: raportId
            })
        })
        .then(response => {
            if (response.ok) {
                alert("Comment and report deleted successfully");
                const row = btn.closest('tr');
                if (row) row.remove();
            } else {
                return response.text().then(text => { throw new Error(text); });
            }
        })
        .catch(error => {
            console.error("Error deleting:", error);
            alert("Failed to delete comment/report: " + error.message);
        });
    }


function deletePostReport(raportId, postId, btn) {
        if (!confirm("Are you sure you want to delete this report and post?")) return;

        fetch(`${url}api/report/post`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                postId: postId,
                raportId: raportId
            })
        })
        .then(response => {
            if (response.ok) {
                alert("Post and report deleted successfully");
                const row = btn.closest('tr');
                if (row) row.remove();
            } else {
                return response.text().then(text => { throw new Error(text); });
            }
        })
        .catch(error => {
            console.error("Error deleting:", error);
            alert("Failed to delete post/report: " + error.message);
        });
    }