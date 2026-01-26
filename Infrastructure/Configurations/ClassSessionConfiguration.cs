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
    public class ClassSessionConfiguration
      : IEntityTypeConfiguration<ClassSession>
    {
        public void Configure(EntityTypeBuilder<ClassSession> builder)
        {
            builder.ToTable("ClassSessions");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.SessionNumber)
                   .IsRequired();

            builder.Property(s => s.SessionDateTime)
                   .IsRequired();

            builder.HasOne(s => s.Group)
                   .WithMany("_sessions")
                   .HasForeignKey("GroupId");

            builder.HasOne(s => s.Status)
                   .WithMany()
                   .HasForeignKey("SessionStatusId");
        }
    }

}
