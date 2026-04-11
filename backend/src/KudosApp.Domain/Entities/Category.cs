namespace KudosApp.Domain.Entities;

public class Category
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int PointValue { get; private set; }

    private Category() { }

    public static Category Create(string name, string description, int pointValue)
    {
        return new Category
        {
            Name = name,
            Description = description,
            PointValue = pointValue
        };
    }
}
