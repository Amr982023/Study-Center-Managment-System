using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configurations
{
    public class StudentRegistrationConfiguration
    : IEntityTypeConfiguration<StudentRegistration>
    {
        public void Configure(EntityTypeBuilder<StudentRegistration> builder)
        {
            builder.ToTable("StudentRegistrations");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.RegistrationTime)
                   .IsRequired();

            builder.HasOne(r => r.Student)
                   .WithMany(s => s.Registrations)
                   .HasForeignKey("StudentId")
                   .OnDelete(DeleteBehavior.NoAction);   // ⭐ FIX

            builder.HasOne(r => r.ClassSession)
                   .WithMany()
                   .HasForeignKey("ClassSessionId")
                   .OnDelete(DeleteBehavior.NoAction);   // ⭐ IMPORTANT
        }
    }

}
