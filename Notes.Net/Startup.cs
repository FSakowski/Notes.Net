using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notes.Net.Infrastructure;
using Notes.Net.Models;
using Notes.Net.Service;

namespace Notes.Net
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Tenant>(new Tenant() { TenantId = 1, Name = "Default" });
            services.AddTransient<IServiceContext, DefaultServiceContext>();
            services.AddSingleton<INoteService, NoteService>();
            services.AddSingleton<IRepository, FakeRepository>();

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
        }
    }
}
