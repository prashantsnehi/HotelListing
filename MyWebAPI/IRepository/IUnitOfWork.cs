using System;
using System.Threading.Tasks;
using MyWebAPI.Data;

namespace MyWebAPI.IRepository
{
    public interface IUnitOfWork: IDisposable
    {
        IGenericRepository<Country> Countries { get; }
        IGenericRepository<Hotel> Hotels { get; }
        Task Save();
    }
}
