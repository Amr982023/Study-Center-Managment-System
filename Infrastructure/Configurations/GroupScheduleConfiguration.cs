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
    public class GroupScheduleConfiguration : IEntityTypeConfiguration<GroupSchedule>
    {
        public void Configure(EntityTypeBuilder<GroupSchedule> builder)
        {
            builder.ToTable("GroupSchedules");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Day)
                   .IsRequired();

            builder.Property(x => x.Time)
                   .IsRequired();

            builder.HasOne(x => x.Group)
                   .WithMany()
                   .HasForeignKey("GroupId")
                   .IsRequired();
        }
    }

}
