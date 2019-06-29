using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Kaffee.Settings;
using Kaffee.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using Kaffee.Models.Http;
using Kaffee.Mappers;

namespace Kaffee
{
    /// <summary>
    /// Startup configs.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Add system configuration.
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            var token = Configuration.GetSection("KaffeeDatabaseSettings").Get<KaffeeDatabaseSettings>().ConnectionString;
            var key = Configuration.GetSection("SecurityKey").Get<string>();
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(key))
            {
                throw new Exception("No connection string or security key given");
            }

            services.Configure<KaffeeDatabaseSettings>(
                Configuration.GetSection(nameof(KaffeeDatabaseSettings)));
            services.AddSingleton<IKaffeeDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<KaffeeDatabaseSettings>>().Value);

            services.AddSingleton<IHttpClient, HttpClientImplementation>();
            services.AddSingleton<IHttpResponseMapper, HttpResponseMapper>();

            // Configure weather service
            switch (Configuration.GetSection("WeatherService").GetSection("Service").Get<string>())
            {
                case "DarkSky":
                    services.Configure<DarkSkySettings>(
                        Configuration.GetSection("WeatherService")
                            .GetSection(nameof(DarkSkySettings))
                    );
                    services.AddSingleton<DarkSkySettings>(sp =>
                        sp.GetRequiredService<IOptions<DarkSkySettings>>().Value);
                    services.AddSingleton<IWeatherService, DarkSkyWeatherService>();
                    break;
            }

            services.AddSingleton<CoffeeService>();
            services.AddSingleton<UserService>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "kaffee.dwelsh.uk",
                        ValidAudience = "kaffee.dwelsh.uk",
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["SecurityKey"]))
                    };
                });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Kaffee API",
                    Description = "API for the Kaffee Logging Service",
                    Contact = new OpenApiContact
                    {
                        Name = "Daniel Welsh",
                        Email = "e@dwelsh.uk",
                        Url = new Uri("https://dwelsh.uk"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under MIT",
                        Url = new Uri("https://github.com/wel-shy/kaffee-api/blob/master/LICENSE"),
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.DescribeAllEnumsAsStrings();
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("CorsPolicy");
            app.UseAuthentication();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc();
        }
    }
}
