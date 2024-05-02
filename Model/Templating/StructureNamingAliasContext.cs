using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace BreastVMATAutomation.Model.Templating
{
    public class StructureNamingAliasContext
    {
        private readonly string _connectionString;

        public StructureNamingAliasContext(string version)
        {
            _connectionString = $"Data Source=StructureNamingAliases{version}.db;Version=3;";

            // Ensure the database and table exist
            CreateDatabaseAndTable();
        }

        private void CreateDatabaseAndTable()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string createTableQuery = @"CREATE TABLE IF NOT EXISTS StructureNamingAliases (
                                        IdOnApplication TEXT PRIMARY KEY,
                                        Alias TEXT)";
                SQLiteCommand command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();
            }
        }

        public void InsertStructureNamingAlias(StructureNamingAlias alias)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string insertQuery = @"INSERT INTO StructureNamingAliases (IdOnApplication, Alias)
                                   VALUES (@IdOnApplication, @Alias)";
                SQLiteCommand command = new SQLiteCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@IdOnApplication", alias.IdOnApplication);
                command.Parameters.AddWithValue("@Alias", alias.Alias);

                command.ExecuteNonQuery();
            }
        }

        public List<StructureNamingAlias> GetAllStructureNamingAliases()
        {
            List<StructureNamingAlias> aliases = new List<StructureNamingAlias>();
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT * FROM StructureNamingAliases";
                SQLiteCommand command = new SQLiteCommand(selectQuery, connection);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StructureNamingAlias alias = new StructureNamingAlias
                        {
                            IdOnApplication = reader["IdOnApplication"].ToString(),
                            Alias = reader["Alias"].ToString()
                        };
                        aliases.Add(alias);
                    }
                }
            }
            return aliases;
        }

        public void UpdateStructureNamingAlias(StructureNamingAlias alias)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string updateQuery = @"UPDATE StructureNamingAliases SET Alias = @Alias WHERE IdOnApplication = @IdOnApplication";
                SQLiteCommand command = new SQLiteCommand(updateQuery, connection);
                command.Parameters.AddWithValue("@Alias", alias.Alias);
                command.Parameters.AddWithValue("@IdOnApplication", alias.IdOnApplication);

                command.ExecuteNonQuery();
            }
        }

        public void DeleteStructureNamingAlias(string idOnApplication)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM StructureNamingAliases WHERE IdOnApplication = @IdOnApplication";
                SQLiteCommand command = new SQLiteCommand(deleteQuery, connection);
                command.Parameters.AddWithValue("@IdOnApplication", idOnApplication);

                command.ExecuteNonQuery();
            }
        }
    }
}
