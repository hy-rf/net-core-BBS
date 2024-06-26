using BBS.Models;
using Microsoft.EntityFrameworkCore;

namespace BBS.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<User> User { get; set; }
    public DbSet<Post> Post { get; set; }
    public DbSet<Reply> Reply { get; set; }
    public DbSet<Tag> Tag { get; set; }
    public DbSet<PostTag> PostTag { get; set; }
    public DbSet<Friend> Friend { get; set; }
    public DbSet<FriendRequest> FriendRequest { get; set; }
    public DbSet<ChatRoom> ChatRoom { get; set; }
    public DbSet<ChatRoomMember> ChatRoomMember { get; set; }
    public DbSet<ChatRoomMessage> ChatRoomMessage { get; set; }
    public DbSet<LikedPost> LikedPost { get; set; }
    public DbSet<LikedReply> LikedReply { get; set; }
    public DbSet<Notification> Notification { get; set; }
}