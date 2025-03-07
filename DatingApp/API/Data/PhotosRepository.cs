using API.Data;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.DTOs;

public class PhotosRepository(DataContext dataContext) : IPhotosRepository
{
    public async Task<Photo?> GetPhotoById(int photoId)
    {
        return await dataContext.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == photoId) ?? throw new Exception("Photo could not be found");
    }
    //because of query filter, the unapproved photos will be returned by default
    public async Task<IEnumerable<PhotoForApprovalDTO>> GetUnapprovedPhotos()
    {
        return await dataContext.Photos
        .IgnoreQueryFilters()
        .Where(p => p.IsApproved == false)
        .Select(u => new PhotoForApprovalDTO
        {
            Id = u.Id,
            Username = u.AppUser.UserName,
            Url = u.Url,
            IsApproved = u.IsApproved
        }).ToListAsync();
    }
    public void RemovePhoto(Photo photo)
    {
        dataContext.Photos.Remove(photo);
    }
}