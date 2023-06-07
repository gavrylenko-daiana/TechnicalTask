using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace UI.Interfaces
{
    public interface IConsoleManager<TEntity> where TEntity : BaseEntity
    {
        Task PerformOperationsAsync(User user);

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<TEntity> GetByIdAsync(Guid id);

        Task<TEntity> GetByPredicateAsync(Func<TEntity, bool> predicate);

        Task CreateAsync(TEntity entity);

        Task UpdateAsync(Guid id, TEntity entity);

        Task DeleteAsync(Guid id);

        Task<string> GetTrimAsync(string input);
    }
}