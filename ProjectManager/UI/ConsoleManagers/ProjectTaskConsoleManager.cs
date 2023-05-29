using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class ProjectTaskConsoleManager : ConsoleManager<IProjectTaskService, ProjectTask>, IConsoleManager<ProjectTask>
{
    private ProjectConsoleManager _projectManager;
    public ProjectTaskConsoleManager(IProjectTaskService service) : base(service)
    {
    }

    public async Task<List<ProjectTask>> CreateTaskAsync(Project project)
    {
        List<ProjectTask> tasks = new List<ProjectTask>();
        string exit = String.Empty;
        
        while (exit != "exit")
        {
            Console.WriteLine("Create task");
            Console.Write("Please, write name of task.\nName: ");
            string taskName = Console.ReadLine()!;
            
            Console.Write("Please, write description.\nDescription: ");
            string taskDescription = Console.ReadLine()!;

            Console.Write("Enter a due date for the task.\n" +
                          "(Please note that the deadline should not exceed the deadline for the implementation of the project itself. Otherwise, the term will be set automatically - the maximum.)\n" +
                          "Due date (dd.MM.yyyy): ");
            string[] date = Console.ReadLine()!.Split('.');
            DateTime enteredDate = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));

            DateTime term = enteredDate <= project.DueDates.Date
                ? enteredDate
                : project.DueDates.Date;

            var priority = Priority.Low;
            Console.WriteLine("Enter role for user: \n1) Urgent; 2) High; 3) Medium; 4) Low; 5) Minor;");
            int choice = int.Parse(Console.ReadLine()!);
            
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
            }
            catch
            {
                Console.WriteLine("Such a type of subscription does not exist!");
            }
            
            

            await CreateAsync(new ProjectTask
            {
                Name = taskName,
                Description = taskDescription,
                DueDates = term,
                Priority = priority,
            });
        }

        return tasks;
    }

    public override Task PerformOperationsAsync()
    {
        throw new NotImplementedException();
    }
    
}