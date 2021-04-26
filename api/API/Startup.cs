using API.Configuration;
using API.Configuration.Authorization;
using Library;
using Library.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Services;
using System.Collections.Generic;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("corsPolicy",
                builder =>
                {
                    builder.WithOrigins(GetAllowedOrigins());
                });
            });

            services.AddHttpContextAccessor();
            services.AddDataProtection();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CustomAuthConstants.SCHEME_NAME;
                options.DefaultChallengeScheme = CustomAuthConstants.SCHEME_NAME;
            })
            .AddScheme<CustomOptionsAuth, CustomAuthHandler>(CustomAuthConstants.SCHEME_NAME, o => o.DisplayMessage = $"Custom scheme: {CustomAuthConstants.SCHEME_NAME}");

            services.AddSingleton<IAuthorizationPolicyProvider, PermissionsPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            services.Configure<AppConfigData>(Configuration.GetSection("ConfigData"));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                c.OperationFilter<SwaggerSecurityRequirementsOperationFilter>();
            });

            services.AddScoped<SecurityService>();
            services.AddScoped<SecurityContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private string[] GetAllowedOrigins()
        {
            var allowedOriginsArray = new List<string>();
            var allowedOrigins = Configuration.GetSection("ConfigData:AllowedOrigins").Value;

            if (!string.IsNullOrWhiteSpace(allowedOrigins))
                allowedOriginsArray = new List<string>(allowedOrigins.Split(";"));

            return allowedOriginsArray.ToArray();
        }
    }
}
