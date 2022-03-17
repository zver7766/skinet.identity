using System;
using System.Linq;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skinet.Identity.Domain.Interfaces;
using skinet.identity.Extensions;
using Skinet.Identity.Infrastructure.Identity;
using Skinet.Identity.Infrastructure.Services;

namespace skinet.identity
{
    public class Startup
    {
        private readonly IConfiguration _config;
        
        public Startup(IConfiguration config)
        {
            _config = config;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var connectionString = _config.GetConnectionString("IdentityConnection");
            
            services.AddDbContext<AppIdentityDbContext>(x =>
            {
                x.UseSqlServer(connectionString);
            });
            
            services.AddScoped<ITokenService, TokenService>();

            services.AddIdentityServices(_config);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Skinet.Identity.WebApi", Version = "v1"});
            });
            
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://skinet.api:5001");
                });
            });

            services.AddHealthChecks()
                .AddSqlServer(connectionString, failureStatus: HealthStatus.Unhealthy, tags: new [] { "ready" });

            // https://skinet.identity:5002/healthchecks-ui
            services.AddHealthChecksUI(s =>
                {
                    s.AddHealthCheckEndpoint("Skinet Identity", "https://skinet.identity:5002/healthui");
                    s.SetMinimumSecondsBetweenFailureNotifications(30);
                    s.SetEvaluationTimeInSeconds(60);
                })
                .AddSqliteStorage("Data Source = healthchecks.db");
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Skinet.Identity.WebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    },
                    ResponseWriter = WriteHealthCheckReadyResponse,
                    Predicate = (check) => check.Tags.Contains("ready"),
                    AllowCachingResponses = false
                });

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    Predicate = (check) => !check.Tags.Contains("ready"),
                    ResponseWriter = WriteHealthCheckLiveResponse,
                    AllowCachingResponses = false
                });

                endpoints.MapHealthChecks("/healthui", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });

            app.UseHealthChecksUI();
        }

        private Task WriteHealthCheckReadyResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";

            var json = new JObject(
                new JProperty("OverallStatus", result.Status.ToString()),
                new JProperty("TotalCheckDuration", result.TotalDuration.TotalSeconds.ToString("0:0.00")),
                new JProperty("DependencyHealthCheck", new JObject(result.Entries.Select(dicItem =>
                    new JProperty(dicItem.Key, new JObject(
                        new JProperty("Status", dicItem.Value.Status.ToString()),
                        new JProperty("Duration", dicItem.Value.Duration.TotalSeconds.ToString("0:0.00"))
                        ))
                )))
            );

            return httpContext.Response.WriteAsync(json.ToString(Formatting.Indented));
        }
        
        private Task WriteHealthCheckLiveResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";

            var json = new JObject(
                new JProperty("OverallStatus", result.Status.ToString()),
                new JProperty("TotalCheckDuration", result.TotalDuration.TotalSeconds.ToString("0:0.00"))
            );

            return httpContext.Response.WriteAsync(json.ToString(Formatting.Indented));
        }
    }
}