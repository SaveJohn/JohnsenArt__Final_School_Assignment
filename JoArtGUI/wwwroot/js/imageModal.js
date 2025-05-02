window.showImageModal = (thumbUrl, previewUrl, fullUrl) => {
    const img = document.getElementById('modalImage');

    img.srcset =
        `${thumbUrl} 300w, ${previewUrl} 800w, ${fullUrl} 1600w`;
    img.sizes =
        '(max-width: 576px) 100vw, ' +
        '(max-width: 992px) 80vw, ' +
        '60vw';
    img.src = fullUrl;
    img.onload = () => {
        console.log("🖼 currentSrc →", img.currentSrc);
        console.log("📐 natural size →", img.naturalWidth, "×", img.naturalHeight);
    };
    img.alt = '';

    const modal = new bootstrap.Modal(
        document.getElementById('imageModal'));
    modal.show();
};