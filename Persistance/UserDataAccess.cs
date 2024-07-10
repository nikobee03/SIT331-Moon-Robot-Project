using Npgsql;

namespace robot_controller_api.Persistance
{
    public class UserDataAccess
    {
        private const string CONNECTION_STRING =
        "Host=localhost;Username=postgres;Password=lobster0408;Database=sit331";

        public static List<UserModel> GetAllUsers()
        {
            var users = new List<UserModel>();

            using (var conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                using (var command = new NpgsqlCommand("SELECT * FROM public.user", conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new UserModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),                                  // Updated formatting as old way used in RobotCommandDataAccess and MapDataAccess was causing issues in here for unknown reasons
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                FirstName = reader.GetString(reader.GetOrdinal("firstname")),
                                LastName = reader.GetString(reader.GetOrdinal("lastname")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("passwordhash")),
                                Role = reader.GetString(reader.GetOrdinal("role")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("createddate")),
                                ModifiedDate = reader.GetDateTime(reader.GetOrdinal("modifieddate"))
                            };

                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }
        public static List<UserModel> GetUsersByRole(string role)
        {
            var users = new List<UserModel>();

            using (var conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                using (var command = new NpgsqlCommand("SELECT * FROM public.user WHERE role = @Role;", conn))
                {
                    command.Parameters.AddWithValue("@Role", role);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new UserModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                FirstName = reader.GetString(reader.GetOrdinal("firstname")),
                                LastName = reader.GetString(reader.GetOrdinal("lastname")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("passwordhash")),
                                Role = reader.GetString(reader.GetOrdinal("role")),
                                Description = reader.GetString(reader.GetOrdinal("description")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("createddate")),
                                ModifiedDate = reader.GetDateTime(reader.GetOrdinal("modifieddate"))
                            };

                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        public static void UpdateUser(int id, UserModel updatedUser)
        {
            using (var connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                string sql = @"UPDATE ""user""
                       SET ""FirstName"" = @FirstName,
                           ""LastName"" = @LastName,
                           ""description"" = @Description,
                           ""role"" = @Role,
                           ""createddate"" = @CreatedDate,
                           ""modifieddate"" = @ModifiedDate
                       WHERE ""id"" = @Id;";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@FirstName", updatedUser.FirstName);
                    command.Parameters.AddWithValue("@LastName", updatedUser.LastName);
                    command.Parameters.AddWithValue("@Description", updatedUser.Description);
                    command.Parameters.AddWithValue("@Role", updatedUser.Role);
                    command.Parameters.AddWithValue("@CreatedDate", updatedUser.CreatedDate);
                    command.Parameters.AddWithValue("@ModifiedDate", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }
        }
 
    public static void InsertUser(UserModel newUser)
        {
            using (var conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                using (var command = new NpgsqlCommand(
                    "INSERT INTO \"user\" (\"Email\", \"FirstName\", \"LastName\", \"PasswordHash\", \"description\", \"role\", \"createddate\", \"modifieddate\") VALUES (@Email, @FirstName, @LastName, @PasswordHash, @Description, @Role, @CreatedDate, @ModifiedDate) RETURNING id", conn))
                {
                    command.Parameters.AddWithValue("@Email", newUser.Email);
                    command.Parameters.AddWithValue("@FirstName", newUser.FirstName);
                    command.Parameters.AddWithValue("@LastName", newUser.LastName);
                    command.Parameters.AddWithValue("@PasswordHash", newUser.PasswordHash);
                    command.Parameters.AddWithValue("@Description", newUser.Description);
                    command.Parameters.AddWithValue("@Role", newUser.Role);
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    command.Parameters.AddWithValue("@ModifiedDate", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }
        }


        public static void DeleteUser(int id)
        {
            using (var connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                string sql = @"DELETE FROM public.user WHERE id = @Id;";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateEmailPassword(UserModel patcheduser)
        {
            using (var conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                string sql = @"UPDATE ""user""
                       SET ""Email"" = @Email,
                           ""PasswordHash"" = @PasswordHash
                       WHERE ""id"" = @Id;";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", patcheduser.Id);
                    cmd.Parameters.AddWithValue("@Email", patcheduser.Email);
                    cmd.Parameters.AddWithValue("@PasswordHash", patcheduser.PasswordHash);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
