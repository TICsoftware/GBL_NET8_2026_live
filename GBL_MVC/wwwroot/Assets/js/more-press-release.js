document.addEventListener('DOMContentLoaded', function () {
    new Swiper('.press-release-swiper', {
        slidesPerView: 1.08,
        spaceBetween: 16,
        navigation: {
            nextEl: '.press-release-nav--next',
            prevEl: '.press-release-nav--prev'
        },
        pagination: {
            el: '.press-release-pagination',
            clickable: true
        },
        breakpoints: {
            // From this width up, Swiper becomes inert (no drag/nav/pagination)
            // and simply lays the slides out in a row — used as a static grid.
            769: {
                enabled: false,
                slidesPerView: 3,
                spaceBetween: 24
            }
        }
    });

    /* Hover fill — green circle grows from the mouse entry point,
       same as the Press Release / Media Coverage listing cards */
    function placeFill(card, e) {
        var rect = card.getBoundingClientRect();
        var w = rect.width;
        var h = rect.height;
        var x = e ? e.clientX - rect.left : w / 2;
        var y = e ? e.clientY - rect.top : h / 2;
        x = Math.min(Math.max(x, 0), w);
        y = Math.min(Math.max(y, 0), h);
        var radius = Math.ceil(Math.hypot(Math.max(x, w - x), Math.max(y, h - y))) + 16;
        card.style.setProperty('--hx', x + 'px');
        card.style.setProperty('--hy', y + 'px');
        card.style.setProperty('--hr', radius + 'px');
    }

    document.querySelectorAll('.press-release-mini-card').forEach(function (card) {
        placeFill(card, null);
        card.addEventListener('mouseenter', function (e) {
            placeFill(card, e);
            card.classList.add('is-filled');
        });
        card.addEventListener('mouseleave', function () {
            card.classList.remove('is-filled');
        });
    });
});