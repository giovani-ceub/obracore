namespace ProjectTemplate.Application.UnitTests.Common.Behaviors
{
    public class Request
    {
        public Request(string id, string description)
        {
            Id = id;
            Description = description;
        }

        public string? Id { get; set; }
        public string? Description { get; set; }
    }
}
