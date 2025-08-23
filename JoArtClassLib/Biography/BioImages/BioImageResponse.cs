namespace JoArtClassLib.About;

public class BioImageResponse
{
    public required int Id { get; set; }
    
    public int OrderIndex { get; set; }
    
    public required string Key { get; set; }
    public required string Url { get; set; } // Pre signed URL
    
}