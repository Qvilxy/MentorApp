using System.Data.Entity;
using MentorApp.Models;

namespace MentorApp.Data
{
    public class MentorContext : DbContext
    {
        public MentorContext() : base("MentorConnection") { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Direction> Directions { get; set; }
        public DbSet<Mentor> Mentors { get; set; }
        public DbSet<Mentee> Mentees { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Отключаем каскадное удаление для всех связей
            modelBuilder.Entity<Mentor>()
                .HasRequired(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Mentor>()
                .HasRequired(m => m.Direction)
                .WithMany()
                .HasForeignKey(m => m.DirectionID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Mentee>()
                .HasRequired(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Mentee>()
                .HasRequired(m => m.Direction)
                .WithMany()
                .HasForeignKey(m => m.DirectionID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Application>()
                .HasRequired(a => a.Mentee)
                .WithMany()
                .HasForeignKey(a => a.MenteeID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Application>()
                .HasRequired(a => a.Mentor)
                .WithMany()
                .HasForeignKey(a => a.MentorID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Meeting>()
                .HasRequired(m => m.Application)
                .WithMany()
                .HasForeignKey(m => m.ApplicationID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Review>()
                .HasRequired(r => r.Mentor)
                .WithMany()
                .HasForeignKey(r => r.MentorID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Review>()
                .HasRequired(r => r.Mentee)
                .WithMany()
                .HasForeignKey(r => r.MenteeID)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}