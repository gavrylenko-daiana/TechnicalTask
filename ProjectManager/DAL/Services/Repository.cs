using Newtonsoft.Json;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace DAL.Services
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly string _filePath;

        public Repository(string filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{typeof(T).Name}.json");
            EnsureFileExists();
        }

        public async Task<Result<List<T>>> GetAllAsync(int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var items = await GetAllItemsAsync();
                var pagedItems = items
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new Result<List<T>>(isSuccessful: true, data: pagedItems);
            }
            catch (Exception ex)
            {
                return new Result<List<T>>(isSuccessful: false, message: $"Failed to get all items. Exception: {ex.Message}");
            }
        }

        public async Task<Result<T>> GetByIdAsync(Guid id)
        {
            return await GetByPredicateAsync(item => item.Id.Equals(id));
        }

        public async Task<Result<T>> GetByPredicateAsync(Func<T, bool> predicate)
        {
            try
            {
                var item = (await GetAllItemsAsync()).FirstOrDefault(predicate);

                if (item == null)
                {
                    return new Result<T>(isSuccessful: false, message: "Item not found.");
                }

                return new Result<T>(isSuccessful: true, data: item);
            }
            catch (Exception ex)
            {
                return new Result<T>(isSuccessful: false, message: $"Failed to get item. Exception: {ex.Message}");
            }
        }

        public async Task<Result<bool>> AddAsync(T obj)
        {
            try
            {
                var items = await GetAllItemsAsync();
                items.Add(obj);
                await SaveItemsAsync(items);

                return new Result<bool>(isSuccessful: true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(isSuccessful: false, message: $"Failed to add item. Exception: {ex.Message}");
            }
        }

        public async Task<Result<bool>> UpdateAsync(Guid id, T updatedObj)
        {
            try
            {
                var items = await GetAllItemsAsync();
                int index = items.FindIndex(item => item.Id.Equals(id));

                if (index != -1)
                {
                    items[index] = updatedObj;
                    await SaveItemsAsync(items);

                    return new Result<bool>(isSuccessful: true);
                }

                return new Result<bool>(isSuccessful: false, message: "Object with the specified Id not found.");
            }
            catch (Exception ex)
            {
                return new Result<bool>(isSuccessful: false, message: $"Failed to update item. Exception: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var items = await GetAllItemsAsync();
                int index = items.FindIndex(item => item.Id.Equals(id));

                if (index != -1)
                {
                    items.RemoveAt(index);
                    await SaveItemsAsync(items);

                    return new Result<bool>(isSuccessful: true);
                }

                return new Result<bool>(isSuccessful: false, message: "Object with the specified Id not found.");
            }
            catch (Exception ex)
            {
                return new Result<bool>(isSuccessful: false, message: $"Failed to delete item. Exception: {ex.Message}");
            }
        }
        
        private void EnsureFileExists()
        {
            if (!File.Exists(_filePath))
            {
                WriteToFileAsync(new List<T>()).GetAwaiter().GetResult();
            }
        }

        private async Task<List<T>> GetAllItemsAsync()
        {
            try
            {
                using StreamReader file = File.OpenText(_filePath);
                using JsonTextReader reader = new JsonTextReader(file);
                JsonSerializer serializer = new JsonSerializer();
                
                return await Task.Run(() => serializer.Deserialize<List<T>>(reader));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get items from the file. Exception: {ex.Message}");
            }
        }

        private async Task WriteToFileAsync(List<T> items)
        {
            try
            {
                using StreamWriter file = File.CreateText(_filePath);
                using JsonTextWriter writer = new JsonTextWriter(file)
                {
                    Formatting = Formatting.Indented
                };
                
                JsonSerializer serializer = new JsonSerializer();
                await Task.Run(() => serializer.Serialize(writer, items));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to write items to the file. Exception: {ex.Message}");
            }
        }

        private async Task SaveItemsAsync(List<T> items)
        {
            await WriteToFileAsync(items);
        }
    }
}
