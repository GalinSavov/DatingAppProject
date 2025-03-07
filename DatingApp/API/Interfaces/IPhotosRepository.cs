using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IPhotosRepository
{
    Task<IEnumerable<PhotoForApprovalDTO>> GetUnapprovedPhotos();
    Task<Photo?> GetPhotoById(int photoId);
    void RemovePhoto(Photo photo);
}