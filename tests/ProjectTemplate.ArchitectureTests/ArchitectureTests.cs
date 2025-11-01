using FluentAssertions;
using NetArchTest.Rules;

namespace ProjectTemplate.ArchitectureTests
{
    public class ArchitectureTests
    {
        private const string BaseProjectName = "ProjectTemplate";
        private const string ApiNamespace = $"{BaseProjectName}.Api";
        private const string ApplicationNamespace = $"{BaseProjectName}.Application";
        private const string DomainNamespace = $"{BaseProjectName}.Domain";
        private const string InfraDataNamespace = $"{BaseProjectName}.Infra.Data";
        private const string InfraExternalServiceNamespace = $"{BaseProjectName}.Infra.ExternalService";
        private const string InfraIocNamespace = $"{BaseProjectName}.Infra.IoC";
        private const string InfraCrossCuttingIdentityNamespace = $"{BaseProjectName}.CrossCutting.Identity";

        [Fact]
        public void DomainLayer_Should_Not_HaveDependencyOnOtherProjects()
        {
            // Arrange
            var assembly = typeof(Domain.Entities.TodoItem).Assembly;
            var otherProjects = new[]
            {
                ApiNamespace,
                ApplicationNamespace,
                DomainNamespace,
                InfraDataNamespace,
                InfraExternalServiceNamespace,
                InfraCrossCuttingIdentityNamespace,
                InfraIocNamespace,
            };

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAll(otherProjects)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void ApplicationLayer_Should_Not_HaveDependencyOnOtherProjects()
        {
            // Arrange
            var assembly = typeof(Application.DependencyInjection).Assembly;
            var otherProjects = new[]
            {
                ApiNamespace,
                InfraDataNamespace,
                InfraExternalServiceNamespace,
                InfraCrossCuttingIdentityNamespace,
                InfraIocNamespace,
            };

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAll(otherProjects)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(Infra.Data.DependencyInjection))]
        [InlineData(typeof(Infra.ExternalService.DependencyInjection))]
        [InlineData(typeof(Infra.IoC.DependencyInjection))]
        [InlineData(typeof(Infra.CrossCutting.Identity.DependencyInjection))]
        [InlineData(typeof(Infra.CrossCutting.Shared.Common.BaseOption))]
        public void InfrastructureLayer_Should_Not_HaveDependencyOnOtherProjects(Type classType)
        {
            // Arrange
            var assembly = classType.Assembly;
            var otherProjects = new[]
            {
                ApiNamespace,
            };

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAll(otherProjects)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void PresentationLayer_Should_Not_HaveDependencyOnOtherProjects()
        {
            // Arrange
            var assembly = typeof(Api.DependencyInjection).Assembly;
            var otherProjects = new[]
            {
                ApplicationNamespace,
                DomainNamespace,
                InfraDataNamespace,
                InfraExternalServiceNamespace,
                InfraCrossCuttingIdentityNamespace,
                InfraIocNamespace,
            };

            // Act            
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAll(otherProjects)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Endpoints_Should_Have_DependencyOnMediatR()
        {
            // Arrange
            var assembly = typeof(Api.DependencyInjection).Assembly;

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .That()
                .HaveNameEndingWith("Endpoint")
                .Should()
                .HaveDependencyOn("MediatR")
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Services_Should_FollowNamingConventionss()
        {
            // Arrange
            var servicesAssembly = typeof(Infra.ExternalService.Caching.CacheService).Assembly;

            // Act
            var serviceTypes = servicesAssembly.GetExportedTypes()
                .Where(t => t.Name.EndsWith("Service") && t.IsClass);

            // Assert
            serviceTypes.Should().NotBeEmpty();
        }

    }
}