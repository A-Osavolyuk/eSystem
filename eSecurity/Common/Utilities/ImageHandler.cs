using eSecurity.Common.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace eSecurity.Common.Utilities;

public static class ImageHandler
{
    public static async Task<List<ImageModel>> ConvertAsync(IReadOnlyList<IBrowserFile> files)
    {
        var images = new List<ImageModel>();
        
        foreach (var file in files)
        {
            var src = await ToSourceAsync(file);

            var image = new ImageModel()
            {
                Id = Guid.CreateVersion7(),
                File = file,
                Source = src,
            };
            
            images.Add(image);
        }
        
        return images;
    }
    
    private static async Task<string> ToSourceAsync(IBrowserFile file)
    {
        await using var stream = file.OpenReadStream(10 * 1024 * 1024);
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var bytes = ms.ToArray();
        var base64 = Convert.ToBase64String(bytes);
        var contentType = file.ContentType;
        var base64Image = $"data:{contentType};base64,{base64}";
        return base64Image;
    }
}