window.sidebar = (function () {
    let backdrop = null;

    function ensureBackdrop() {
        if (!backdrop) {
            backdrop = document.createElement('div');
            backdrop.className = 'sidebar-backdrop';
            backdrop.addEventListener('click', () => hide());
            document.body.appendChild(backdrop);
        }
    }

    function show() {
        const sidebar = document.querySelector('.sidebar');
        if (!sidebar) return;
        sidebar.classList.add('show');
        ensureBackdrop();
        // show backdrop
        backdrop.style.display = 'block';
        // prevent body scroll
        document.body.style.overflow = 'hidden';
    }

    function hide() {
        const sidebar = document.querySelector('.sidebar');
        if (!sidebar) return;
        sidebar.classList.remove('show');
        if (backdrop) backdrop.style.display = 'none';
        document.body.style.overflow = '';
    }

    function toggle() {
        const sidebar = document.querySelector('.sidebar');
        if (!sidebar) return;
        if (sidebar.classList.contains('show')) hide(); else show();
    }

    return { show, hide, toggle };
})();
