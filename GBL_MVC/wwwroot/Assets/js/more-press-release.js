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
});