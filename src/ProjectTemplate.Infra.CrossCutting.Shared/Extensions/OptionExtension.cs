using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProjectTemplate.Infra.CrossCutting.Shared.Common;

namespace ProjectTemplate.Infra.CrossCutting.Shared.Extensions
{
    public static class OptionExtension
    {
        public static IServiceCollection RegisterOption<T>(this IServiceCollection services)
            where T : BaseOption
        {
            T optionInstance = Activator.CreateInstance<T>();

            services.AddOptionsWithValidateOnStart<T>()
                .BindConfiguration(optionInstance.OptionKey)
                .ValidateDataAnnotations();

            return services;
        }

        public static IServiceCollection RegisterOption<T, TValidator>(this IServiceCollection services)
            where T : BaseOption
            where TValidator : class, IValidateOptions<T>
        {
            T optionInstance = Activator.CreateInstance<T>();

            services.AddOptionsWithValidateOnStart<T>()
                .BindConfiguration(optionInstance.OptionKey)
                .ValidateDataAnnotations()
                .Services.AddSingleton<IValidateOptions<T>, TValidator>();

            return services;
        }
    }
}
