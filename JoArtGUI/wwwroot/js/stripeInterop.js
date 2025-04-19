let stripe;
let elements;

export async function initializeStripe(publishableKey, clientSecret) {
    console.log("initializeStripe started");
    
    if (typeof Stripe === "undefined") {
        console.error("Stripe.js er ikke lastet!");
        return;
    }
    
    await waitForStripe();
    
    window.stripe = Stripe(publishableKey);
    if (!window.stripe) {
        console.error("Stripe() returnerte undefined");
        return;
    }
    
    window.clientSecret = clientSecret;
    localStorage.setItem('stripeKey', publishableKey);
    elements =  window.stripe.elements();

    // Wait for #elements to exist in DOM before mounting Stripe card element
    await waitForElement("#card-number-element");
    await waitForElement("#card-expiry-element");
    await waitForElement("#card-cvc-element");
    
    const style = {
        base: {
            fontSize: "16px",
            color: '#32325d',
            '::placeholder': {
                color: '#aab7c4',
            },
            iconColor: '#666EE8',
        },
        invalid: {
            color: '#fa755a',
            iconColor: '#fa755a'
        }
    };
    
    const cardNumber = elements.create('cardNumber', {style});
    const cardExpiry = elements.create('cardExpiry', {style});
    const cardCvc = elements.create('cardCvc', {style});
    
    cardNumber.mount('#card-number-element');
    cardExpiry.mount('#card-expiry-element');
    cardCvc.mount('#card-cvc-element');
    
    window.cardElements = {
        number: cardNumber,
        expiry: cardExpiry,
        cvc: cardCvc
    };
    
    console.log("Stripe elements loaded");

}

async function waitForStripe() {
    return new Promise((resolve, reject) => {
        const interval = setInterval(() => {
            if (typeof Stripe !== "undefined") {
                clearInterval(interval);
                resolve();
            }
        }, 50);

        setTimeout(() => {
            clearInterval(interval);
            reject("Stripe.js tok for lang tid å laste");
        }, 5000); // timeout etter 5 sekunder
    });
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

export async function mountCardElements() {
    if (!window.cardElements) {
        console.warn('Card elements not initialized');
        return;
    }

    try{
        await window.cardElements.number.unmount();
        await window.cardElements.expiry.unmount();
        await window.cardElements.cvc.unmount();
    }
    catch(e) {
        console.warn("Unmounting card elements failed", e);
    }

    await waitForElement("#card-number-element");
    await waitForElement("#card-expiry-element");
    await waitForElement("#card-cvc-element");

    window.cardElements.number.mount('#card-number-element');
    window.cardElements.expiry.mount('#card-expiry-element');
    window.cardElements.cvc.mount('#card-cvc-element');

    console.log("Stripe elements remounted");

}

export async function confirmCardPayment() {
    const result = await window.stripe.confirmCardPayment(window.clientSecret, {
        payment_method: {
                card: window.cardElements.number,
            billing_details: {
                name: document.querySelector('input[name="fullName"]').value,
                email: document.querySelector('input[type="email"]').value
            }
        }
    });

    return {
        error: result.error ?? null,
        paymentIntent: result.paymentIntent ?? null
    };
}

export async function confirmKlarnaPayment() {
    const result = await window.stripe.confirmKlarnaPayment(window.clientSecret, {
        payment_method: {
            billing_details: {
                email: document.querySelector('input[type=email]').value,
                name: document.querySelector('input[name="fullName"]').value
            }
        },
        return_url: `${window.location.origin}/checkout/${window.productId}?payment_intent_client_secret=${window.clientSecret}`

    });
    if (!window.productId) {
        console.warn("ProductId not set before Klarna redirect!");
    }
    return {
        error: result.error ?? null,
        paymentIntent: result.paymentIntent ?? null
    };
}

export function setProductId(id) {
    window.productId = id;
}

export async function checkPaymentStatus() {
    const urlParams = new URLSearchParams(window.location.search);
    const clientSecret = urlParams.get('payment_intent_client_secret');

    if (!clientSecret) return null;
    
    if (!window.stripe) {
        const storedKey = localStorage.getItem('stripeKey');
        if (!storedKey) {
            console.warn('Stripe key is missing in localStorage');
            return null;
        }
        window.stripe = Stripe(storedKey);
    }
    
    const result = await window.stripe.retrievePaymentIntent(clientSecret);
    return result.paymentIntent?.status ?? null;
}

