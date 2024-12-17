using Microsoft.AspNetCore.Mvc;
using System.Data.Odbc;

namespace DataBaseWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataBaseController : ControllerBase
    {
        //[HttpGet("get-data")]
        //public IActionResult GetData()
        //{
        //    return Ok("This is a test response from GetData.");
        //}
        private readonly string connectionString = @"Driver={Microsoft Access Driver (*.mdb)};Dbq=C:\Data\UsersDB.mdb;Uid=Admin;Pwd=;";
        [HttpGet("get-data")]
        public IActionResult GetData()
        {
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    List<string> data = new List<string>();
                    string query = "SELECT * FROM Users";
                    OdbcCommand command = new OdbcCommand(query, connection);
                    using (OdbcDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string row = string.Empty;
                            for (int i = 0; i < reader.FieldCount; i++) row += reader[i] + " , ";
                            data.Add(row);
                        }
                    }
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                // Log or return the exception details
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
