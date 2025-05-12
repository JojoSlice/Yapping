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
            builder.Services.AddScoped<APIManager.UserManager>();
            builder.Services.AddScoped<APIManager.PostManager>();
            builder.Services.AddScoped<APIManager.CommentManager>();
            builder.Services.AddScoped<APIManager.CategoryManager>();
            builder.Services.AddScoped<Services.AuthenticationService>();
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

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.MapControllers();

            app.Run();
        }
    }
}
