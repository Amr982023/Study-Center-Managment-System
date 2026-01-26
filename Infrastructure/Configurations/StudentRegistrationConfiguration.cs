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
                   .WithMany("_registrations")
                   .HasForeignKey("StudentId");

            builder.HasOne(r => r.ClassSession)
                   .WithMany()
                   .HasForeignKey("ClassSessionId");
        }
    }

}
