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

            builder.HasKey(u => u.Id);

            builder.Property(u => u.UserName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(u => u.UserName).IsUnique();          

            builder.Property(u => u.HashedPassword)
                   .IsRequired().HasMaxLength(500);

            builder.HasOne(u => u.Person)
                   .WithMany()
                   .HasForeignKey("PersonId")
                   .IsRequired();
        }
    }

}
