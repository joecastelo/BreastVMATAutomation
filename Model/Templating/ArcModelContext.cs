using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace BreastVMATAutomation.Model.Templating
{
    public class ArcModelContext
    {
        private readonly string ConnectionString;

        public ArcModelContext(string version)
        {
            ConnectionString = $"Data Source=ArcModels{version}.db;Version=3;";
            
            // Ensure the database and table exist
            CreateDatabaseAndTable();
        }

        private void CreateDatabaseAndTable()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string createTableQuery = @"CREATE TABLE IF NOT EXISTS Arcs (
                                        Id TEXT PRIMARY KEY,
                                        GapSectorAngle INTEGER,
                                        NbPartialArcs INTEGER,
                                        NbIsoCenters INTEGER,
                                        InnerAngleMargin INTEGER,
                                        OuterAngleMargin INTEGER)";
                SQLiteCommand command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();
            }
        }

        public void InsertArc(ArcModel arc)
        {
            arc.ResolveId();
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string insertQuery = @"INSERT INTO Arcs (Id, GapSectorAngle, NbPartialArcs, NbIsoCenters,InnerAngleMargin,OuterAngleMargin)
                                   VALUES (@Id, @GapSectorAngle, @NbPartialArcs, @NbIsoCenters, @InnerAngleMargin, @OuterAngleMargin)";
                SQLiteCommand command = new SQLiteCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@Id", arc.Id);
                command.Parameters.AddWithValue("@GapSectorAngle", arc.GapSectorAngle);
                command.Parameters.AddWithValue("@NbPartialArcs", arc.NbPartialArcs);
                command.Parameters.AddWithValue("@NbIsoCenters", arc.NbIsoCenters);
                command.Parameters.AddWithValue("@InnerAngleMargin", arc.InnerAngleMargin);
                command.Parameters.AddWithValue("@OuterAngleMargin", arc.OuterAngleMargin);

                command.ExecuteNonQuery();
            }
        }

        public List<ArcModel> GetAllArcs()
        {
            List<ArcModel> arcs = new List<ArcModel>();
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string selectQuery = "SELECT * FROM Arcs";
                SQLiteCommand command = new SQLiteCommand(selectQuery, connection);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ArcModel arc = new ArcModel
                        {
                            Id = reader["Id"].ToString(),
                            GapSectorAngle = Convert.ToInt32(reader["GapSectorAngle"]),
                            NbPartialArcs = Convert.ToInt32(reader["NbPartialArcs"]),
                            NbIsoCenters = Convert.ToInt32(reader["NbIsoCenters"]),
                            InnerAngleMargin = Convert.ToInt32(reader["InnerAngleMargin"]),
                            OuterAngleMargin = Convert.ToInt32(reader["OuterAngleMargin"]),

                        };
                        arcs.Add(arc);
                    }
                }
            }
            return arcs;
        }

        public void UpdateArc(ArcModel arc)
        {
            var copy = new ArcModel
            {
                GapSectorAngle = arc.GapSectorAngle,
                InnerAngleMargin = arc.InnerAngleMargin,
                OuterAngleMargin = arc.OuterAngleMargin,
                NbIsoCenters = arc.NbIsoCenters,
                NbPartialArcs = arc.NbPartialArcs,
            };
            copy.ResolveId();
            DeleteArc(arc.Id);
            InsertArc(copy);
        }

        public void DeleteArc(string id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM Arcs WHERE Id = @Id";
                SQLiteCommand command = new SQLiteCommand(deleteQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }
    }
}
