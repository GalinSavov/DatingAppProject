using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Data;
public class DataContext(DbContextOptions options) : DbContext(options)
{
    public required DbSet<AppUser> Users { get; set; }
    public required DbSet<UserLike> Likes { get; set; }
    public required DbSet<Message> Messages { get; set; }

    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
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
    }
}