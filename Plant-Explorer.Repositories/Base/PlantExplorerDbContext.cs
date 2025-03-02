using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Plant_Explorer.Contract.Repositories.Entity;

namespace Plant_Explorer.Repositories.Base
{
    public class PlantExplorerDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public PlantExplorerDbContext(DbContextOptions<PlantExplorerDbContext> options)  // Constructor that accepts options
           : base(options)
        {

        }

        public virtual DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
        public virtual DbSet<ApplicationRole> ApplicationRoles => Set<ApplicationRole>();
        public virtual DbSet<Avatar> Avatars => Set<Avatar>();
        public virtual DbSet<Badge> Badges => Set<Badge>();
        public virtual DbSet<BugReport> BugReports => Set<BugReport>();
        public virtual DbSet<Option> Options => Set<Option>();
        public virtual DbSet<Plant> Plants => Set<Plant>();
        public virtual DbSet<Question> Questions => Set<Question>();
        public virtual DbSet<Quiz> Quizzes => Set<Quiz>();
        public virtual DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
        public virtual DbSet<ScanHistory> ScanHistories => Set<ScanHistory>();
        public virtual DbSet<UserBadge> UserBadges => Set<UserBadge>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Remove words "AspNet" in Identity Table
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                string tableName = entityType.GetTableName() ?? "";
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            // ApplicationUser - ApplicationRole (One-to-many)
            builder.Entity<ApplicationUser>(b =>
            {
                b.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<ApplicationRole>(b =>
            {
                b.HasMany(r => r.Users)
                    .WithOne(u => u.Role)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ApplicationUser - BugReport (One-to-many)
            builder.Entity<ApplicationUser>(b =>
            {
                b.HasMany(u => u.BugReports)
                    .WithOne(br => br.User)
                    .HasForeignKey(br => br.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // BugReport - ApplicationUser (One-to-many - from BugReport side)
            builder.Entity<BugReport>(b =>
            {
                b.HasOne(br => br.User)
                    .WithMany(u => u.BugReports)
                    .HasForeignKey(br => br.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ApplicationUser - ScanHistory (One-to-many)
            builder.Entity<ApplicationUser>(b =>
            {
                b.HasMany(u => u.ScanHistories)
                    .WithOne(sh => sh.User)
                    .HasForeignKey(sh => sh.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Plant - ScanHistory (One-to-many)
            builder.Entity<Plant>(b =>
            {
                b.HasMany(p => p.ScanHistories)
                    .WithOne(sh => sh.Plant)
                    .HasForeignKey(sh => sh.PlantId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ApplicationUser - Avatar (One-to-many)
            builder.Entity<ApplicationUser>(b =>
            {
                b.HasOne(u => u.Avatar)
                    .WithMany(a => a.Users)
                    .HasForeignKey(u => u.AvatarId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Avatar - ApplicationUser (One-to-many - from Avatar side)
            builder.Entity<Avatar>(b =>
            {
                b.HasMany(a => a.Users)
                    .WithOne(u => u.Avatar)
                    .HasForeignKey(u => u.AvatarId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Badge - UserBadge (One-to-many)
            builder.Entity<Badge>(b =>
            {
                b.HasMany(bg => bg.UserBadges)
                    .WithOne(ub => ub.Badge)
                    .HasForeignKey(ub => ub.BadgeId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ApplicationUser - UserBadge (One-to-many)
            builder.Entity<ApplicationUser>(b =>
            {
                b.HasMany(u => u.UserBadges)
                    .WithOne(ub => ub.User)
                    .HasForeignKey(ub => ub.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ApplicationUser - UserPoint (One-to-one)
            builder.Entity<ApplicationUser>(b =>
            {
                b.HasOne(u => u.UserPoint)
                    .WithOne(up => up.User)
                    .HasForeignKey<UserPoint>(up => up.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Quiz - Question (One-to-many)
            builder.Entity<Quiz>(b =>
            {
                b.HasMany(q => q.Questions)
                    .WithOne(qn => qn.Quiz)
                    .HasForeignKey(qn => qn.QuizId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Quiz - QuizAttempt (One-to-many)
            builder.Entity<Quiz>(b =>
            {
                b.HasMany(q => q.QuizAttempts)
                    .WithOne(qa => qa.Quiz)
                    .HasForeignKey(qa => qa.QuizId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ApplicationUser - QuizAttempt (One-to-many)
            builder.Entity<ApplicationUser>(b =>
            {
                b.HasMany(u => u.QuizAttempts)
                    .WithOne(qa => qa.User)
                    .HasForeignKey(qa => qa.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Question - Option (One-to-many)
            builder.Entity<Question>(b =>
            {
                b.HasMany(q => q.Options)
                    .WithOne(o => o.Question)
                    .HasForeignKey(o => o.QuestionId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Plant - FavoritePlant (One-to-many)
            builder.Entity<Plant>(b =>
            {
                b.HasMany(p => p.FavoritePlants)
                    .WithOne(fp => fp.Plant)
                    .HasForeignKey(fp => fp.PlantId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // FavoritePlant - ApplicationUser (One-to-many)
            builder.Entity<FavoritePlant>(b =>
            {
                b.HasOne(fp => fp.User)
                    .WithMany(u => u.FavoritePlants)
                    .HasForeignKey(fp => fp.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Plant - PlantCharacteristic (One-to-many)
            builder.Entity<Plant>(b =>
            {
                b.HasMany(p => p.PlantCharacteristics)
                    .WithOne(pc => pc.Plant)
                    .HasForeignKey(fp => fp.PlantId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // PlantCharacteristic - CharacteristicCategory (One-to-many)
            builder.Entity<PlantCharacteristic>(b =>
            {
                b.HasOne(pc => pc.CharacteristicCategory)
                    .WithMany(cc => cc.PlantCharacteristics)
                    .HasForeignKey(pc => pc.CharacteristicCategoryId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Plant - PlantApplication (One-to-many)
            builder.Entity<Plant>(b =>
            {
                b.HasMany(p => p.PlantApplications)
                    .WithOne(pc => pc.Plant)
                    .HasForeignKey(fp => fp.PlantId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // PlantApplication - ApplicationCategory (One-to-many)
            builder.Entity<PlantApplication>(b =>
            {
                b.HasOne(pa => pa.ApplicationCategory)
                    .WithMany(ac => ac.PlantApplications)
                    .HasForeignKey(pa => pa.ApplicationCategoryId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Plant - PlantImage (One-to-many)
            builder.Entity<Plant>(b =>
            {
                b.HasMany(p => p.PlantImages)
                    .WithOne(pi => pi.Plant)
                    .HasForeignKey(pi => pi.PlantId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

        }
    }
}