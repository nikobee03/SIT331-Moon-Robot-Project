using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Authentication;
using robot_controller_api.Persistance;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController : ControllerBase
{
    private static readonly List<RobotCommand> _commands = new List<RobotCommand>
    {
        new RobotCommand(
            id: 1,
            name: "LEFT", 
            isMoveCommand: true,
            createdDate: new DateTime(2024, 4, 3),
            modifiedDate: new DateTime(2024, 4, 3),
            description: "Turns the robot left."
            ),
        new RobotCommand(
            id: 2,
            name: "RIGHT",
            isMoveCommand: true,
            createdDate: new DateTime(2024, 4, 3),
            modifiedDate: new DateTime(2024, 4, 3),
            description: "Turns the robot right."
            ),
        new RobotCommand(
            id: 3,
            name: "MOVE",
            isMoveCommand: true,
            createdDate: new DateTime(2024, 4, 3),
            modifiedDate: new DateTime(2024, 4, 3),
            description: "Moves the robot forward."
            ),
        new RobotCommand(
            id: 4,
            name: "PLACE", 
            isMoveCommand: false,
            createdDate: new DateTime(2024, 4, 3),
            modifiedDate: new DateTime(2024, 4, 3),
            description: "Places the robot on the map."
            ),
        new RobotCommand(
            id: 5,
            name: "REPORT",
            isMoveCommand: false,
            createdDate: new DateTime(2024, 4, 3),
            modifiedDate: new DateTime(2024, 4, 3),
            description: "Reports the robot's position."
            )
    };

    // Robot commands endpoints here

    // GET: api/robot-commands
    /// <summary>
    /// Retrieves all robot commands
    /// </summary>
    /// <returns> All existing robot commands </returns>
    /// <response code="200">Successfully returns all robot commands</response>
    /// <response code="204">If there are no commands available</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpGet(), Authorize(Policy = "UserOnly")]
    public IEnumerable<RobotCommand> GetAllRobotCommands()
    {
        return RobotCommandDataAccess.GetRobotCommands();
    }

    // GET: api/robot-commands/move
    /// <summary>
    /// Retrieves only move commands
    /// </summary>
    /// <returns> All existing robot commands </returns>
    /// <response code="200">Successfully returns all robot commands</response>
    /// <response code="204">If there are no move commands available</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpGet("move"), Authorize(Policy = "UserOnly")]
    public IEnumerable<RobotCommand> GetMoveCommandsOnly()
    {
        return RobotCommandDataAccess.GetRobotCommands().Where(x => x.IsMoveCommand == true);
    }

    // GET: api/robot-commands/{id}
    /// <summary>
    /// Retrieves commands based on ID
    /// </summary>
    /// <param name="id">The ID for the desired command.</param>
    /// <returns> The command with the requested ID</returns>
    /// <response code="200">Successfully returns requested command</response>
    /// <response code="404">If the provided ID is out of range</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}", Name = "GetRobotCommand"), Authorize(Policy = "UserOnly")]
    public IActionResult GetRobotCommandById(int id)
    {
        // Check if the provided id is within the range of available commands
        if (id >= 0 && id < RobotCommandDataAccess.GetRobotCommands().Count)
        {
            // Retrieve the command from the _commands collection based on the provided id
            var command = RobotCommandDataAccess.GetRobotCommands().FirstOrDefault(m => m.Id == id);

            // Return a 200 OK response with the command object in the body
            return Ok(command);
        }
        else
        {
            // If the provided id is out of range, return a 404 Not Found response
            return NotFound();
        }
    }

    // POST: api/robot-commands
    /// <summary>
    /// Adds a new command
    /// </summary>
    /// <param name="newCommand">>A new robot command from the HTTP request.</param>
    /// <returns> The newly created command </returns>
    /// <response code="201">Returns the newly created robot command</response>
    /// <response code="400">If the robot command is null</response>
    /// <response code="409">If a robot command with the same name already exists</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost(), Authorize(Policy = "AdminOnly")]
    public IActionResult AddRobotCommand(RobotCommand newCommand)
    {
        // Check if newCommand is null
        if (newCommand == null)
        {
            return BadRequest();
        }

        // Check if a command with the same name already exists
        if (RobotCommandDataAccess.GetRobotCommands().Any(c => c.Name == newCommand.Name))
        {
            return Conflict();
        }

        // Insert the new command into the database
        RobotCommandDataAccess.InsertRobotCommand(newCommand);

        // Get the base URL of the API
        var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";

        // Construct the URL for the newly created resource
        var locationUrl = $"{baseUrl}/api/robot-commands/{newCommand.Id}";

        // Return a CreatedAtRoute response with the newly created command and the Location header
        return CreatedAtRoute("GetRobotCommand", new { id = newCommand.Id }, newCommand);
    }

    // PUT: api/robot-commands/{id}
    /// <summary>
    /// Updates an existing command
    /// </summary>
    /// <param name="id">The ID of the command being modified.</param>
    /// <param name="command">The updated version of the command</param>
    /// <returns> No content if the update is successful </returns>
    /// <response code="204">No content indicates a successful update</response>
    /// <response code="400">If the robot command is null</response>
    /// <response code="404">If the robot command with the specified ID is not found.</response>
    /// <response code="409">If a robot command with the same name already exists</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateRobotCommand(int id, RobotCommand command)
    {
        if (id <= 0)
        {
            return BadRequest();
        }
        if (command == null)
        {
            return BadRequest();
        }

        RobotCommandDataAccess.UpdateRobotCommand(id, command);
        return NoContent();
    }

    // DELETE: api/robot-commands/{id}
    /// <summary>
    /// Deletes a command
    /// </summary>
    /// <param name="id">The ID of the command being deleted.</param>
    /// <returns> No content if the deletion is successful </returns>
    /// <response code="204">No content indicates a successful update</response>
    /// <response code="404">If the robot command with the specified ID is not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult DeleteRobotCommand(int id)
    {
        // Find the command by id
        var commandToRemove = RobotCommandDataAccess.GetRobotCommands().FirstOrDefault(c => c.Id == id);

        // If the command doesn't exist, return NotFound
        if (commandToRemove == null)
        {
            return NotFound();
        }

        // Remove the command from commands
        RobotCommandDataAccess.DeleteRobotCommand(id);

        // Return NoContent to indicate successful deletion
        return NoContent();
    }


}
