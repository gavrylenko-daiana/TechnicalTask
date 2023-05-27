using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class TesterService : GenericService<User>, ITesterService
{
    public TesterService(IRepository<User> repository) : base(repository)
    {
    }
    
}
