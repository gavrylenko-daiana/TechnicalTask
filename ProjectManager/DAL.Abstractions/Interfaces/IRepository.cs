using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace DAL.Abstractions.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<Result<List<T>>> GetAllAsync(int pageNumber = 1, int pageSize = 10);

    Task<Result<T>> GetByIdAsync(Guid id);

    Task<Result<T>> GetByPredicateAsync(Func<T, bool> predicate);

    Task<Result<bool>> AddAsync(T obj);

    Task<Result<bool>> UpdateAsync(Guid id, T updatedObj);

    Task<Result<bool>> DeleteAsync(Guid id);
}