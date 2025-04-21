export function observeElement(dotNetHelper, element) {
    if (!element) {
        console.warn("Element was null – will not observe.");
        return;
    }

    console.log("Observer attached!");

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                console.log("Trigger reached - requesting next page...");
                dotNetHelper.invokeMethodAsync('LoadMoreArtworks');
            }
        });
    });

    observer.observe(element);
}
