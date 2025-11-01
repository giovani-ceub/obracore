using Microsoft.Extensions.Options;

namespace ProjectTemplate.Infra.ExternalService.Apic.Authentication.Settings
{
    internal class ApicAuthenticationSettingsValidator : ValidateOptionsBase, IValidateOptions<ApicAuthenticationSettings>
    {
        private ValidateOptionsResultBuilder _validationBuilder;

        public ApicAuthenticationSettingsValidator()
        {
            _validationBuilder = new ValidateOptionsResultBuilder();
        }

        public ValidateOptionsResult Validate(string? name, ApicAuthenticationSettings options)
        {
            ValidateClientSecret(options);
            ValidateClientId(options);

            return _validationBuilder.Build();
        }

        private void ValidateClientSecret(ApicAuthenticationSettings options)
        {
            if (string.IsNullOrEmpty(options.ClientSecret) || options.ClientSecret == Guid.Empty.ToString())
            {
                _validationBuilder.AddError(Required_Message, nameof(options.ClientSecret));
            }
        }

        private void ValidateClientId(ApicAuthenticationSettings options)
        {
            if (string.IsNullOrEmpty(options.ClientId) || options.ClientId == Guid.Empty.ToString())
            {
                _validationBuilder.AddError(Required_Message, nameof(options.ClientId));
            }
        }
    }
}
