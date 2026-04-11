// Hero swipe card tracking
window.setupHeroSwipe = function (dotnetRef) {
    const el = document.getElementById('heroSwipe');
    if (!el) return;
    let timer;
    el.addEventListener('scroll', function () {
        clearTimeout(timer);
        timer = setTimeout(function () {
            const w = el.clientWidth;
            if (w === 0) return;
            const idx = Math.round(el.scrollLeft / w);
            dotnetRef.invokeMethodAsync('OnCardChanged', idx);
        }, 120);
    }, { passive: true });
};

window.scrollHeroToCard = function (index) {
    const el = document.getElementById('heroSwipe');
    if (!el) return;
    el.scrollTo({ left: el.clientWidth * index, behavior: 'smooth' });
};
