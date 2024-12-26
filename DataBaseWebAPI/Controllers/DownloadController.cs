using System.Data.Odbc;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DownloadController : Controller
{
    private readonly IWebHostEnvironment _env;

    public DownloadController(IWebHostEnvironment env)
    {
        _env = env;
    }

    private void CleanUpDuplicateFile(string filePath)
    {
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateAndDownloadMdbAsync([FromBody] UserData userData)
    {
        try
        {
            string templatePath = "Template.mdb";
            string duplicatePath = Path.Combine("DuplicatedDB", "DataBase.mdb");

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(duplicatePath));

            // Duplicate the template
            if (System.IO.File.Exists(duplicatePath))
            {
                System.IO.File.Delete(duplicatePath);
            }
            System.IO.File.Copy(templatePath, duplicatePath);

            // Insert user data
            await InsertIntoDBAsync(duplicatePath, userData.firstName ?? "DefaultFirstName", userData.lastName ?? "DefaultLastName");

            // Read file bytes
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(duplicatePath);

            // Return the file for download
            return File(fileBytes, "application/vnd.ms-access", "DataBase.mdb");
        }
        catch (Exception ex)
        {
            // Log and return error response
            Console.WriteLine($"Error generating MDB file: {ex.Message}");
            return StatusCode(500, $"Root: {_env.ContentRootPath}. An error occurred while generating the MDB file: {ex.Message}.");
        }
        finally
        {
            string duplicatePath = Path.Combine(_env.ContentRootPath, "DuplicatedDB", "DataBase.mdb");
            CleanUpDuplicateFile(duplicatePath);
        }
    }


    [HttpPost("generatemulti")]
    public async Task<IActionResult> GenerateMultiAndDownloadMdb([FromBody] UserData[] userDatas)
    {
        try
        {
            string templatePath = Path.Combine(_env.ContentRootPath, "Template.mdb");
            string duplicatePath = Path.Combine(_env.ContentRootPath, "DuplicatedDB", "DataBase.mdb");

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(duplicatePath));

            // Duplicate the template
            if (System.IO.File.Exists(duplicatePath))
            {
                System.IO.File.Delete(duplicatePath);
            }
            System.IO.File.Copy(templatePath, duplicatePath);

            // Insert multiple user data
            await InsertMultiIntoDBAsync(duplicatePath, userDatas);

            // Read the file as byte array
            byte[] fileBytes = System.IO.File.ReadAllBytes(duplicatePath);

            // Return the file for download
            return File(fileBytes, "application/vnd.ms-access", "DataBase.mdb");
        }
        catch (Exception ex)
        {
            // Log and return error response
            Console.WriteLine($"Error generating MDB file: {ex.Message}");
            return StatusCode(500, "An error occurred while generating the MDB file.");
        }
        finally
        {
            string duplicatePath = Path.Combine(_env.ContentRootPath, "DuplicatedDB", "DataBase.mdb");
            if (System.IO.File.Exists(duplicatePath))
            {
                System.IO.File.Delete(duplicatePath);
            }
        }
    }

    public async Task InsertIntoDBAsync(string filePath, string firstName, string lastName)
    {
        string driver = "{Microsoft Access Driver (*.mdb)}";
        string connectionString = $"Driver={driver};Dbq={filePath};Uid=Admin;Pwd=;";
        await using var connection = new OdbcConnection(connectionString);
        await connection.OpenAsync();

        string query = "INSERT INTO Users([First Name], [Last Name]) VALUES(?, ?)";
        await using var command = new OdbcCommand(query, connection);
        command.Parameters.AddWithValue("First Name", firstName);
        command.Parameters.AddWithValue("Last Name", lastName);
        await command.ExecuteNonQueryAsync();
    }

    public async Task InsertMultiIntoDBAsync(string filePath, IEnumerable<UserData> users)
    {
        using var connection = GetDbConnection(filePath);
        await connection.OpenAsync();
        const string insertText = "INSERT INTO Users([First Name], [Last Name]) VALUES(?, ?)";

        foreach (var user in users)
        {
            using var command = new OdbcCommand(insertText, connection);
            command.Parameters.AddWithValue("First Name", user.firstName);
            command.Parameters.AddWithValue("Last Name", user.lastName);
            await command.ExecuteNonQueryAsync();
        }
    }

    private OdbcConnection GetDbConnection(string filePath)
    {
        string driver = "{Microsoft Access Driver (*.mdb)}";
        string connectionString = $"Driver={driver};Dbq={filePath};Uid=Admin;Pwd=;";
        return new OdbcConnection(connectionString);
    }
}

public class UserData
{
    public string? firstName { get; set; }
    public string? lastName { get; set; }
}
