using Plant_Explorer.Contract.Repositories.ModelViews.ImageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(string imagePath);
        Task<ImageRecord> GetImageAsync(string documentId);
    }
}
