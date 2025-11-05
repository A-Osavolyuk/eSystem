using Microsoft.AspNetCore.Components.Forms;

namespace eSecurity.Common.Models;

public class ImageModel
{
    public Guid Id { get; set; }
    public required string Source { get; set; }
    public required IBrowserFile File { get; set; }
}