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
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");

            builder.HasKey(p => p.Id);

            builder.Property(s => s.FirstName)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.Property(s => s.PersonalPhone)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(s => s.Code)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(s => s.GuardianPhone)
                   .HasMaxLength(20);

            builder.HasOne(s => s.Grade)
                   .WithMany()
                   .HasForeignKey("GradeId");
        }
    }

}
