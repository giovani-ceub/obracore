using Microsoft.Extensions.Options;

namespace ProjectTemplate.Infra.ExternalService.AzureAD.Settings
{
    internal class AzureADSettingsValidator : ValidateOptionsBase, IValidateOptions<AzureADSettings>
    {
        private ValidateOptionsResultBuilder _validationBuilder;

        public AzureADSettingsValidator()
        {
            _validationBuilder = new ValidateOptionsResultBuilder();
        }

        public ValidateOptionsResult Validate(string? name, AzureADSettings options)
        {
            ValidateDomain(options);
            ValidateClient(options);

            return _validationBuilder.Build();
        }

        private void ValidateDomain(AzureADSettings options)
        {
            if (string.IsNullOrEmpty(options.Domain))
            {
                _validationBuilder.AddError(Required_Message, nameof(options.Domain));
            }
            else
            {
                ValidateDomainPattern(options);
            }
        }

        private void ValidateDomainPattern(AzureADSettings options)
        {
            string domainPattern = "onmicrosoft.com";

            if (!options.Domain.EndsWith(domainPattern))
            {
                _validationBuilder.AddError(IsInvalid_Message, nameof(options.Domain));
            }
        }

        private void ValidateClient(AzureADSettings options)
        {
            if (options.ClientId == Guid.Empty)
            {
                _validationBuilder.AddError(Required_Message, nameof(options.ClientId));
            }
        }
    }
}
