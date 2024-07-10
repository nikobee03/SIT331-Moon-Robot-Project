using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Authentication;
using robot_controller_api.Persistance;

namespace robot_controller_api.Controllers
{
    [ApiController]
    [Route("api/maps")]
    public class MapsController : ControllerBase
    {
        private static readonly List<Map> _maps = new List<Map>
        {
             new Map(
                id: 0,
                columns: 10,
                rows: 10,
                issquare: true,
                name: "Default Map",
                createdDate: new DateTime(2024, 4, 3),
                modifiedDate: new DateTime(2024, 4, 3),
                description: "Default 10x10 map."
                ),
             new Map(
                 id: 1,
                 columns: 20,
                 rows: 10,
                 issquare: false,
                 name: "Non-square Map",
                 createdDate: new DateTime(2024, 4, 3),
                 modifiedDate: new DateTime(2024, 4, 3),
                 description: "Non-square map"
             )
        };

        // GET: api/maps
        /// <summary>
        /// Retrieves all maps.
        /// </summary>
        /// <returns>All existing maps.</returns>
        /// <response code="200">Successfully returns all maps</response>
        /// <response code="204">If there are no maps available</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet, Authorize(Policy = "UserOnly")]
        public IEnumerable<Map> GetAllMaps()
        {
            return MapDataAccess.GetMaps();
        }

        // GET: api/maps/square
        /// <summary>
        /// Retrieves only square maps.
        /// </summary>
        /// <returns>All existing square maps.</returns>
        /// <response code="200">Successfully returns all square maps</response>
        /// <response code="204">If there are no square maps available</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("square"), Authorize(Policy = "UserOnly")]
        public IEnumerable<Map> GetSquareMapsOnly()
        {
            return _maps.Where(m => m.Columns == m.Rows);
        }

        // GET: api/maps/{id}
        /// <summary>
        /// Retrieves a map by its ID.
        /// </summary>
        /// <param name="id">The ID of the desired map.</param>
        /// <returns>The map with the specified ID.</returns>
        /// <response code="200">Successfully returns requested map</response>
        /// <response code="404">If the provided ID is out of range</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetMap"), Authorize(Policy = "UserOnly")]
        public IActionResult GetMapById(int id)
        {
            var map = MapDataAccess.GetMapById(id);
            if (map != null)
            {
                return Ok(map);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: api/maps
        /// <summary>
        /// Adds a new map.
        /// </summary>
        /// <param name="newMap">The map to be added.</param>
        /// <returns>The newly created map.</returns>
        /// <response code="201">Returns the newly created map</response>
        /// <response code="400">If the map is null</response>
        /// <response code="409">If a map with the same name already exists</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost, Authorize(Policy = "AdminOnly")]
        public IActionResult AddMap(Map newMap)
        {
            if (newMap == null)
            {
                return BadRequest();
            }

            if (_maps.Any(c => c.Name == newMap.Name))
            {
                return Conflict();
            }

            MapDataAccess.InsertMap(newMap);

            // Get the base URL of the API
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";

            // Construct the URL for the newly created resource
            var locationUrl = $"{baseUrl}/api/maps/{newMap.Id}";

            // Return a CreatedAtRoute response with the newly created map and the Location header
            return CreatedAtRoute("GetMap", new { id = newMap.Id }, newMap);
        }

        // PUT: api/maps/{id}
        /// <summary>
        /// Updates an existing map.
        /// </summary>
        /// <param name="id">The ID of the map to be updated.</param>
        /// <param name="map">The updated version of the map.</param>
        /// <returns>No content if the update is successful.</returns>
        /// <response code="204">No content indicates a successful update</response>
        /// <response code="400">If the map is null</response>
        /// <response code="404">If the map with the specified ID is not found.</response>
        /// <response code="409">If a map with the same name already exists</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut("{id}"), Authorize(Policy = "AdminOnly")]
        public IActionResult UpdateMap(int id, Map map)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            if (map == null)
            {
                return BadRequest();
            }

            MapDataAccess.UpdateMap(id, map);
            return NoContent();
        }

        // DELETE: api/maps/{id}
        /// <summary>
        /// Deletes a map.
        /// </summary>
        /// <param name="id">The ID of the map to be deleted.</param>
        /// <returns>No content if the deletion is successful.</returns>
        /// <response code="204">No content indicates a successful update</response>
        /// <response code="404">If the map with the specified ID is not found.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}"), Authorize(Policy = "AdminOnly")]
        public IActionResult DeleteMap(int id)
        {
            var mapToRemove = MapDataAccess.GetMapById(id);

            if (mapToRemove == null)
            {
                return NotFound();
            }

           MapDataAccess.DeleteMap(id);

            return NoContent();
        }

        // GET: api/maps/{id}/{x}-{y}
        /// <summary>
        /// Checks if a set of coordinates are on a specific map.
        /// </summary>
        /// <param name="id">The ID of the map.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>True if the coordinates are within the map's boundaries; otherwise, false.</returns>
        /// <response code="200">Successfully returns requested point on map</response>
        /// <response code="400">If the provided coordinates are invalid.</response>
        /// <response code="404">If the provided map ID or coordinates are out of range.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}/{x}-{y}"), Authorize(Policy = "UserOnly")]
        public IActionResult CheckCoordinate(int id, int x, int y)
        {
            if (x < 0 || y < 0)
            {
                return BadRequest("Invalid coordinate format.");
            }

            var map = MapDataAccess.GetMaps().FirstOrDefault(m => m.Id == id);
            if (map == null)
            {
                return NotFound($"Map with ID {id} not found.");
            }

            bool isOnMap = x < map.Columns && y < map.Rows;

            return Ok(isOnMap);
        }
    }
}
