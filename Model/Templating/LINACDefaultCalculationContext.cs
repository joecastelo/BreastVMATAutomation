using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace BreastVMATAutomation.Model.Templating
{
    public class LINACDefaultCalculationContext
    {
        private readonly string ConnectionString;

        public LINACDefaultCalculationContext(string version)
        {
            ConnectionString = $"Data Source=LINACDefaultCalculations{version}.db;Version=3;";
            CreateDatabaseAndTable();
        }

        private void CreateDatabaseAndTable()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string createTableQuery = @"CREATE TABLE IF NOT EXISTS LINACDefaultCalculations (
                                            LINAC TEXT PRIMARY KEY,
                                            PhotonCalculationModel TEXT,
                                            DVHEstimationModel TEXT,
                                            PortalDoseModel TEXT,
                                            OptimizationModel TEXT)";
                SQLiteCommand command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();
            }
        }

        public void InsertLINACDefaultCalculation(LINACDefaultCalculation linacDefaultCalculation)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string insertQuery = @"INSERT INTO LINACDefaultCalculations (LINAC, PhotonCalculationModel, DVHEstimationModel, PortalDoseModel, OptimizationModel)
                                       VALUES (@LINAC, @PhotonCalculationModel, @DVHEstimationModel, @PortalDoseModel, @OptimizationModel)";
                SQLiteCommand command = new SQLiteCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@LINAC", linacDefaultCalculation.LINAC);
                command.Parameters.AddWithValue("@PhotonCalculationModel", linacDefaultCalculation.PhotonCalculationModel);
                command.Parameters.AddWithValue("@DVHEstimationModel", linacDefaultCalculation.DVHEstimationModel);
                command.Parameters.AddWithValue("@PortalDoseModel", linacDefaultCalculation.PortalDoseModel);
                command.Parameters.AddWithValue("@OptimizationModel", linacDefaultCalculation.OptimizationModel);
                command.ExecuteNonQuery();
            }
        }

        public List<LINACDefaultCalculation> GetAllLINACDefaultCalculations()
        {
            List<LINACDefaultCalculation> linacDefaultCalculations = new List<LINACDefaultCalculation>();
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string selectQuery = "SELECT * FROM LINACDefaultCalculations";
                SQLiteCommand command = new SQLiteCommand(selectQuery, connection);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LINACDefaultCalculation linacDefaultCalculation = new LINACDefaultCalculation
                        {
                            LINAC = reader["LINAC"].ToString(),
                            PhotonCalculationModel = reader["PhotonCalculationModel"].ToString(),
                            DVHEstimationModel = reader["DVHEstimationModel"].ToString(),
                            PortalDoseModel = reader["PortalDoseModel"].ToString(),
                            OptimizationModel = reader["OptimizationModel"].ToString()
                        };
                        linacDefaultCalculations.Add(linacDefaultCalculation);
                    }
                }
            }
            return linacDefaultCalculations;
        }

        public void UpdateLINACDefaultCalculation(LINACDefaultCalculation linacDefaultCalculation)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string updateQuery = @"UPDATE LINACDefaultCalculations 
                                       SET PhotonCalculationModel = @PhotonCalculationModel, 
                                           DVHEstimationModel = @DVHEstimationModel, 
                                           PortalDoseModel = @PortalDoseModel, 
                                           OptimizationModel = @OptimizationModel
                                       WHERE LINAC = @LINAC";
                SQLiteCommand command = new SQLiteCommand(updateQuery, connection);
                command.Parameters.AddWithValue("@PhotonCalculationModel", linacDefaultCalculation.PhotonCalculationModel);
                command.Parameters.AddWithValue("@DVHEstimationModel", linacDefaultCalculation.DVHEstimationModel);
                command.Parameters.AddWithValue("@PortalDoseModel", linacDefaultCalculation.PortalDoseModel);
                command.Parameters.AddWithValue("@OptimizationModel", linacDefaultCalculation.OptimizationModel);
                command.Parameters.AddWithValue("@LINAC", linacDefaultCalculation.LINAC);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteLINACDefaultCalculation(string linac)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM LINACDefaultCalculations WHERE LINAC = @LINAC";
                SQLiteCommand command = new SQLiteCommand(deleteQuery, connection);
                command.Parameters.AddWithValue("@LINAC", linac);
                command.ExecuteNonQuery();
            }
        }
    }
}
