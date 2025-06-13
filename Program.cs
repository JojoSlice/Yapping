using Microsoft.AspNetCore.Antiforgery;

namespace miniReddit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddHttpClient<APIManager.UserManager>();
            builder.Services.AddHttpClient<APIManager.PostManager>();
            builder.Services.AddHttpClient<APIManager.CommentManager>();
            builder.Services.AddHttpClient<APIManager.CategoryManager>();
            builder.Services.AddHttpClient<APIManager.LikeManager>();
            builder.Services.AddHttpClient<APIManager.MessageManager>();
            builder.Services.AddHttpClient<APIManager.ReportManager>();
            builder.Services.AddHttpClient<APIManager.ChatManager>();
            builder.Services.AddScoped<APIManager.UserManager>();
            builder.Services.AddScoped<APIManager.PostManager>();
            builder.Services.AddScoped<APIManager.CommentManager>();
            builder.Services.AddScoped<APIManager.CategoryManager>();
            builder.Services.AddScoped<APIManager.LikeManager>();
            builder.Services.AddScoped<APIManager.MessageManager>();
            builder.Services.AddScoped<APIManager.ReportManager>();
            builder.Services.AddScoped<APIManager.ChatManager>();
            builder.Services.AddScoped<Services.ImgUpload>();
            builder.Services.AddScoped<Services.AuthenticationService>();

            builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
            
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAuthentication("MyCookieAuth")
                .AddCookie("MyCookieAuth", options =>
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.Name = "MyCookieAuth";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(90);
                });

            builder.Services.AddAuthorization();

            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });

            var app = builder.Build();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGet("antiforgery/token", (IAntiforgery forgeryService, HttpContext context) =>
            {
                var tokens = forgeryService.GetAndStoreTokens(context);
                context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
                new CookieOptions { HttpOnly = false });

                return Results.Ok();
            }).RequireAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.MapControllers();

            app.Run();
        }
    }
}
