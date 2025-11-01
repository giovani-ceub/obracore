using Microsoft.Extensions.Options;

namespace ProjectTemplate.Infra.ExternalService.AzureKeyVault.Settings
{
    internal class AzureKeyVaultSettingsValidator : ValidateOptionsBase, IValidateOptions<AzureKeyVaultSettings>
    {
        private ValidateOptionsResultBuilder _validationBuilder;

        public AzureKeyVaultSettingsValidator()
        {
            _validationBuilder = new ValidateOptionsResultBuilder();
        }

        ValidateOptionsResult IValidateOptions<AzureKeyVaultSettings>.Validate(string? name, AzureKeyVaultSettings options)
        {
            ValidateDomain(options);
            ValidateClient(options);

            return _validationBuilder.Build();
        }

        private void ValidateDomain(AzureKeyVaultSettings options)
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

        private void ValidateDomainPattern(AzureKeyVaultSettings options)
        {
            string domainPattern = "onmicrosoft.com";

            if (!options.Domain.EndsWith(domainPattern))
            {
                _validationBuilder.AddError(IsInvalid_Message, nameof(options.Domain));
            }
        }

        private void ValidateClient(AzureKeyVaultSettings options)
        {
            if (options.ClientId == Guid.Empty)
            {
                _validationBuilder.AddError(Required_Message, nameof(options.ClientId));
            }
        }
    }
}
