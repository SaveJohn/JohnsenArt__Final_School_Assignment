
window.resetCarouselToFirst = (selector) => {
    const el = document.querySelector(selector);
    if (!el) return;
    
    const inst = bootstrap.Carousel.getInstance(el)
        || new bootstrap.Carousel(el, { ride: false });
    inst.to(0);
};