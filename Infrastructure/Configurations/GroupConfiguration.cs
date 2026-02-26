using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Models;

namespace Infrastructure.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("Groups");

            builder.HasKey(g => g.Id);

            builder.HasIndex(g => g.SubjectGradeHandlerId)
                   .IsUnique();

            builder.Property(g => g.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(g => g.FirstSessionDate)
                   .IsRequired();

            builder.HasOne(g => g.SubjectGrade)
                   .WithMany()
                   .HasForeignKey(g => g.SubjectGradeHandlerId);

            builder.HasMany(g => g.Sessions)
              .WithOne()
              .HasForeignKey("GroupId");

            builder.Navigation(g => g.Sessions)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }

    }
}
