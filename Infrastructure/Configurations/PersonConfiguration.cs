

using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    internal class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("People");

            builder.HasKey(p => p.Id);

            builder.Ignore(p => p.FullName);

            builder.Property(s => s.FirstName)
               .IsRequired()
               .HasMaxLength(50);

            builder.Property(s => s.LastName)
                    .IsRequired()
                   .HasMaxLength(50);

            builder.Property(s => s.PersonalPhone)
                   .IsRequired()
                   .HasMaxLength(20);


        }
    }
}
