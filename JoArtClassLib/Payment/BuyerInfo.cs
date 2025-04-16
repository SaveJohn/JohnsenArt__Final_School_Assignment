namespace JoArtClassLib.Payment;

public class BuyerInfo
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    
    public string DeliveryMethod { get; set; } = "";
    public string AddressLine { get; set; } = "";
    public string City { get; set; } = "";
    public string PostalCode { get; set; } = "";
}