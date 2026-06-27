using EmployeeDAPI.Models;
using Microsoft.Data.SqlClient;

namespace EmployeeDAPI.Repository
{
    public class EmployeeRepository
    {
        private readonly string _connectionString;  

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection"); 
        }

        public List<Employee> GetEmployees()
        {
            List<Employee> employees = new List<Employee>();
            Employee objEmp = null;

            using(SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "Select * From Employee";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    objEmp = new Employee()
                    {
                        EmpId = Convert.ToInt32(reader["EmpId"]),
                        EmpName = reader["EmpName"].ToString(),
                        Department = reader["Department"].ToString(),
                        Salary = Convert.ToDecimal(reader["Salary"])
                    };

                    employees.Add(objEmp);
                }
            }

            return employees;
        }
    }
}
