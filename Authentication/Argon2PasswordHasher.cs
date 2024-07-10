/*using Microsoft.AspNetCore.Identity;
using Konscious.Security.Cryptography;

namespace robot_controller_api.Authentication
{
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
}*/