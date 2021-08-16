using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emploes.Api;
using Emploes.Data.Entities;
using Emploes.Data.Repositories;
using Emploes.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Emploes.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployesController : ControllerBase
    {
        private readonly IEmployeRepository _employeRepository;

        private readonly ILogger<EmployesController> _logger;

        public EmployesController(ILogger<EmployesController> logger, IOptions<ConnectionConfig> connectionConfig)
        {
            string connectionString = connectionConfig.Value.DapperConnection;
            _employeRepository = new EmployeRepository(connectionString);
            _logger = logger;
        }

        [HttpGet("/get/all/")]
        public IEnumerable<Employe> Get()
        {
            return _employeRepository.GetAllEmployes();
        }

        [HttpGet("/get/all/by-company/{CompanyId:int}/")]
        public IEnumerable<Employe> GetByCompany(int CompanyId)
        {
            return _employeRepository.GetAllEmployesByCompany(CompanyId);
        }

        [HttpGet("/get/all/by-department/{DepartmentId:int}/")]
        public IEnumerable<Employe> GetByDepartment(int DepartmentId)
        {
            return _employeRepository.GetAllEmployesByDepartment(DepartmentId);
        }

        [HttpDelete("/delete/{id:int}/")]
        public void Delete(int id)
        {
            _employeRepository.Delete(id);
        }

        [HttpPost("/create/")]
        public int Create([FromBody] Employe employe)
        {
            return _employeRepository.Create(employe);
        }
        
        [HttpPut("/update/{id:int}")]
        public void Create(int id,[FromBody] Employe employe)
        {
            employe.Id = id;
            _employeRepository.Update(employe);
        }
    }
}
