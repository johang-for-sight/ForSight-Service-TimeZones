using System;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using ForSight.Audit;
using ForSight.Authorization;
using ForSight.TimeZonesService.API.Dispatcher;
using ForSight.TimeZonesService.API.Extensions;
using ForSight.TimeZonesService.Data;
using ForSight.TimeZonesService.Data.DbSeeding;
using ForSight.Messaging.Extensions;
using ForSight.Utils.Extensions;
using IdentityModel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ForSight.TimeZonesService.API
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
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    // IMPORTANT! Set MapInboundClaims to false in order to keep the default claims schema.
                    // When true, this will map to the MS default scheme. The sub claim is mapped to ClaimTypes.NameIdentifier instead of 'sub' claim.
                    // When false instead of checking for IsInRole, we need to check roles as any other claim - Use HasRole extension method on Forth.Authorization package.
                    options.MapInboundClaims = false;
                    options.IncludeErrorDetails = true; // Good for debugging errors

                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.Authority = Configuration["OpenIdConnectConfig:Authority"];
                    options.Audience = Configuration["OpenIdConnectConfig:Audience"];

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        RoleClaimType = JwtClaimTypes.Role,
                        NameClaimType = JwtClaimTypes.Name,
                        ValidateAudience = true,
                        ValidAudience = "campaigns",
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["OpenIdConnectConfig:Authority"]
                    };

                    // hook on the OnMessageReceived event to allow the JWT authentication handler to read the access token from the query string
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorizationWithPolicies();

            // Add HttpClient to request/use token.
            services.AddHttpClient();

            var defaultConnection = Configuration.GetConnectionString("DefaultConnection");
            services.AddControllers();
            services.AddDbContext<TimeZonesServiceDbContext>(options =>
                options.UseSqlServer(defaultConnection, x => x.MigrationsAssembly("ForSight.TimeZonesService.Data")));

            services.AddSwaggerGen(c =>
            {
                var openApiInfo = new OpenApiInfo
                {
                    Title = "ForSight.TimeZonesService.API",
                    Version = "v1"
                };
                c.SwaggerDoc("v1", openApiInfo);
            });

            // TODO - https://fb-jira.atlassian.net/browse/FSV4-4674
            services.AddForSightMessaging<TimeZonesServiceDbContext>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ITimeZonesServiceDbContext>(provider => provider.GetService<TimeZonesServiceDbContext>() ?? throw new ApplicationException("Could not resolve a TimeZonesServiceDbContext in the container."));

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddHttpContextAccessor();            
            services.AddApplicationInsights();

            services.AddScopedForSightAuthorizationService();
            services.AddScopedForSightCurrentUserContext();
            services.RegisterHandlers();
            services.RegisterServices();
            services.ConfigureAppSettings(Configuration);

            // see https://riptutorial.com/asp-net-core/example/18611/rate-limiting-based-on-client-ip
            // "EnableEndpointRateLimiting": true && "Period": "1m","Limit": 100 is setting a limit of 100 requests x second for each api
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimitOptions"));
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TelemetryConfiguration telemetryConfiguration)
        {
            if (env.IsDevelopment())
            {
                InitializeDatabase(app).GetAwaiter().GetResult();
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ForSight.TimeZonesService.API v1"));

                telemetryConfiguration.DisableTelemetry = true;
            }
            else
            {
                app.UseForSightExceptionHandlerMiddleware();
            }
            

            app.UseIpRateLimiting();

            app.UseHttpsRedirection();

            app.UseForSightMessaging();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization(); // Allows [Authorize] attribute on controllers/endpoints

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private static async Task InitializeDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var timeZonesDbContext = serviceScope.ServiceProvider.GetRequiredService<TimeZonesServiceDbContext>();

            await timeZonesDbContext.Database.MigrateAsync();

            if (!await timeZonesDbContext.ActiveTimeZones.AnyAsync())
            {
                var activeTimeZones = ConfigurationDbSeeding.GetActiveTimeZones();
                await timeZonesDbContext.ActiveTimeZones.AddRangeAsync(activeTimeZones);
                await timeZonesDbContext.SaveChangesAsync();
            }
        
        }

    }
}
