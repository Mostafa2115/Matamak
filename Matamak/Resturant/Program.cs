using Core.IReprosatory;
using Core.IServices;
using Core.Models;
using Infrastructure.Context;
using Infrastructure.Reprosatory;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;

namespace Resturant
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // =========================
            // Controllers
            // =========================
            builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });

                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // =========================
            // DB Context
            // =========================
            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                );
            });

            // =========================
            // Identity
            // =========================
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();
            //=============================
            //SignalR
            //=============================
            builder.Services.AddSignalR();


            // =========================
            // JWT Authentication
            // =========================
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/orderHub"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty)
                    )
                };
            });

            // =========================
            // Repositories
            // =========================
            builder.Services.AddScoped<IItemRepo, ItemRepo>();
            builder.Services.AddScoped<ICatrgoryRepo, CategoryRepo>();
            builder.Services.AddScoped<IcountryRepo, CountryRepo>();
            builder.Services.AddScoped<IOrderItemsRepo, OrderItemsRepo>();
            builder.Services.AddScoped<IDineinOrderRepo, DineinOrderRepo>();
            builder.Services.AddScoped<IDeliveryOrderRepo, DelivaryOrderRepo>();
            builder.Services.AddScoped<ITakeAwayOrderRepo, TakeAwayOrderRepo>();
            builder.Services.AddScoped<IInventoryRepo, InventoryRepo>();
            builder.Services.AddScoped<IOfferRepo, OfferRepo>();
            builder.Services.AddScoped<IReservationRepo, ReservationRepo>();
            builder.Services.AddScoped<IReviewRepo, ReviewRepo>();
            builder.Services.AddScoped<IPaymentRepo, PaymentRepo>();
            builder.Services.AddScoped<IAccountRepo, AccountRepo>();


            // =========================
            // Services
            // =========================
            builder.Services.AddScoped<IAccountServices, AccountServices>();
            builder.Services.AddScoped<IItemService, IteamServices>();
            builder.Services.AddScoped<IOrderItemsService, OredrItemsServices>();
            builder.Services.AddScoped<IDieninOrderService, DineinOredrServices>();
            builder.Services.AddScoped<ITakeAwayOrderService, TakeAwayOredrServices>();
            builder.Services.AddScoped<IDelivaryOrderService, DeliveryOredrServices>();
            builder.Services.AddScoped<IDailyCounter, DailyCounter>();
            var paymobSettings = builder.Configuration
             .GetSection("PaymobSettings")
             .Get<PaymobSettings>() ?? new PaymobSettings();

            builder.Services.AddSingleton(paymobSettings);

            builder.Services.AddScoped<IPaymobService, PaymobService>();
            builder.Services.AddHttpClient<PaymobService>();

            // =========================
            // Swagger
            // =========================
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            SeedIdentity(app).GetAwaiter().GetResult();
            SeedMenu(app).GetAwaiter().GetResult();


            // =========================
            // Middleware
            // =========================
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<OrderHub>("/orderHub");
            app.MapControllers();

            app.Run();
        }

        private static async Task SeedIdentity(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            foreach (var role in new[] { "Admin", "Cashier", "Customer" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            await EnsureStaffUser(userManager, "admin@gmail.com", "123456789", "Admin", "System Admin", "Main Branch");
            await EnsureStaffUser(userManager, "cashier@gmail.com", "147258369", "Cashier", "Main Cashier", "Cashier Desk");
        }

        private static async Task EnsureStaffUser(
            UserManager<AppUser> userManager,
            string email,
            string password,
            string role,
            string fullName,
            string address)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    PhoneNumber = "01000000000",
                    FullName = fullName,
                    Address = address,
                    IsValid = true
                };

                var createResult = await userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to seed {role} user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                user.UserName = email;
                user.Email = email;
                user.EmailConfirmed = true;
                user.IsValid = true;
                user.FullName = string.IsNullOrWhiteSpace(user.FullName) ? fullName : user.FullName;
                user.Address = string.IsNullOrWhiteSpace(user.Address) ? address : user.Address;
                await userManager.UpdateAsync(user);

                var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await userManager.ResetPasswordAsync(user, resetToken, password);
                if (!resetResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to reset {role} password: {string.Join(", ", resetResult.Errors.Select(e => e.Description))}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                var roleResult = await userManager.AddToRoleAsync(user, role);
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to assign {role} role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
        }

        private static async Task SeedMenu(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();

            // Ensure a default country exists
            var country = await context.Countries.FirstOrDefaultAsync();
            if (country == null)
            {
                country = new Country { Name = "General" };
                context.Countries.Add(country);
                await context.SaveChangesAsync();
            }

            // Categories to add
            var categoriesData = new[] { "وجبات", "حلويات", "مشروبات" };
            var categoryMap = new System.Collections.Generic.Dictionary<string, Category>();

            foreach (var catName in categoriesData)
            {
                var category = await context.Categories.FirstOrDefaultAsync(c => c.Name == catName);
                if (category == null)
                {
                    category = new Category { Name = catName };
                    context.Categories.Add(category);
                    await context.SaveChangesAsync();
                }
                categoryMap[catName] = category;
            }

            // Items to seed
            var itemsToSeed = new System.Collections.Generic.List<Item>();

            // Meals (وجبات)
            if (!context.Items.Any(i => i.CatogryId == categoryMap["وجبات"].Id))
            {
                itemsToSeed.Add(new Item
                {
                    Name = "كباب وكفتة مشوية",
                    Description = "أسياخ كباب وكفتة مشوية على الفحم مع أرز وبطاطس وسلطات",
                    Price = 250,
                    ImageUrl = "https://images.unsplash.com/photo-1544025162-d76694265947?auto=format&fit=crop&q=80&w=600",
                    IsAvailable = true,
                    CatogryId = categoryMap["وجبات"].Id,
                    CountryId = country.id
                });
                itemsToSeed.Add(new Item
                {
                    Name = "بيتزا مارجريتا",
                    Description = "بيتزا إيطالية تقليدية بصلصة الطماطم وجبنة الموزاريلا والريحان الطازج",
                    Price = 120,
                    ImageUrl = "https://images.unsplash.com/photo-1604382354936-07c5d9983bd3?auto=format&fit=crop&q=80&w=600",
                    IsAvailable = true,
                    CatogryId = categoryMap["وجبات"].Id,
                    CountryId = country.id
                });
                itemsToSeed.Add(new Item
                {
                    Name = "شاورما عربي دجاج",
                    Description = "شاورما دجاج متبلة ومقطعة في خبز التورتيلا مع بطاطس وثومية",
                    Price = 90,
                    ImageUrl = "https://images.unsplash.com/photo-1561651823-34fed0225408?auto=format&fit=crop&q=80&w=600",
                    IsAvailable = true,
                    CatogryId = categoryMap["وجبات"].Id,
                    CountryId = country.id
                });
            }

            // Desserts (حلويات)
            if (!context.Items.Any(i => i.CatogryId == categoryMap["حلويات"].Id))
            {
                itemsToSeed.Add(new Item
                {
                    Name = "طاجن أم علي بالخلطة",
                    Description = "حلوى أم علي المصرية التقليدية بالمكسرات والحليب الطازج والقشطة",
                    Price = 65,
                    ImageUrl = "https://images.unsplash.com/photo-1579372786545-d24232daf58c?auto=format&fit=crop&q=80&w=600",
                    IsAvailable = true,
                    CatogryId = categoryMap["حلويات"].Id,
                    CountryId = country.id
                });
                itemsToSeed.Add(new Item
                {
                    Name = "مولتن كيك شكولاتة",
                    Description = "كيك شوكولاتة ساخن محشو بالشوكولاتة السائلة مع آيس كريم فانيليا",
                    Price = 80,
                    ImageUrl = "https://images.unsplash.com/photo-1606313564200-e75d5e30476c?auto=format&fit=crop&q=80&w=600",
                    IsAvailable = true,
                    CatogryId = categoryMap["حلويات"].Id,
                    CountryId = country.id
                });
                itemsToSeed.Add(new Item
                {
                    Name = "وافل نوتيلا وفراولة",
                    Description = "وافل مقرمش مغطى بشوكولاتة النوتيلا وقطع الفراولة الطازجة",
                    Price = 75,
                    ImageUrl = "https://images.unsplash.com/photo-1585502759556-91307b22a013?auto=format&fit=crop&q=80&w=600",
                    IsAvailable = true,
                    CatogryId = categoryMap["حلويات"].Id,
                    CountryId = country.id
                });
            }

            // Drinks (مشروبات)
            if (!context.Items.Any(i => i.CatogryId == categoryMap["مشروبات"].Id))
            {
                itemsToSeed.Add(new Item
                {
                    Name = "عصير مانجو فريش",
                    Description = "عصير مانجو طبيعي طازج وبارد من الفاكهة الطبيعية",
                    Price = 45,
                    ImageUrl = "https://images.unsplash.com/photo-1546173159-315724a31696?auto=format&fit=crop&q=80&w=600",
                    IsAvailable = true,
                    CatogryId = categoryMap["مشروبات"].Id,
                    CountryId = country.id
                });
                itemsToSeed.Add(new Item
                {
                    Name = "موجيتو ليمون ونعناع منعش",
                    Description = "مشروب غازي منعش بالليمون والنعناع الطازج وقطع الثلج",
                    Price = 50,
                    ImageUrl = "https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?auto=format&fit=crop&q=80&w=600",
                    IsAvailable = true,
                    CatogryId = categoryMap["مشروبات"].Id,
                    CountryId = country.id
                });
                itemsToSeed.Add(new Item
                {
                    Name = "قهوة إسبريسو مزدوجة",
                    Description = "قهوة إسبريسو غنية ومحضرة من حبوب البن الفاخرة",
                    Price = 35,
                    ImageUrl = "https://images.unsplash.com/photo-151097252790b-a638d6481a50?auto=format&fit=crop&q=80&w=600",
                    IsAvailable = true,
                    CatogryId = categoryMap["مشروبات"].Id,
                    CountryId = country.id
                });
            }

            if (itemsToSeed.Any())
            {
                context.Items.AddRange(itemsToSeed);
                await context.SaveChangesAsync();
            }
        }
    }
}


