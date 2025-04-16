let stripe;
let elements;
let card;

export async function initializeStripe(publishableKey, clientSecret) {
    stripe = Stripe(publishableKey);
    elements = stripe.elements();

    // Wait for #card-element to exist in DOM before mounting Stripe card element
    await waitForElement("#card-element");
    
    card = elements.create('card', {
        hidePostalCode: true
    });
    card.mount('#card-element');
    window.clientSecret = clientSecret;
    console.log("stripeInterop.js loaded");

}

async function waitForElement(selector) {
    return new Promise(resolve => {
        if (document.querySelector(selector)) return resolve();
        
        const observer = new MutationObserver(() => {
            if (document.querySelector(selector)) {
                observer.disconnect();
                resolve();
            }
        });
        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    });
}

export async function confirmCardPayment() {
    return await stripe.confirmCardPayment(window.clientSecret, {
        payment_method: {
            card: card
        }
    });
}