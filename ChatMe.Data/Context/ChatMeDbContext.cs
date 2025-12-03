using ChatMe.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace ChatMe.Data.Context
{
    public class ChatMeDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,string,
        ApplicationUserClaim, ApplicationUserRole, IdentityUserLogin<string>, ApplicationRoleClaim, IdentityUserToken<string>>

    {
        public ChatMeDbContext(DbContextOptions<ChatMeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    // Optional: You can enforce foreign key behaviors here if needed
        //    // For example, if a Conversation is deleted, delete all Messages:
        //    builder.Entity<Message>()
        //        .HasOne(m => m.Conversation)
        //        .WithMany(c => c.Messages)
        //        .HasForeignKey(m => m.ConversationId)
        //        .OnDelete(DeleteBehavior.Cascade);
        //}
    }
}