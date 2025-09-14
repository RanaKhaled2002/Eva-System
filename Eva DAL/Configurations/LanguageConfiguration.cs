using Eva_DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.Configurations
{
    public class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.HasKey(L => L.Id);

            builder.Property(L => L.Lang)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(L => L.Direction)
                   .IsRequired()
                   .HasMaxLength(3);

            builder.Property(L => L.LangShortCode)
                   .IsRequired()
                   .HasMaxLength(3);
        }
    }
}
