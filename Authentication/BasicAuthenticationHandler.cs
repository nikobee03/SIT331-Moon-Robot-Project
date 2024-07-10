using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using robot_controller_api.Persistance;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace robot_controller_api.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IPasswordHasher<UserModel> _passwordHasher;
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IPasswordHasher<UserModel> passwordHasher) : base(options, logger, encoder, clock)
        {
            // _passwordHasher = new Argon2PasswordHasher<UserModel>();
            _passwordHasher = passwordHasher;
        }
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            base.Response.Headers.Add("WWW-Authenticate", @"Basic realm=""Access to the robot controller.""");
            var authHeader = base.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
            {
                // No credentials provided
                return Task.FromResult(AuthenticateResult.Fail("No credentials provided"));
            }
            try
            {
                var base64Credentials = authHeader.Substring("Basic ".Length).Trim();
                var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(base64Credentials));
                var credentials = decodedCredentials.Split(':');
                var email = credentials[0];
                var password = credentials[1];

                var userlist = UserDataAccess.GetAllUsers();
                var existingUser = UserDataAccess.GetAllUsers().FirstOrDefault(m => m.Email == email);

                if (existingUser == null)
                {
                    Response.StatusCode = 401;
                    return Task.FromResult(AuthenticateResult.Fail($"Authentication Failed."));
                }

                
                var pwVerificationResult = _passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, password);

                if (pwVerificationResult != PasswordVerificationResult.Success)
                {
                    Response.StatusCode = 401;
                    return Task.FromResult(AuthenticateResult.Fail("Authentication Failed"));
                }

                else if (pwVerificationResult == PasswordVerificationResult.Success)
                {
                    var claims = new[]
                    {
                        new Claim("name", $"{existingUser.FirstName} {existingUser.LastName}"),
                        new Claim(ClaimTypes.Role, existingUser.Role),
                    };
                    var identity = new ClaimsIdentity(claims, "Basic");
                    var claimsPrincipal = new ClaimsPrincipal(identity);
                    var authTicket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

                    return Task.FromResult(AuthenticateResult.Success(authTicket));
                }

                else
                {
                    Response.StatusCode = 401;
                    return Task.FromResult(AuthenticateResult.Fail("Authentication failed"));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error that occurred during authentication.");
                return Task.FromResult(AuthenticateResult.Fail("Server error"));
            }
            
        }
    }

    public class Argon2PasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
    {
        public string HashPassword(TUser user, string password)
        {
            // Create a new instance of the Argon2id algorithm with the provided password as input.
            var argon2 = new Argon2id(System.Text.Encoding.UTF8.GetBytes(password));

            // Set the salt for the algorithm using the user object's string representation.
            argon2.Salt = System.Text.Encoding.UTF8.GetBytes(user.ToString());

            // Set the degree of parallelism for the algorithm (number of parallel threads).
            argon2.DegreeOfParallelism = 8;

            // Set the number of iterations for the algorithm (higher value means more secure but slower).
            argon2.Iterations = 3;

            // Set the memory size for the algorithm (higher value means more secure but more memory-intensive).
            argon2.MemorySize = 1024 * 1024;

            // Generate the hashed password and return it as a Base64-encoded string.
            return Convert.ToBase64String(argon2.GetBytes(16));
        }

        public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
        {
            // Create a new instance of the Argon2id algorithm with the provided password as input.
            var argon2 = new Argon2id(System.Text.Encoding.UTF8.GetBytes(providedPassword));

            // Set the salt for the algorithm using the user object's string representation.
            argon2.Salt = System.Text.Encoding.UTF8.GetBytes(user.ToString());

            // Set the degree of parallelism for the algorithm (number of parallel threads).
            argon2.DegreeOfParallelism = 8;

            // Set the number of iterations for the algorithm (higher value means more secure but slower).
            argon2.Iterations = 3;

            // Set the memory size for the algorithm (higher value means more secure but more memory-intensive).
            argon2.MemorySize = 1024 * 1024;

            // Generate the hash for the provided password and convert it to a Base64-encoded string.
            var hash = Convert.ToBase64String(argon2.GetBytes(16));

            // Compare the generated hash with the previously hashed password.
            if (hashedPassword == hash)
                return PasswordVerificationResult.Success; // Passwords match
            else
                return PasswordVerificationResult.Failed; // Passwords do not match
        }
    }
}
