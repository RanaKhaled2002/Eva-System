using Eva_DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.Data
{
    public class EvaDbContext :IdentityDbContext<IdentityUser>
    {
        public EvaDbContext(DbContextOptions<EvaDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }

        public DbSet<RevokedToken> RevokedTokens { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryTranslation> CategoryTranslations { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientTranslation> IngredientTranslations { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductIngredient> ProductIngredients { get; set; }
        public DbSet<ProductTranslation> ProductTranslations { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<RegionTranslation> RegionTranslations { get; set; }
        public DbSet<ProductMedia> ProductMedias { get; set; }
        public DbSet<IngredientImage> IngredientImages { get; set; }
        public DbSet<RegionCulturalImage> RegionCulturalImages { get; set; }
        public DbSet<RegionHistoricalImage> RegionHistoricalImages { get; set; }
    }
}
