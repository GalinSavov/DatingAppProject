using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IUserRepository
{
    void Update(AppUser appUser);
    Task<IEnumerable<AppUser>> GetAllUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByPhotoId(int photoId);
    Task<AppUser?> GetUserByUsernameAsync(string username);
    Task<PagedList<MemberDTO>> GetAllMembersAsync(UserParams userParams);
    Task<MemberDTO?> GetMemberByUsernameAsync(string username, bool isCurrentUser);


}