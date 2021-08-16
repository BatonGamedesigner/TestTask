using System.Collections.Generic;
using System.Data;
using Emploes.Data.Entities;
using System.Data.SqlClient;
using System.Linq;
using System.Configuration;
using Emploes.Data.Repositories.Interfaces;
using Dapper;

namespace Emploes.Data.Repositories
{
    public class EmployeRepository : IEmployeRepository
    {
        private readonly string _connectionString;
        public EmployeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Employe> GetAllEmployesByCompany(int CompanyId)
        {
            using IDbConnection database = new SqlConnection(_connectionString);
            var sqlQuery = @"SELECT empl.Id, empl.Name, empl.Surname, empl.Phone, empl.CompanyId , pass.Id as PassId, pass.Type, pass.Number, empl.DepartmentId, dep.Name, dep.Phone  FROM Employe as empl INNER JOIN Department AS dep on dep.id = empl.DepartmentId INNER JOIN Passport AS pass on pass.UserId = empl.Id WHERE empl.CompanyId = @CompanyId";

            return database.Query<Employe, Passport, Department, Employe>(sqlQuery, (empl, pass, dep) =>
            {
                empl.Passport = pass;
                empl.Department = dep;
                return empl;
            }, splitOn: "PassId,DepartmentId", param: new { CompanyId }).ToList();
        }

        public IEnumerable<Employe> GetAllEmployesByDepartment(int DepartmentId)
        {
            using IDbConnection database = new SqlConnection(_connectionString);
            var sqlQuery = @"SELECT empl.Id, empl.Name, empl.Surname, empl.Phone, empl.CompanyId , pass.Id as PassId, pass.Type, pass.Number, empl.DepartmentId, dep.Name, dep.Phone  FROM Employe as empl INNER JOIN Department AS dep on dep.id = empl.DepartmentId INNER JOIN Passport AS pass on pass.UserId = empl.Id WHERE empl.DepartmentId = @DepartmentId";

            return database.Query<Employe, Passport, Department, Employe>(sqlQuery, (empl, pass, dep) =>
            {
                empl.Passport = pass;
                empl.Department = dep;
                return empl;
            }, splitOn: "PassId,DepartmentId", param: new { DepartmentId }).ToList();
        }

        public Employe Get(int id)
        {
            using IDbConnection database = new SqlConnection(_connectionString);
            var sqlQuery = @"SELECT empl.Id, empl.Name, empl.Surname, empl.Phone, empl.CompanyId , pass.Id as PassId, pass.Type, pass.Number, empl.DepartmentId, dep.Name, dep.Phone  FROM Employe as empl INNER JOIN Department AS dep on dep.id = empl.DepartmentId INNER JOIN Passport AS pass on pass.UserId = empl.Id WHERE empl.Id = @id";
            return database.Query<Employe, Passport, Department, Employe>(sqlQuery, (empl, pass, dep) =>
            {
                empl.Passport = pass;
                empl.Department = dep;
                return empl;
            }, splitOn: "PassId,DepartmentId", param: new { id }).FirstOrDefault();
        }

        public int Create(Employe employe)
        {
            using IDbConnection database = new SqlConnection(_connectionString);
            var sqlQuery = @"SELECT id FROM Department WHERE Name = @Name AND Phone = @Phone";
            var dataDepartment = employe.Department;
            var departmentId = database.Query<int>(sqlQuery, param: dataDepartment).FirstOrDefault();
            if (departmentId == 0)
            {
                sqlQuery = @"INSERT INTO Department (Name, Phone) VALUES(@Name, @Phone); SELECT CAST(SCOPE_IDENTITY() as INTEGER)";
                departmentId = database.Query<int>(sqlQuery, param: dataDepartment).First();
            }
            sqlQuery = "INSERT INTO Employe (Name, Surname, Phone, CompanyId, DepartmentId) VALUES (@Name, @Surname, @Phone, @CompanyId, @departmentId); SELECT CAST(SCOPE_IDENTITY() AS INTEGER)";
            var userId = database.Query<int>(sqlQuery, param: new {employe.Name, employe.Surname, employe.Phone, employe.CompanyId, departmentId}).First();
            sqlQuery = "INSERT INTO Passport (Type, Number, UserId) values(@Type, @Number, @userId)";
            var passport = employe.Passport;
            _ = database.Query<int>(sqlQuery, new { passport.Type, passport.Number, userId }).FirstOrDefault();
            return userId;
        }
        
        public void Update(Employe employe)
        {
            using IDbConnection database = new SqlConnection(_connectionString);
            var sqlQuery = @"SELECT id FROM Department WHERE Name = @Name AND Phone = @Phone";
            var dataDepartment = employe.Department;
            var departmentId = database.Query<int>(sqlQuery, param: dataDepartment).FirstOrDefault();
            if (departmentId == 0)
            {
                sqlQuery = @"INSERT INTO Department (Name, Phone) VALUES(@Name, @Phone); SELECT CAST(SCOPE_IDENTITY() as INTEGER)";
                departmentId = database.Query<int>(sqlQuery, param: dataDepartment).First();
            }
            sqlQuery = "UPDATE Employe SET Name = @Name, Surname = @Surname, Phone = @Phone, CompanyId = @CompanyId, DepartmentId = @departmentId WHERE Id = @Id";
            database.Execute(sqlQuery, param: new {employe.Id, employe.Name, employe.Surname, employe.Phone, employe.CompanyId, departmentId});
            sqlQuery = "UPDATE Passport SET Type = @Type, Number = @Number WHERE UserId = @userId";
            var passport = employe.Passport;
            database.Execute(sqlQuery,  new { passport.Type, passport.Number, userId = employe.Id });
        }
        
        public void Delete(int id)
        {
            using IDbConnection database = new SqlConnection(_connectionString);
            var sqlQuery = "DELETE FROM Employe WHERE Id = @id; DELETE FROM Passport WHERE UserId = @id";
            database.Execute(sqlQuery, new { id });
        }
    }
}
