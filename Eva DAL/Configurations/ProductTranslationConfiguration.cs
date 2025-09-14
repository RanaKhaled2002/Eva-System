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
    public class ProductTranslationConfiguration : IEntityTypeConfiguration<ProductTranslation>
    {
        public void Configure(EntityTypeBuilder<ProductTranslation> builder)
        {
            builder.HasKey(pt => pt.Id);

            builder.Property(pt => pt.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(pt => pt.MediaDescription)
                   .HasMaxLength(1000);

            builder.Property(pt => pt.Instruction)
                   .HasMaxLength(1000);

            builder.HasOne(pt => pt.Product)
                   .WithMany(p => p.Translations)
                   .HasForeignKey(pt => pt.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pt => pt.Language)
                   .WithMany(l => l.ProductTranslations)
                   .HasForeignKey(pt => pt.LanguageId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
