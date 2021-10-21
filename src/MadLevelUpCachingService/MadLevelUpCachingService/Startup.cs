using MadLevelUpCachingService.API.HostedService;
using MadLevelUpCachingService.API.ScheduledService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

namespace MadLevelUpCachingService
{
    public class Startup
    {
        public IHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(this.Configuration, "AzureAd")
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddMicrosoftGraph(this.Configuration.GetSection("GraphApi"))
                .AddInMemoryTokenCaches();

            services.AddLogging();

            services.AddCors(options =>
            {
                var origins = Environment.IsDevelopment() ?
                    new[] { "http://localhost:3001", "https://localhost:3001" } :
                    new[] { "https://learning.mad.itera.no" };

                options.AddDefaultPolicy(
                    builder => builder
                        .WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            services.AddHostedService<Worker>();

            services.AddControllers();
            services.AddSwaggerGen(static c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MadLevelUpCachingService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!string.IsNullOrWhiteSpace(this.Configuration["PathBase"]))
                app.UsePathBase(this.Configuration["PathBase"]);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(static c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MadLearning.API v1"));
            }
            else
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.All,
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
            });
        }
    }
}
