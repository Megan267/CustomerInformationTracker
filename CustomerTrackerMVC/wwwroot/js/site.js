(function () {
    'use strict';

    var sidebar = document.getElementById('sidebar');
    var backdrop = document.querySelector('.sidebar-backdrop');

    function setSidebar(open) {
        if (!sidebar) {
            return;
        }
        sidebar.classList.toggle('sidebar-open', open);
        if (backdrop) {
            backdrop.hidden = !open;
        }
    }

    document.querySelectorAll('[data-sidebar-open]').forEach(function (btn) {
        btn.addEventListener('click', function () { setSidebar(true); });
    });
    document.querySelectorAll('[data-sidebar-close]').forEach(function (el) {
        el.addEventListener('click', function () { setSidebar(false); });
    });

    document.querySelectorAll('[data-dismissible] .notice-dismiss').forEach(function (btn) {
        btn.addEventListener('click', function () {
            btn.closest('[data-dismissible]').remove();
        });
    });

    var filterForm = document.querySelector('form[data-live-filter]');
    if (filterForm) {
        var typingDelay = 300;
        var timer = null;
        var inFlight = null;
        var lastQuery = currentQuery();
        var clearLink = filterForm.querySelector('[data-filter-clear]');
        var status = filterForm.querySelector('[data-filter-status]');
        var countPill = document.querySelector('[data-count-pill]');

        function currentQuery() {
            var params = new URLSearchParams();
            new FormData(filterForm).forEach(function (value, key) {
                if (String(value).trim() !== '') {
                    params.append(key, String(value).trim());
                }
            });
            return params.toString();
        }

        function hasFilter() {
            return Array.prototype.some.call(filterForm.querySelectorAll('.search-input'), function (input) {
                return input.value.trim() !== '';
            });
        }

        function refresh() {
            var query = currentQuery();
            if (query === lastQuery) {
                return;
            }
            lastQuery = query;

            if (inFlight) {
                inFlight.abort();
            }
            inFlight = new AbortController();

            var url = filterForm.getAttribute('action') + (query ? '?' + query : '');
            var results = document.getElementById('customer-results');
            if (results) {
                results.classList.add('is-loading');
            }
            if (status) {
                status.textContent = 'Searching…';
            }

            fetch(url, {
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                signal: inFlight.signal
            })
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error('HTTP ' + response.status);
                    }
                    return response.text();
                })
                .then(function (html) {
                    var current = document.getElementById('customer-results');
                    if (current) {
                        current.outerHTML = html;
                    }
                    var fresh = document.getElementById('customer-results');
                    if (fresh && countPill) {
                        countPill.textContent = fresh.getAttribute('data-count-text');
                    }
                    history.replaceState(null, '', url);
                    updateReturnLinks();
                    if (status) {
                        status.textContent = '';
                    }
                })
                .catch(function (err) {
                    if (err.name === 'AbortError') {
                        return;
                    }
                    window.location.href = url;
                });
        }

        function scheduleRefresh() {
            if (clearLink) {
                clearLink.hidden = !hasFilter();
            }
            clearTimeout(timer);
            timer = setTimeout(refresh, typingDelay);
        }

        function updateReturnLinks() {
            var here = window.location.pathname + window.location.search;
            document.querySelectorAll('a[data-return-link]').forEach(function (link) {
                var target = new URL(link.href, window.location.origin);
                target.searchParams.set('returnUrl', here);
                link.href = target.pathname + target.search;
            });
        }

        filterForm.querySelectorAll('.search-input').forEach(function (input) {
            input.addEventListener('input', scheduleRefresh);
        });

        filterForm.addEventListener('submit', function (e) {
            e.preventDefault();
            clearTimeout(timer);
            refresh();
        });

        if (clearLink) {
            clearLink.addEventListener('click', function (e) {
                e.preventDefault();
                filterForm.querySelectorAll('.search-input').forEach(function (input) {
                    input.value = '';
                });
                clearLink.hidden = true;
                clearTimeout(timer);
                refresh();
                filterForm.querySelector('.search-input').focus();
            });
        }
    }

    var modal = document.getElementById('delete-modal');
    if (!modal) {
        return;
    }

    var nameSlot = modal.querySelector('[data-modal-name]');
    var confirmBtn = modal.querySelector('[data-modal-confirm]');
    var pendingForm = null;
    var lastFocus = null;

    function openModal(form) {
        pendingForm = form;
        lastFocus = document.activeElement;
        nameSlot.textContent = form.getAttribute('data-customer-name') || 'this customer';
        modal.hidden = false;
        confirmBtn.focus();
    }

    function closeModal() {
        modal.hidden = true;
        pendingForm = null;
        if (lastFocus) {
            lastFocus.focus();
        }
    }

    document.addEventListener('submit', function (e) {
        var form = e.target;
        if (!form.matches('form[data-confirm-delete]') || form.dataset.confirmed === 'true') {
            return;
        }
        e.preventDefault();
        openModal(form);
    });

    confirmBtn.addEventListener('click', function () {
        if (!pendingForm) {
            return;
        }
        var form = pendingForm;
        form.dataset.confirmed = 'true';
        confirmBtn.disabled = true;
        confirmBtn.textContent = 'Deleting…';
        form.submit();
    });

    modal.querySelectorAll('[data-modal-cancel]').forEach(function (btn) {
        btn.addEventListener('click', closeModal);
    });

    modal.addEventListener('click', function (e) {
        if (e.target === modal) {
            closeModal();
        }
    });
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && !modal.hidden) {
            closeModal();
        }
    });
})();