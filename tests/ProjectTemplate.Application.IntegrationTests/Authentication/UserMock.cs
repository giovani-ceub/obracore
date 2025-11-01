using ProjectTemplate.Application.Common.Interfaces;

namespace ProjectTemplate.Application.IntegrationTests.Authentication
{
    public record UserMock : IUser
    {
        public UserMock(string id, string name, string userName, string password, IEnumerable<string> roles)
        {
            Id = id;
            Name = name;
            Password = password;
            Roles = roles;
        }

        public string? Id { get; set; }
        public string Name { get; init; }
        public string Password { get; init; }
        public IEnumerable<string> Roles { get; init; }
    }
}
