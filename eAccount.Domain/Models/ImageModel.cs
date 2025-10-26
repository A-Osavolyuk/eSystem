using Microsoft.AspNetCore.Components.Forms;

namespace eAccount.Domain.Models;

public class ImageModel
{
    public Guid Id { get; set; }
    public required string Source { get; set; }
    public required IBrowserFile File { get; set; }
}