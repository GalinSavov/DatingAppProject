using API.Data;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.DTOs;

public class PhotosRepository(DataContext dataContext, IMapper mapper) : IPhotosRepository
{
    public async Task<Photo?> GetPhotoById(int photoId)
    {
        return await dataContext.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == photoId) ?? throw new Exception("Photo could not be found");
    }
    //because of query filter, the unapproved photos will be returned by default
    public async Task<IEnumerable<PhotoForApprovalDTO>> GetUnapprovedPhotos()
    {
        var query = dataContext.Photos.IgnoreQueryFilters().Where(x => x.IsApproved == false).AsQueryable();
        return await query.ProjectTo<PhotoForApprovalDTO>(mapper.ConfigurationProvider).ToListAsync();
    }
    public void RemovePhoto(Photo photo)
    {
        dataContext.Photos.Remove(photo);
    }
}