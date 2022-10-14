﻿using Microsoft.Extensions.DependencyInjection;
using ScoreboardApp.Api.Filters;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace ScoreboardApp.Api
{
    public static class ServicesConfiguration
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<ApiExceptionFilterAttribute>();
                options.UseDateOnlyTimeOnlyStringConverters();
            })
            .AddJsonOptions(options => options.UseDateOnlyTimeOnlyStringConverters());

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = ApiVersion.Default;
                options.ApiVersionReader = new HeaderApiVersionReader("api-version");
                options.ReportApiVersions = true;
            });

            return services;
        }
    }
}
