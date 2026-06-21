document.addEventListener("click", async function (e) {
    const link = e.target.closest("a[data-spa]");
    if (!link) return;

    e.preventDefault();
    const url = link.getAttribute("href");

    try {
        const res = await fetch(url, {
            headers: { "X-Requested-With": "XMLHttpRequest" }
        });
        const html = await res.text();

        document.getElementById("app-content").innerHTML = html;
        history.pushState({}, "", url);

        // re-run AOS, counters, etc. since new DOM was injected
        if (window.AOS) AOS.refresh();
    } catch (err) {
        window.location.href = url; // fallback to full nav on failure
    }
});

window.addEventListener("popstate", () => {
    location.reload(); // simplest way to handle back/forward correctly
});