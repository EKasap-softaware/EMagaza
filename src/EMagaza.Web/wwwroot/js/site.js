// Toast: birkaç saniye sonra kendiliğinden kaybolur
(function () {
    const t = document.getElementById('toast');
    if (t) setTimeout(() => t.classList.add('toast-hide'), 4500);
})();

// Zil rozeti: Worker yeni bildirim yazdığında sayfayı yenilemeden güncellenir (4 sn'de bir yoklama).
(function () {
    if (!window.__signedIn) return;
    const badge = document.getElementById('bell-badge');
    if (!badge) return;
    async function poll() {
        try {
            const r = await fetch('/Notifications/Count', { headers: { Accept: 'application/json' } });
            if (!r.ok) return;
            const { unread } = await r.json();
            const prev = parseInt(badge.textContent || '0', 10) || 0;
            badge.textContent = unread;
            badge.hidden = unread === 0;
            if (unread > prev) { badge.classList.remove('pulse'); void badge.offsetWidth; badge.classList.add('pulse'); }
        } catch { /* ağ hatası: bir sonraki turda tekrar */ }
    }
    setInterval(poll, 4000);
})();

// Beden seçimi / kart alanı biçimlendirme
(function () {
    const num = document.querySelector('[data-card-number]');
    if (num) num.addEventListener('input', () => {
        const d = num.value.replace(/\D/g, '').slice(0, 19);
        num.value = d.replace(/(.{4})/g, '$1 ').trim();
    });
    const exp = document.querySelector('[data-card-expiry]');
    if (exp) exp.addEventListener('input', () => {
        let d = exp.value.replace(/\D/g, '').slice(0, 4);
        if (d.length >= 3) d = d.slice(0, 2) + '/' + d.slice(2);
        exp.value = d;
    });
    const cvc = document.querySelector('[data-card-cvc]');
    if (cvc) cvc.addEventListener('input', () => { cvc.value = cvc.value.replace(/\D/g, '').slice(0, 4); });
    document.querySelectorAll('[data-fill-card]').forEach(b => b.addEventListener('click', () => {
        if (!num) return;
        num.value = b.dataset.fillCard; num.dispatchEvent(new Event('input'));
        if (exp) { exp.value = '12/30'; }
        if (cvc) cvc.value = '123';
        const h = document.querySelector('[name=CardHolder]'); if (h && !h.value) h.value = 'TEST KULLANICI';
    }));
})();

// Sipariş sayfası: bildirimin Worker (RabbitMQ) tarafından yazılmasını canlı bekle
(function () {
    const box = document.getElementById('notif-proof');
    if (!box) return;
    const { orderId } = box.dataset;
    let tries = 0;
    async function check() {
        tries++;
        try {
            const r = await fetch(`/Notifications/ForOrder?orderId=${encodeURIComponent(orderId)}&kind=paid`);
            const j = await r.json();
            if (j.found) {
                box.classList.add('ok');
                box.querySelector('.np-state').textContent = 'Bildirimin ulaştı';
                box.querySelector('.np-text').textContent = j.message;
                return;
            }
        } catch { }
        if (tries < 30) setTimeout(check, 1000);
        else box.querySelector('.np-text').textContent = 'Bildirim henüz ulaşmadı (Worker kapalı olabilir). Worker açılınca otomatik gelecek.';
    }
    check();
})();

// Ödeme sayfası: kayıtlı adrese tıklayınca form alanları dolar
document.querySelectorAll('[data-fill-address]').forEach(b => b.addEventListener('click', () => {
    const set = (name, value) => { const e = document.querySelector(`[name=${name}]`); if (e) e.value = value; };
    set('CustomerName', b.dataset.name);
    set('Phone', b.dataset.phone);
    set('City', b.dataset.city);
    set('Address', b.dataset.line);
}));
