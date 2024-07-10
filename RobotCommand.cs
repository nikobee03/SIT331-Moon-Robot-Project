namespace robot_controller_api;

public class RobotCommand
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsMoveCommand { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public RobotCommand(int id, string name, bool isMoveCommand, DateTime createdDate, DateTime modifiedDate, string? description = null)
    {
        Id = id;
        Name = name;
        Description = description;
        IsMoveCommand = isMoveCommand;
        CreatedDate = createdDate;
        ModifiedDate = modifiedDate;
    }
}
