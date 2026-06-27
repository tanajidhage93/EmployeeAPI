using EmployeeDAPI.Models;
using EmployeeDAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace EmployeeDAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeRepository _repo;
        private readonly string _connectionString;

        public EmployeeController(EmployeeRepository repo, IConfiguration configuration)
        {
            _repo = repo;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IActionResult GetEmployees()
        {
            var data = _repo.GetEmployees();

            return Ok(data);
        }

        [HttpPost("AddEmployee")]
        public IActionResult CreateNewEmployee([FromBody] Employee emp)
        {
            if (emp == null)
                return BadRequest();

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Employee (EmpName, Department, Salary)
                        VALUES (@EmpName, @Department, @Salary);
                        SELECT CAST(SCOPE_IDENTITY() AS int);";

                    cmd.Parameters.AddWithValue("@EmpId", (object)emp.EmpId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmpName", (object)emp.EmpName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Department", (object)emp.Department ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Salary", emp.Salary);

                    con.Open();
                    var result = cmd.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int newId))
                    {
                        emp.EmpId = newId;
                        return CreatedAtAction(nameof(GetEmployees), null, emp);
                    }

                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create employee.");
                }
            }
            catch (SqlException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database error occurred.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}
