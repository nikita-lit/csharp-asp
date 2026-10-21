using Microsoft.AspNetCore.Http;

namespace School2.Core.DTO;

public class EmailDTO
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public List<IFormFile> Attachment { get; set; } = new();
}