using System.Collections.Generic;
using Emploes.Data.Entities;

namespace Emploes.Data.Repositories.Interfaces
{
    public interface IEmployeRepository
    {
        int Create(Employe employe);
        void Delete(int id);
        Employe Get(int id);
        IEnumerable<Employe> GetAllEmployes();
        IEnumerable<Employe> GetAllEmployesByCompany(int CompanyId);
        IEnumerable<Employe> GetAllEmployesByDepartment(int DepartmentId);
        void Update(Employe employe);
    }
}
