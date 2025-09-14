
using Eva_API.DataSeed;
using Eva_API.Mapping;
using Eva_API.Middelwares;
using Eva_BLL.Interfaces;
using Eva_BLL.Repositories;
using Eva_DAL.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace Eva_API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<ITranslateService, TranslateService>();
            builder.Services.AddScoped<IRegionService, RegionService>();
            builder.Services.AddScoped<IIngredientService, IngredientService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(M => M.AddProfile(new ProductProfile(builder.Configuration)));
            builder.Services.AddAutoMapper(M => M.AddProfile(new IngredientProfile(builder.Configuration)));
            builder.Services.AddAutoMapper(M => M.AddProfile(new RegionProfile(builder.Configuration)));
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<LanguageProfile>();
                cfg.AddProfile<CategoryProfile>();
            });


            #region Add Database Service

            builder.Services.AddDbContext<EvaDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<IdentityUser,IdentityRole>()
                    .AddEntityFrameworkStores<EvaDbContext>()
                    .AddDefaultTokenProviders();

            #endregion


            #region JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]))
                };
            });
            #endregion

            var app = builder.Build();

            app.UseStaticFiles();

            #region Update Database
            using var scope = app.Services.CreateScope();

            var service = scope.ServiceProvider;

            var context = service.GetRequiredService<EvaDbContext>();

            var loggerFactory = service.GetRequiredService<ILoggerFactory>();

            try
            {
                await context.Database.MigrateAsync();
                await IdentitySeed.SeedAsync(service);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "There Are Problems During Apply Migrations !!");
            }

            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseMiddleware<TokenValidationMiddelware>();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
