using Npgsql;

namespace robot_controller_api.Persistance
{
    public static class RobotCommandDataAccess
    {
        // Connection string is usually set in a config file for the ease of change.
        private const string CONNECTION_STRING =
        "Host=localhost;Username=postgres;Password=lobster0408;Database=sit331";
        public static List<RobotCommand> GetRobotCommands()
        {
            var robotCommands = new List<RobotCommand>();
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT * FROM robotcommand", conn);
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                // Create a new RobotCommand instance and add it to the list
                var robotCommand = new RobotCommand(
                    id: (int)dr["id"],
                    name: (string)dr["name"],
                    description: dr["description"] as string,
                    isMoveCommand: (bool)dr["ismovecommand"],
                    createdDate: (DateTime)dr["createddate"],
                    modifiedDate: (DateTime)dr["modifieddate"]
                );
                robotCommands.Add(robotCommand);
            }
            return robotCommands;
        }

        /*        public static void UpdateRobotCommand(int id, RobotCommand robotCommand)
                {
                    using var conn = new NpgsqlConnection(CONNECTION_STRING);
                    conn.Open();

                    string sql = @"UPDATE robotcommand 
                           SET ""Name"" = @Name, 
                               description = @Description, 
                               ismovecommand = @IsMoveCommand, 
                               modifieddate = @ModifiedDate 
                           WHERE id = @Id;";

                    using var cmd = new NpgsqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Id", robotCommand.Id);
                    cmd.Parameters.AddWithValue("@Name", robotCommand.Name);
                    cmd.Parameters.AddWithValue("@Description", robotCommand.Description);
                    cmd.Parameters.AddWithValue("@IsMoveCommand", robotCommand.IsMoveCommand);
                    cmd.Parameters.AddWithValue("@ModifiedDate", DateTime.Now); // Update the modified date

                    cmd.ExecuteNonQuery();
                }*/

        public static void UpdateRobotCommand(int id, RobotCommand robotCommand)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            string sql = @"UPDATE robotcommand 
                   SET ""Name"" = @Name, 
                       ismovecommand = @IsMoveCommand, 
                       modifieddate = @ModifiedDate";

            // Check if the Description is not null
            if (!string.IsNullOrEmpty(robotCommand.Description))
            {
                sql += ", description = @Description";
            }
            else
            {
                sql += ", description = NULL"; // Explicitly set the description to NULL
            }

            sql += " WHERE id = @Id;";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", robotCommand.Id);
            cmd.Parameters.AddWithValue("@Name", robotCommand.Name);

            // Add the Description parameter only if it's not null
            if (!string.IsNullOrEmpty(robotCommand.Description))
            {
                cmd.Parameters.AddWithValue("@Description", robotCommand.Description);
            }

            cmd.Parameters.AddWithValue("@IsMoveCommand", robotCommand.IsMoveCommand);
            cmd.Parameters.AddWithValue("@ModifiedDate", DateTime.Now); // Update the modified date

            cmd.ExecuteNonQuery();
        }

        public static void InsertRobotCommand(RobotCommand robotCommand)
        {
            using (var conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(
                    "INSERT INTO \"robotcommand\" (\"Name\", \"description\", \"ismovecommand\", \"createddate\", \"modifieddate\") VALUES (@Name, @Description, @IsMoveCommand, @CreatedDate, @ModifiedDate)", conn))
                {
                    cmd.Parameters.AddWithValue("@Name", robotCommand.Name);
                    cmd.Parameters.AddWithValue("@Description", (object)robotCommand.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsMoveCommand", robotCommand.IsMoveCommand);
                    cmd.Parameters.AddWithValue("@CreatedDate", robotCommand.CreatedDate);
                    cmd.Parameters.AddWithValue("@ModifiedDate", robotCommand.ModifiedDate);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static void DeleteRobotCommand(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            string sql = @"DELETE FROM robotcommand WHERE id = @Id;";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }


    }
}
