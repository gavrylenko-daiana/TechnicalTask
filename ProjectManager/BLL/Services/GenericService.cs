using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public abstract class GenericService<T> : IGenericService<T> where T : BaseEntity
{
    private readonly IRepository<T> _repository;

    protected GenericService(IRepository<T> repository)
    {
        _repository = repository;
    }

    public virtual async Task Add(T obj)
    {
        try
        {
            var result = await _repository.AddAsync(obj);

            if (!result.IsSuccessful)
            {
                throw new Exception($"Failed to add {typeof(T).Name}.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to add {typeof(T).Name}. Exception: {ex.Message}");
        }
    }

    public virtual async Task Delete(Guid id)
    {
        try
        {
            var result = await _repository.DeleteAsync(id);

            if (!result.IsSuccessful)
            {
                throw new Exception($"Failed to delete {typeof(T).Name} with Id {id}.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete {typeof(T).Name} with Id {id}. Exception: {ex.Message}");
        }
    }

    public virtual async Task<T> GetById(Guid id)
    {
        try
        {
            var result = await _repository.GetByIdAsync(id);

            if (!result.IsSuccessful)
            {
                throw new Exception($"Failed to get {typeof(T).Name} by Id {id}.");
            }

            return result.Data;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get {typeof(T).Name} by Id {id}. Exception: {ex.Message}");
        }
    }

    public virtual async Task<List<T>> GetAll()
    {
        try
        {
            var result = await _repository.GetAllAsync();

            if (!result.IsSuccessful)
            {
                throw new Exception($"Failed to get all {typeof(T).Name}s.");
            }

            return result.Data;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get all {typeof(T).Name}s. Exception: {ex.Message}");
        }
    }

    public async Task<T> GetByPredicate(Func<T, bool> predicate)
    {
        try
        {
            var result = await _repository.GetByPredicateAsync(predicate);

            if (!result.IsSuccessful)
            {
                throw new Exception($"Failed to get by predicate {typeof(T).Name}s.");
            }

            return result.Data;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get by predicate {typeof(T).Name}s. Exception: {ex.Message}");
        }
    }

    public virtual async Task Update(Guid id, T obj)
    {
        try
        {
            var result = await _repository.UpdateAsync(id, obj);

            if (!result.IsSuccessful)
            {
                throw new Exception($"Failed to update {typeof(T).Name} with Id {id}.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {typeof(T).Name} with Id {id}. Exception: {ex.Message}");
        }
    }

    public string GetPasswordHash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha256.ComputeHash(bytes);
            StringBuilder builder = new StringBuilder();
            foreach (byte b in hash)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public bool VerifyHashedPassword(string hashedPassword, string password)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword)) return false;
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

        byte[] buffer4;

        byte[] src = Convert.FromBase64String(hashedPassword);
        if ((src.Length != 0x31) || (src[0] != 0))
        {
            return false;
        }

        byte[] dst = new byte[0x10];
        Buffer.BlockCopy(src, 1, dst, 0, 0x10);
        byte[] buffer3 = new byte[0x20];
        Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
        using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
        {
            buffer4 = bytes.GetBytes(0x20);
        }

        bool verify = buffer3.Equals(buffer4);

        return verify;
    }
}