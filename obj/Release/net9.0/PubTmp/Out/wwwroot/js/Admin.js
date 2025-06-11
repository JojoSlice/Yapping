async function deleteReport (id, button) {
    const response = await fetch(`Admin?handler=Delete`, {
        method: 'POST',
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ Id = id })
    });
    if (!response.ok) {
        console.log(response);
    }
}
async function markReport(id, button) {
    const response = await fetch(`Admin?handler=MarkAsHandled`, {
        method: 'POST',
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ Id = id })
    });
    if (!response.ok) {
        console.log(response);
    }
}