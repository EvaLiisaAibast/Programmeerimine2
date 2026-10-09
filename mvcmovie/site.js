(function () {
    var el = document.getElementById("np-clock");
    if (!el || window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
        return;
    }
    var parts = /^(\d+):(\d{2})$/.exec(el.textContent.trim());
    var secs = parts ? parseInt(parts[1], 10) * 60 + parseInt(parts[2], 10) : 0;
    window.setInterval(function () {
        secs += 1;
        var mm = Math.floor(secs / 60);
        var ss = secs % 60;
        el.textContent = (mm < 10 ? "0" + mm : String(mm)) + ":" + (ss < 10 ? "0" + ss : String(ss));
    }, 1000);
})();
