using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Persons");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(p => p.FirstName);

            builder.Property(p => p.MidName)
                   .HasMaxLength(100);

            builder.Property(p => p.LastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.PersonalPhone)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.HasIndex(p => p.PersonalPhone)
                   .IsUnique();

            builder.Property(p => p.Gender)
                   .IsRequired()
                   .HasMaxLength(10);
        }
    }

}
