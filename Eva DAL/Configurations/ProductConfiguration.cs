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
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(P => P.Id);

            builder.Property(P => P.Name)
              .IsRequired()
              .HasMaxLength(200);

            builder.Property(P => P.Size)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(P => P.Quantity)
                   .HasPrecision(18,2)
                   .IsRequired();

            builder.Property(P => P.MediaDescription)
                   .HasMaxLength(1000);

            builder.Property(P => P.Instruction)
                   .HasMaxLength(1000);

            builder.Property(P => P.TranslationKey)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(P => P.TranslationKey)
                   .IsUnique();

            builder.HasOne(P => P.Category)
                   .WithMany(C => C.Products)
                   .HasForeignKey(P => P.categoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(P => P.ProductIngredients)
                   .WithOne(PI => PI.Product)
                   .HasForeignKey(PI => PI.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(P => P.Translations)
                   .WithOne(T => T.Product)
                   .HasForeignKey(T => T.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.ProductMedias)
                   .WithOne(pi => pi.Product)
                   .HasForeignKey(pi => pi.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
