using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Data;
public class DataContext(DbContextOptions options) : IdentityDbContext<AppUser,
AppRole, int, IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
IdentityRoleClaim<int>, IdentityUserToken<int>>(options)
{
    public DbSet<UserLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Connection> Connections { get; set; }
    public DbSet<Photo> Photos { get; set; }

    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>().HasMany(x => x.UserRoles).WithOne(x => x.User).HasForeignKey(x => x.UserId).IsRequired();
        modelBuilder.Entity<AppRole>().HasMany(x => x.UserRoles).WithOne(x => x.Role).HasForeignKey(x => x.RoleId).IsRequired();

        modelBuilder.Entity<UserLike>().HasKey(k => new { k.SourceUserId, k.TargetUserId });
        //likes modelling
        modelBuilder.Entity<UserLike>().
        HasOne(s => s.SourceUser).
        WithMany(l => l.LikedUsers).
        HasForeignKey(k => k.SourceUserId).
        OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserLike>().
        HasOne(t => t.TargetUser).
        WithMany(l => l.LikedByOtherUsers).
        HasForeignKey(k => k.TargetUserId).
        OnDelete(DeleteBehavior.Cascade);

        //messages modelling
        modelBuilder.Entity<Message>().HasOne(user => user.Sender).
        WithMany(messages => messages.SentMessages).
        OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>().
        HasOne(user => user.Recipient).
        WithMany(messages => messages.ReceivedMessages).
        OnDelete(DeleteBehavior.Restrict);

        //photo modelling
        modelBuilder.Entity<Photo>().HasQueryFilter(x => x.IsApproved == true);
    }
}