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
    public class ProductIngredientConfiguration : IEntityTypeConfiguration<ProductIngredient>
    {
        public void Configure(EntityTypeBuilder<ProductIngredient> builder)
        {
            builder.HasKey(pi => new { pi.ProductId, pi.IngredientId });

            builder.HasOne(pi => pi.Product)
                   .WithMany(p => p.ProductIngredients)
                   .HasForeignKey(pi => pi.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pi => pi.Ingredient)
                   .WithMany(i => i.ProductIngredients)
                   .HasForeignKey(pi => pi.IngredientId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
