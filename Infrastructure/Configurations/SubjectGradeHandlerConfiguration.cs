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
    public class SubjectGradeHandlerConfiguration
    : IEntityTypeConfiguration<SubjectGradeHandler>
    {
        public void Configure(EntityTypeBuilder<SubjectGradeHandler> builder)
        {
            builder.ToTable("SubjectGradeHandlers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SessionFees)
                   .IsRequired()
                   .HasColumnType("decimal(10,2)");

            builder.HasOne(x => x.Subject)
                   .WithMany()
                   .HasForeignKey("SubjectId");

            builder.HasOne(x => x.Grade)
                   .WithMany()
                   .HasForeignKey("GradeId");
        }
    }

}
