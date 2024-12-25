using System.Text.Json;
using System.Data.Odbc;
using Microsoft.AspNetCore.Mvc;
using System.IO;

[ApiController]
[Route("api/[controller]")]
public class DownloadController : Controller
{
    [HttpPost("generate")]
    public IActionResult GenerateAndDownloadMdb([FromBody] UserData userData)
    {
        // Define the path for the MDB file
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Template.mdb");

        // Ensure the directory exists (create if not)
        var directory = Path.GetDirectoryName(filePath);
        //if (!Directory.Exists(directory))
        //{
        //    Directory.CreateDirectory(directory);
        //}
        string duplicatedPath = Path.Combine(Directory.GetCurrentDirectory(), "DuplicatedDB/DataBase.mdb");
        if (System.IO.File.Exists(duplicatedPath)) System.IO.File.Delete(duplicatedPath);
        System.IO.File.Copy(filePath, duplicatedPath);
        InsertIntoDB(duplicatedPath, userData.firstName, userData.lastName);
        // Create the MDB file if it doesn't exist
        //if (!System.IO.File.Exists(filePath))
        //{
        //    CreateMdbFile(filePath); // Create the file with a valid schema
        //}

        // Read the generated file as a byte array
        var fileBytes = System.IO.File.ReadAllBytes(duplicatedPath);
        FileContentResult file = File(fileBytes, "application/vnd.ms-access", "DataBase.mdb");
        System.IO.File.Delete(duplicatedPath);
        // Return the file to the user for download
        return file;
    }
    [HttpPost("generatemulti")]
    public IActionResult GenerateMultiAndDownloadMdb([FromBody] UserData[] userDatas)
    {
        // Define the path for the MDB file
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Template.mdb");

        // Ensure the directory exists (create if not)
        var directory = Path.GetDirectoryName(filePath);
        //if (!Directory.Exists(directory))
        //{
        //    Directory.CreateDirectory(directory);
        //}
        string duplicatedPath = Path.Combine(Directory.GetCurrentDirectory(), "DuplicatedDB/DataBase.mdb");
        if (System.IO.File.Exists(duplicatedPath)) System.IO.File.Delete(duplicatedPath);
        System.IO.File.Copy(filePath, duplicatedPath);
        InsertMultiIntoDB(duplicatedPath, userDatas);
        // Create the MDB file if it doesn't exist
        //if (!System.IO.File.Exists(filePath))
        //{
        //    CreateMdbFile(filePath); // Create the file with a valid schema
        //}

        // Read the generated file as a byte array
        byte[] fileBytes = System.IO.File.ReadAllBytes(duplicatedPath);
        FileContentResult file = File(fileBytes, "application/vnd.ms-access", "DataBase.mdb");
        System.IO.File.Delete(duplicatedPath);
        // Return the file to the user for download
        return file;
    }
    [HttpPost("test")]
    public IActionResult Hello([FromBody] int num)
    {
        string text = num == 0 ? "Yes, it's zero!" : "No, it's not";
        return Ok(text);
    }
    [HttpPost("testmodel")]
    public IActionResult GetData([FromBody] UserData userData)
    {
        return Ok(userData);
    }
    [HttpPost("testmultimodel")]
    public IActionResult GetMultiData([FromBody] UserData[] userData)
    {
        return Ok(userData);
    }
    public void InsertIntoDB(string filePath, string firstName, string lastName)
    {
        string driver = "{Microsoft Access Driver (*.mdb)}";
        string connectionString = $"Driver={driver};Dbq={filePath};Uid=Admin;Pwd=;";
        using (OdbcConnection connection = new OdbcConnection(connectionString))
        {
            connection.Open();
            string insertText = "INSERT INTO Users([First Name], [Last Name]) VALUES(?, ?)";
            using (OdbcCommand command = new OdbcCommand(insertText, connection))
            {
                command.Parameters.AddWithValue("First Name", firstName);
                command.Parameters.AddWithValue("Last Name", lastName);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
    public void InsertMultiIntoDB(string filePath, IEnumerable<UserData> users)
    {
        string driver = "{Microsoft Access Driver (*.mdb)}";
        string connectionString = $"Driver={driver};Dbq={filePath};Uid=Admin;Pwd=;";
        using (OdbcConnection connection = new OdbcConnection(connectionString))
        {
            connection.Open();
            string insertText = "INSERT INTO Users([First Name], [Last Name]) VALUES(?, ?)";
            foreach (UserData user in users)
            {
                using (OdbcCommand command = new OdbcCommand(insertText, connection))
                {
                    command.Parameters.AddWithValue("First Name", user.firstName);
                    command.Parameters.AddWithValue("Last Name", user.lastName);
                    command.ExecuteNonQuery();
                }
            }
            connection.Close();
        }
    }
    private void CreateMdbFile(string filePath)
    {
        // Connection string for creating an Access MDB file using OleDb
        string connectionString = @"Driver={Microsoft Access Driver (*.mdb)};Dbq=C:\Data\UsersDB.mdb;Uid=Admin;Pwd=;";

        try
        {
            // Create the MDB file with a valid table
            using (var connection = new OdbcConnection(connectionString))
            {
                connection.Open();

                // Create the table in the MDB file
                var createTableQuery = "CREATE TABLE SampleTable (Id AUTOINCREMENT PRIMARY KEY, Name VARCHAR(255), Age INT)";
                using (var command = new OdbcCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Insert some data into the table
                var insertDataQuery = "INSERT INTO SampleTable (Name, Age) VALUES (?, ?)";
                using (var insertCommand = new OdbcCommand(insertDataQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("?", "Alice");
                    insertCommand.Parameters.AddWithValue("?", 30);
                    insertCommand.ExecuteNonQuery();

                    insertCommand.Parameters.Clear();
                    insertCommand.Parameters.AddWithValue("?", "Bob");
                    insertCommand.Parameters.AddWithValue("?", 25);
                    insertCommand.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
        catch (Exception ex)
        {
            // Log the exception for debugging purposes
            Console.WriteLine("Error creating MDB file: " + ex.Message);
        }
    }
}
public class UserData
{
    public string? firstName { get; set; }
    public string? lastName { get; set; }
}
