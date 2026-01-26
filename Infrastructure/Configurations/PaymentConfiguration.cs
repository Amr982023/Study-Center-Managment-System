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
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                   .HasColumnType("decimal(10,2)")
                   .IsRequired();

            builder.Property(p => p.Month)
                   .IsRequired();

            builder.HasOne(p => p.Student)
                   .WithMany()
                   .HasForeignKey("StudentId");

            builder.HasOne(p => p.PerformedBy)
                   .WithMany()
                   .HasForeignKey("PerformedByUserId");
        }
    }

}
