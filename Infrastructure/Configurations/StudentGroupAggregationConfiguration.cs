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
    public class StudentGroupAggregationConfiguration
        : IEntityTypeConfiguration<StudentGroupAggregation>
    {
        public void Configure(EntityTypeBuilder<StudentGroupAggregation> builder)
        {
            builder.ToTable("StudentGroupAggregations");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Student)
                   .WithMany()
                   .HasForeignKey("StudentId")
                   .OnDelete(DeleteBehavior.NoAction)   // ✅ FIX
                   .IsRequired();

            builder.HasOne(x => x.Group)
                   .WithMany()
                   .HasForeignKey("GroupId")
                   .OnDelete(DeleteBehavior.NoAction)   // ✅ FIX
                   .IsRequired();

            builder.HasIndex("StudentId", "GroupId")
                   .IsUnique();
        }
    }

}
