using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IUserRepository
{
    void Update(AppUser appUser);
    Task<bool> SaveAllAsync();
    Task<IEnumerable<AppUser>> GetAllUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByUsernameAsync(string username);
    Task<PagedList<MemberDTO>> GetAllMembersAsync(UserParams userParams);
    Task<MemberDTO?> GetMemberByUsernameAsync(string username);


}