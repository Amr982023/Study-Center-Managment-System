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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(p => p.Id);

            builder.Property(s => s.FirstName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(s => s.PersonalPhone)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(s => s.Email)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.UserName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(u => u.UserName).IsUnique();          

            builder.Property(u => u.HashedPassword)
                   .IsRequired().HasMaxLength(500);

            
        }
    }

}
