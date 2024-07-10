namespace robot_controller_api;

public class Map
{
    public int Id { get; set; }
    public int Columns { get; set; }
    public int Rows { get; set; }
    public bool IsSquare { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public Map(int id, string name, int columns, int rows, bool issquare, DateTime createdDate, DateTime modifiedDate, string? description = null)
    {
        Id = id;
        Name = name;
        Columns = columns;
        Rows = rows;
        IsSquare = issquare;
        Description = description;
        CreatedDate = createdDate;
        ModifiedDate = modifiedDate;
    }
}
