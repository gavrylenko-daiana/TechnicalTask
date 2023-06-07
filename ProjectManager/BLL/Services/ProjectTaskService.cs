using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class ProjectTaskService : GenericService<ProjectTask>, IProjectTaskService
{ 
    public ProjectTaskService(IRepository<ProjectTask> repository) : base(repository)
    {
    }
    
    public async Task<bool> ProjectTaskIsAlreadyExist(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput)) throw new ArgumentNullException(nameof(userInput));
        
        var check = (await GetAll()).Any(p => p.Name == userInput);

        return check;
    }
    
    public async Task AddFileToDirectory(string sourceFilePath, ProjectTask projectTask)
    {
        const string pathToFolder = "/Users/dayanagavrylenko/Desktop/Web/dotNet/github/Technical-Task/ProjectManager/UI/bin/Debug/net6.0/DirectoryForUser";

        if (!Directory.Exists(pathToFolder))
        {
            Directory.CreateDirectory(pathToFolder);
        }

        string fileName = Path.GetFileName(sourceFilePath);
        string destinationFilePath = Path.Combine(pathToFolder, fileName);

        if (File.Exists(destinationFilePath))
        {
            int count = 1;
            string fileNameOnly = Path.GetFileNameWithoutExtension(destinationFilePath);
            string extension = Path.GetExtension(destinationFilePath);
            string newFullPath = destinationFilePath;
            
            while (File.Exists(newFullPath))
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(pathToFolder, tempFileName + extension);
            }
            
            File.Copy(sourceFilePath, newFullPath);
            projectTask.UploadedFiles.Add(newFullPath);
            await Update(projectTask.Id, projectTask);
        }
        else
        {
            File.Copy(sourceFilePath, destinationFilePath);
            projectTask.UploadedFiles.Add(destinationFilePath);
            await Update(projectTask.Id, projectTask);
        }
    }

    public async Task<List<ProjectTask>> GetTasksByDeveloper(User developer)
    {
        var tasks = (await GetAll()).Where(t => t.Developer != null && t.Developer.Id == developer.Id).ToList();

        return tasks;
    }
    
    public async Task<List<ProjectTask>> GetTasksByTester(User tester)
    {
        var tasks = (await GetAll()).Where(t => t.Tester != null && t.Tester.Id == tester.Id).ToList();

        return tasks;
    }

    public async Task<List<ProjectTask>> GetWaitTasksByTester(User tester)
    {
        var tasks = (await GetAll()).Where(t => t.Tester != null
                                                && t.Tester.Id == tester.Id
                                                && t.Progress == Progress.WaitingTester).ToList();

        return tasks;
    }

    public Task<List<ProjectTask>> GetApproveTasks(Project project)
    {
        var approveTasks = project.Tasks.Where(t => t.Progress == Progress.CompletedTester).ToList();
        
        return Task.FromResult(approveTasks);
    }

    public async Task<Priority> GetPriority(int choice, Priority priority)
    {
        try
        {
            priority = choice switch
            {
                1 => Priority.Urgent,
                2 => Priority.High,
                3 => Priority.Medium,
                4 => Priority.Low,
                5 => Priority.Minor,
            };

            return priority;
        }
        catch
        {
            Console.WriteLine("Such a type of priority does not exist!");
            
            return 0;
        }
    }
    
    public async Task<ProjectTask> GetTaskByName(string taskName)
    {
        ProjectTask task = await GetByPredicate(t => t.Name == taskName);
        
        if (task == null) throw new ArgumentNullException(nameof(task));

        return task;
    }
}