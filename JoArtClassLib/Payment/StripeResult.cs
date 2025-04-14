namespace JoArtClassLib.Payment;

public class StripeResult
{
    public  StripeError? error { get; set; }

    public class StripeError
    {
        public string message { get; set; } = "";
    }
}