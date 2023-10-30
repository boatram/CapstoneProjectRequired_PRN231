using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BusinessObjects.BusinessObjects
{
    public partial class CPRContext : DbContext
    {
        public CPRContext()
        {
        }

        public CPRContext(DbContextOptions<CPRContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<GroupProject> GroupProjects { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Semester> Semesters { get; set; } = null!;
        public virtual DbSet<Specialization> Specializations { get; set; } = null!;
        public virtual DbSet<StudentInGroup> StudentInGroups { get; set; } = null!;
        public virtual DbSet<StudentInSemester> StudentInSemesters { get; set; } = null!;
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<Topic> Topics { get; set; } = null!;
        public virtual DbSet<TopicOfGroup> TopicOfGroups { get; set; } = null!;
        public virtual DbSet<TopicOfLecturer> TopicOfLecturers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=(local);uid=sa;pwd=123;database=CPR");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.Avatar).IsUnicode(false);

                entity.Property(e => e.Code).IsUnicode(false);

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.Gender)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Account__RoleId__33D4B598");

                entity.HasOne(d => d.Specialization)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.SpecializationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Account__Special__34C8D9D1");
            });

            modelBuilder.Entity<GroupProject>(entity =>
            {
                entity.ToTable("GroupProject");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Name).HasMaxLength(20);
            });

            modelBuilder.Entity<Semester>(entity =>
            {
                entity.ToTable("Semester");

                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");
            });

            modelBuilder.Entity<Specialization>(entity =>
            {
                entity.ToTable("Specialization");

                entity.Property(e => e.Code).HasMaxLength(10);

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<StudentInGroup>(entity =>
            {
                entity.ToTable("StudentInGroup");

                entity.Property(e => e.JoinDate).HasColumnType("date");

                entity.Property(e => e.Role).HasMaxLength(20);

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.StudentInGroups)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__StudentIn__Group__3F466844");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.StudentInGroups)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__StudentIn__Stude__403A8C7D");
            });

            modelBuilder.Entity<StudentInSemester>(entity =>
            {
                entity.ToTable("StudentInSemester");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.StudentInSemesters)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__StudentIn__Semes__3C69FB99");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.StudentInSemesters)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__StudentIn__Stude__3A81B327");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.StudentInSemesters)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__StudentIn__Subje__3B75D760");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("Subject");

                entity.Property(e => e.Code).HasMaxLength(10);

                entity.Property(e => e.IsPrerequisite).HasColumnName("isPrerequisite");

                entity.HasOne(d => d.Specialization)
                    .WithMany(p => p.Subjects)
                    .HasForeignKey(d => d.SpecializationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Subject__Special__37A5467C");
            });

            modelBuilder.Entity<Topic>(entity =>
            {
                entity.ToTable("Topic");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.Topics)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Topic__SemesterI__286302EC");

                entity.HasOne(d => d.Specialization)
                    .WithMany(p => p.Topics)
                    .HasForeignKey(d => d.SpecializationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Topic__Specializ__29572725");
            });

            modelBuilder.Entity<TopicOfGroup>(entity =>
            {
                entity.HasKey(e => new { e.TopicId, e.GroupProjectId });

                entity.ToTable("TopicOfGroup");

                entity.HasOne(d => d.GroupProject)
                    .WithMany(p => p.TopicOfGroups)
                    .HasForeignKey(d => d.GroupProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TopicOfGr__Group__2F10007B");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.TopicOfGroups)
                    .HasForeignKey(d => d.TopicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TopicOfGr__Topic__2E1BDC42");
            });

            modelBuilder.Entity<TopicOfLecturer>(entity =>
            {
                entity.HasKey(e => new { e.LecturerId, e.TopicId });

                entity.ToTable("TopicOfLecturer");

                entity.Property(e => e.IsSuperLecturer).HasColumnName("isSuperLecturer");

                entity.HasOne(d => d.Lecturer)
                    .WithMany(p => p.TopicOfLecturers)
                    .HasForeignKey(d => d.LecturerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TopicOfLe__Lectu__4316F928");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.TopicOfLecturers)
                    .HasForeignKey(d => d.TopicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TopicOfLe__Topic__440B1D61");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
