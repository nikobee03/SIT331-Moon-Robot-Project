using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Authentication;
using robot_controller_api.Persistance;


namespace robot_controller_api.Controllers
{
    [ApiController]
    [Route("api/users")]

    public class UsersController : ControllerBase
    {
        private IPasswordHasher<UserModel> _passwordHasher;

        public UsersController(IPasswordHasher<UserModel> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        // GET: api/users
        /// <summary>
        /// Retrieves all users
        /// </summary>
        /// <returns> All existing users </returns>
        /// <response code="200">Successfully returns all users</response>
        /// <response code="204">There are no users available</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet(), Authorize(Policy = "AdminOnly")]
        public IEnumerable<UserModel> GetAllUsers()
        {
            return UserDataAccess.GetAllUsers();
        }
    
        /// GET: api/users/admin
        /// <summary>
        /// Retrieves all admin-level users
        /// </summary>
        /// <returns> All existing admin-level users </returns>
        /// <response code="200">Successfully returns all admin-level users</response>
        /// <response code="204">There are no admin-level users available</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("admin"), Authorize(Policy = "AdminOnly")]
        public IEnumerable<UserModel> GetAllAdminUsers()
        {
            return UserDataAccess.GetUsersByRole("Admin");
        }

        // GET: api/users/{id}
        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the desired user.</param>
        /// <returns>The user with the specified ID.</returns>
        /// <response code="200">Successfully returns requested user</response>
        /// <response code="404">If the user with the specified ID is not found</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetUser"), Authorize(Policy = "AdminOnly")]
        public IActionResult GetUserById(int id)
        {
            var user = UserDataAccess.GetAllUsers().FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound();
            }
        }


        // POST: api/users
        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="newUser">The user to be added.</param>
        /// <returns>The newly created user.</returns>
        /// <response code="201">Returns the newly created user</response>
        /// <response code="400">If the user is null</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, Authorize(Policy = "AdminOnly")]
        public IActionResult AddUser(UserModel newUser)
        {
            if (newUser == null)
            {
                return BadRequest();
            }

            var password = newUser.PasswordHash.ToString();
            var pwHash = _passwordHasher.HashPassword(newUser, password);
            var pwVerificationResult = _passwordHasher.VerifyHashedPassword(newUser, pwHash, password);
            if (pwVerificationResult == PasswordVerificationResult.Success)
            {
                newUser.PasswordHash = pwHash;
                newUser.CreatedDate = DateTime.Now;
                newUser.ModifiedDate = DateTime.Now;
                int newId = UserDataAccess.GetAllUsers().Count + 1;
                newUser.Id = newId;

                UserDataAccess.InsertUser(newUser);

                // Get the base URL of the API
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";

                // Construct the URL for the newly created resource
                var locationUrl = $"{baseUrl}/api/users/{newUser.Id}";

                // Return a CreatedAtRoute response with the newly created map and the Location header
                return CreatedAtRoute("GetUser", new { id = newUser.Id }, newUser);
            }
            else
            {
                return BadRequest();
            } 
        }

        // PUT: api/users/{id}
        /// <summary>
        /// Updates an existing user (excluding email and password).
        /// </summary>
        /// <param name="id">The ID of the user to be updated.</param>
        /// <param name="user">The updated user.</param>
        /// <returns>No content if the update is successful.</returns>
        /// <response code="204">No content indicates a successful update</response>
        /// <response code="400">If the user is null</response>
        /// <response code="404">If the user with the specified ID is not found.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}"), Authorize(Policy = "AdminOnly")]
        public IActionResult UpdateUser(int id, UserModel user)
        {
            if (id <=  0)
            {
                return BadRequest();
            }
            if (user == null)
            {
                return BadRequest();
            }

            UserDataAccess.UpdateUser(id, user);
            return NoContent();
        }

        // DELETE: api/users/{id}
        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">The ID of the user to be deleted.</param>
        /// <returns>No content if the deletion is successful.</returns>
        /// <response code="204">No content indicates a successful deletion</response>
        /// <response code="404">If the user with the specified ID is not found.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}"), Authorize(Policy = "AdminOnly")]
        public IActionResult DeleteUser(int id)
        {
            var userToRemove = UserDataAccess.GetAllUsers().FirstOrDefault(c => c.Id == id);

            if (userToRemove == null)
            {
                return NotFound();
            }

            UserDataAccess.DeleteUser(id);

            return NoContent();
        }

        // PATCH: api/users/{id}
        /// <summary>
        /// Updates a user' email and password.
        /// </summary>
        /// <param name="id">The ID of the user to be updated.</param>
        /// <returns>No content if the update is successful.</returns>
        /// <response code="204">No content indicates a successful update</response>
        /// <response code="404">If the user with the specified ID is not found.</response>
        /// <response code="409">If a user with the same emaoil already exists</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPatch("{id}"), Authorize(Policy = "AdminOnly")]
        public IActionResult UpdateEmailPassword(int id, LoginModel login)
        {
            var existingUser = UserDataAccess.GetAllUsers().FirstOrDefault(m => m.Id == id);
            if (existingUser == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(login.Email))
            {
                existingUser.Email = login.Email;
            }

            if (!string.IsNullOrWhiteSpace(login.Password))
            {
                var password = login.Password;
                var pwHash = _passwordHasher.HashPassword(existingUser, password);
                var pwVerificationResult = _passwordHasher.VerifyHashedPassword(existingUser, pwHash, password);
                if (pwVerificationResult != PasswordVerificationResult.Success)
                    return BadRequest();

                existingUser.PasswordHash = pwHash;
            }

            existingUser.ModifiedDate = DateTime.Now;

            UserDataAccess.UpdateEmailPassword(existingUser);

            return NoContent();
        }

    }
}
