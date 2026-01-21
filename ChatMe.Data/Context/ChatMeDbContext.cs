using ChatMe.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Reflection.Emit;

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
        public DbSet<ConversationMember> ConversationMembers { get; set; }
        public DbSet<MessageStatus> MessageStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(b =>
            {
                b.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
            });

            // Keep your existing Conversation -> Message cascade
            builder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            // THE FIX: Stop the Cycle in MessageStatus
            builder.Entity<MessageStatus>()
                .HasOne(ms => ms.User)     // The User relationship
                .WithMany()                // (Assuming User doesn't have a List<MessageStatus>)
                .HasForeignKey(ms => ms.UserId)
                .OnDelete(DeleteBehavior.NoAction); // <--- THIS FIXES THE ERROR
            // Optional: You can enforce foreign key behaviors here if needed
            // For example, if a Conversation is deleted, delete all Messages:
            //builder.Entity<Message>()
            //    .HasOne(m => m.Conversation)
            //    .WithMany(c => c.Messages)
            //    .HasForeignKey(m => m.ConversationId)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

//Add-Migration AddedReadReceipts -StartupProject ChatMe.API
//Update-Database -StartupProject ChatMe.API