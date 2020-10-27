using DatingAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingAPI.Services.Photo
{
  public interface IPhotoServices
  {
    Task<string> GetPhoto(string photoId);
    Task<bool> AddPhoto(PhotoModel photoModel);
    Task<List<PhotoModel>> GetPhotos(string userId);
  }
}
