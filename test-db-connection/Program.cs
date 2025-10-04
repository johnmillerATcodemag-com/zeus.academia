using Microsoft.Data.SqlClient;
using System;

try
{
    var connectionString = "Server=(localdb)\\mssqllocaldb;Database=ZeusAcademiaDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true";

    using var connection = new SqlConnection(connectionString);
    Console.WriteLine("Attempting to connect to database...");
    connection.Open();
    Console.WriteLine("✓ Database connection successful!");

    var command = new SqlCommand("SELECT @@VERSION", connection);
    var version = command.ExecuteScalar();
    Console.WriteLine($"SQL Server Version: {version}");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Database connection failed: {ex.Message}");
    Console.WriteLine($"Exception Type: {ex.GetType().Name}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();