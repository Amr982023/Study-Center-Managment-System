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
    public class ExamStatusConfiguration : IEntityTypeConfiguration<ExamStatus>
    {
        public void Configure(EntityTypeBuilder<ExamStatus> builder)
        {
            builder.ToTable("ExamStatuses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(50);
        }
    }

}
