let stripe;
let elements;
let card;

export function initializeStripe(publishableKey, clientSecret){
    stripe = Stripe(publishableKey);
    elements = stripe.elements();
    card = elements.create('card');
    card.mount('#card-element');
    window.clientSecret = clientSecret;
}

export async function confirmCardPayment(){
    return await stripe.confirmCardPayment(window.clientSecret, {
        payment_method: {
            card: card
        }
    });
}