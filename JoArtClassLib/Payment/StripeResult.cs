namespace JoArtClassLib.Payment;

public class StripeResult
{
    public  StripeError? error { get; set; }
    public StripePaymentIntent? paymentIntent { get; set; }

    public class StripeError
    {
        public string message { get; set; } = "";
    }
    
    public class StripePaymentIntent
    {
        public string status { get; set; } = "";
    }
    
}