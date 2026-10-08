using System.ComponentModel.DataAnnotations;

namespace School2.Core.DTO;

public class FileToDatabaseDTO
{
    [Key]
    public Guid ImageId { get; set; }
    public string? ImageTitle  { get; set; }
    public byte[]? ImageData { get; set; }
    public Guid? LecturerId { get; set; }
}