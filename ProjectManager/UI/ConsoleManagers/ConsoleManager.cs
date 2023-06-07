using System.Net;
using System.Net.Mail;
using BLL.Abstractions.Interfaces;
using BLL.Services;
using Core.Models;

namespace UI.ConsoleManagers;

public abstract class ConsoleManager<TService, TEntity>
    where TEntity : BaseEntity
    where TService : IGenericService<TEntity>
{
    protected readonly TService Service;

    protected ConsoleManager(TService service)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public abstract Task PerformOperationsAsync(User user);

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        try
        {
            return await Service.GetAll();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllAsync: {ex.Message}");

            return Enumerable.Empty<TEntity>();
        }
    }

    public virtual async Task<string> GetTrimAsync(string input)
    {
        try
        {
            return await Service.GetStringWithoutSpace(input);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetByIdAsync: {ex.Message}");

            return null!;
        }
    }

    public virtual async Task<TEntity> GetByIdAsync(Guid id)
    {
        try
        {
            return await Service.GetById(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetByIdAsync: {ex.Message}");

            return null!;
        }
    }

    public virtual async Task<TEntity> GetByPredicateAsync(Func<TEntity, bool> predicate)
    {
        try
        {
            return await Service.GetByPredicate(predicate);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetByPredicateAsync: {ex.Message}");

            return null!;
        }
    }

    public virtual async Task CreateAsync(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        try
        {
            await Service.Add(entity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateAsync: {ex.Message}");
        }
    }

    public virtual async Task UpdateAsync(Guid id, TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        try
        {
            await Service.Update(id, entity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
        }
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        try
        {
            await Service.Delete(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
        }
    }
}