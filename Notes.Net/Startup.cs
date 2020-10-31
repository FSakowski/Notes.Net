using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Notes.Net.Infrastructure;
using Notes.Net.Models;
using Notes.Net.Service;
using System;

namespace Notes.Net
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<NotesDbContext>(options => options.UseNpgsql(Configuration["Data:Notes:ConnectionString"]));

            services.AddTransient<IServiceContext, DefaultServiceContext>();
            services.AddTransient<IRepository, EFNotesRepository>();
            services.AddTransient<INoteService, NoteService>();
            // services.AddSingleton<IRepository, FakeRepository>();

            services.AddTransient<IUserStore<User>, UserStore>();

            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedAccount = false;

            })
                .AddSignInManager()
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
                .AddCookie(IdentityConstants.ApplicationScheme, o =>
                {
                    o.LoginPath = new PathString("/Account/Login");
                    o.Events = new CookieAuthenticationEvents
                    {
                        OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
                    };
                })
                .AddCookie(IdentityConstants.ExternalScheme, o =>
                {
                    o.Cookie.Name = IdentityConstants.ExternalScheme;
                    o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                });

            services.AddMvc();
            services.AddMemoryCache();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCSP(
                "default-src 'self' *.fontawesome.com;" + 
                "img-src * data:;" + 
                "media-src *;" + 
                "style-src 'unsafe-inline' 'self'"
                );

            app.UseRouting();
            app.UseStaticFiles();
            app.UseStatusCodePages();
            app.UseSession();
            app.UseRequestLocalization();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            SeedData.EnsurePopulated(app);
        }
    }
}
