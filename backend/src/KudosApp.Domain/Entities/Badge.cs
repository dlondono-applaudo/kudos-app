namespace KudosApp.Domain.Entities;

public class Badge
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Icon { get; private set; } = string.Empty;
    public int RequiredPoints { get; private set; }

    private Badge() { }

    public static Badge Create(string name, string description, string icon, int requiredPoints)
    {
        return new Badge
        {
            Name = name,
            Description = description,
            Icon = icon,
            RequiredPoints = requiredPoints
        };
    }
}
