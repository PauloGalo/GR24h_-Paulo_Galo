using System.Globalization;
using GR24h_Repos;
using GR24h_Services;

namespace GR24h_Controller
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            builder.Services.AddScoped<UsersRepository>();
            builder.Services.AddScoped<RecipeRepository>();
            builder.Services.AddScoped<CategoryRepository>();
            builder.Services.AddScoped<DifficultyRepository>();
            builder.Services.AddScoped<UserFavouritesRepository>();
            builder.Services.AddScoped<CommentRepository>();
            builder.Services.AddScoped<RatingRepository>();
            builder.Services.AddScoped<RecipeIngredientRepository>();
            builder.Services.AddScoped<IngredientRepository>();


            builder.Services.AddScoped<UsersService>();
            builder.Services.AddScoped<RecipeService>();
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<DifficultyService>();
            builder.Services.AddScoped<UserFavouritesService>();
            builder.Services.AddScoped<CommentService>();
            builder.Services.AddScoped<RatingService>();
            builder.Services.AddScoped<RecipeIngredientService>();
            builder.Services.AddScoped<IngredientService>();

            builder.Services.AddScoped<FileService>(sp =>
            {
                var env = sp.GetRequiredService<IWebHostEnvironment>();
                return new FileService(env.WebRootPath);
            });

            var app = builder.Build();

            var supportedCultures = new[] { new CultureInfo("pt-PT") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pt-PT"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");

               
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

           
            app.UseStatusCodePagesWithReExecute("/PageNotFound", "?code={0}");

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
