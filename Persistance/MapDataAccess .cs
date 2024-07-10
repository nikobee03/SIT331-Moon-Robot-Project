using Npgsql;

namespace robot_controller_api.Persistance
{
    public static class MapDataAccess
    {
        private const string CONNECTION_STRING =
        "Host=localhost;Username=postgres;Password=lobster0408;Database=sit331";
        
        public static List<Map> GetMaps()
        {
            var Maps = new List<Map>();
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT * FROM map", conn);
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                var map = new Map(
                    id: (int)dr["id"],
                    name: (string)dr["name"],
                    columns: (int)dr["columns"],
                    rows: (int)dr["rows"],
                    description: dr["description"] as string,
                    issquare: (bool)dr["issquare"],
                    createdDate: (DateTime)dr["createddate"],
                    modifiedDate: (DateTime)dr["modifieddate"]
                    );
                Maps.Add(map);  
            }
            return Maps;
        }
        public static Map GetMapById(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT * FROM map WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                return new Map(
                    id: (int)dr["id"],
                    name: (string)dr["name"],
                    columns: (int)dr["columns"],
                    rows: (int)dr["rows"],
                    description: dr["description"] as string,
                    issquare: (bool)dr["issquare"],
                    createdDate: (DateTime)dr["createddate"],
                    modifiedDate: (DateTime)dr["modifieddate"]
                );
            }

            return null;
        }
        public static void UpdateMap(int id, Map map)
        {
            using (var conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                using (var command = new NpgsqlCommand(
                    "UPDATE \"map\" SET \"Name\" = @Name, \"description\" = @Description, \"columns\" = @Columns, \"rows\" = @Rows, \"modifieddate\" = @ModifiedDate WHERE \"id\" = @Id", conn))
                {
                    command.Parameters.AddWithValue("@Name", map.Name);
                    command.Parameters.AddWithValue("@Description", (object)map.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Columns", map.Columns);
                    command.Parameters.AddWithValue("@Rows", map.Rows);
                    command.Parameters.AddWithValue("@ModifiedDate", DateTime.Now);
                    command.Parameters.AddWithValue("@Id", map.Id); 

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void InsertMap(Map map)
        {
            using (var conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                using (var command = new NpgsqlCommand(
                    "INSERT INTO \"map\" (\"Name\", \"description\", \"columns\", \"rows\", \"createddate\", \"modifieddate\") VALUES (@Name, @Description, @Columns, @Rows, @CreatedDate, @ModifiedDate)", conn))
                {
                    command.Parameters.AddWithValue("@Name", map.Name);
                    command.Parameters.AddWithValue("@Description", (object)map.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Columns", map.Columns);
                    command.Parameters.AddWithValue("@Rows", map.Rows);
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    command.Parameters.AddWithValue("@ModifiedDate", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteMap(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            string sql = @"DELETE FROM map WHERE id = @Id;";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }


    }
}
