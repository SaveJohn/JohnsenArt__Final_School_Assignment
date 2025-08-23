using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JoArtClassLib.About;

public class BioImage
{
    [Key] public int Id { get; set; }
    [ForeignKey(nameof(Id))] public int BioBlockId { get; set; }
    
    public int OrderIndex { get; set; }
    
    [Required] public string Key { get; set; } = default!;
    
    public string? Alt { get; set; }
    public string? Caption { get; set; }
    
    public virtual BioBlock BioBlock { get; set; } = default!;
    
    
}