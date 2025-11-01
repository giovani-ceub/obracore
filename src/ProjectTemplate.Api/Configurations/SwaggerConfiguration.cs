using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NSwag;

namespace ProjectTemplate.Api.Configurations
{
    public static class SwaggerConfiguration
    {
        private const string SwaggerV1Title = "Project Template API v1";

        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddFastEndpoints();
            services.SwaggerDocument(o =>
            {
                o.EnableJWTBearerAuth = true;
                o.DocumentSettings = s =>
                {
                    s.DocumentName = SwaggerV1Title;
                    s.Title = SwaggerV1Title;
                    s.Version = "v1.0";
                    s.AddAuth("ApiKey", new()
                    {
                        Description = "Put your JWT like this: Bearer {your token}",
                        Name = "Authorization",
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "JWT",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Type = OpenApiSecuritySchemeType.ApiKey,
                    });
                };
            });

            //services.AddSwaggerGen(setup =>
            //{
            //    setup.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Title = SwaggerV1Title,
            //        Version = "v1",
            //        Description = "Project Template API Swagger surface",
            //        Contact = new OpenApiContact
            //        {
            //            Name = "Your company here",
            //            Email = "contato@yourcompany.com",
            //            Url = new Uri("http://www.yourcompany.com")
            //        },
            //        License = new OpenApiLicense
            //        {
            //            Name = "MIT",
            //            Url = new Uri("https://github.com/yourgithub/TemplateProject/blob/master/LICENSE")
            //        }
            //    });

            //    setup.AddSecurityDefinition(JWTSecurityName, new OpenApiSecurityScheme
            //    {
            //        Description = "Put your JWT like this: Bearer {your token}",
            //        Name = "Authorization",
            //        Scheme = "Bearer",
            //        BearerFormat = "JWT",
            //        In = ParameterLocation.Header,
            //        Type = SecuritySchemeType.ApiKey
            //    });

            //    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            //    {
            //        {
            //            new OpenApiSecurityScheme
            //            {
            //                Reference = new OpenApiReference
            //                {
            //                    Type = ReferenceType.SecurityScheme,
            //                    Id = "Bearer"
            //                }
            //            },
            //            new string[] {}
            //        }
            //    });

            //});

            return services;
        }

        public static void UseSwaggerSetup(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            app.UseFastEndpoints()
                .UseSwaggerGen();

            //app.UseSwagger();            
            //app.UseSwaggerUI(options =>
            //{
            //    options.SwaggerEndpoint("/swagger/v1/swagger.json", SwaggerV1Title);
            //});            
        }

        //private static void AddFluentValidationSchemaProcessor(IServiceCollection services)
        //{
        //    services.AddScoped(provider =>
        //    {
        //        var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
        //        var loggerFactory = provider.GetService<ILoggerFactory>();

        //        //return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);                
        //    });
        //}
    }
}
