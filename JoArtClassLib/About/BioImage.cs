namespace JoArtClassLib.About;

public class BioImage
{
    public int Id { get; set; }
    public int BioBlockId { get; set; }
    public int OrderIndex { get; set; }
    
    public string Key { get; set; } = default!;
    public string Url { get; set; } = default!;
    public string? Alt { get; set; }
    public string? Caption { get; set; }
    
    public virtual BioBlock BioBlock { get; set; } = default!;
    
}