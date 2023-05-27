namespace Core.Enums;

public enum UserRole
{
    StakeHolder, // Добавляет проект, приймає фінальне рішення, чи готовий проект чи ні
    Tester, // Тестирует проект перед StakeHolder (если таск в статусе completed то проходит тестирование)
    Developer, // работает с таксами // public уведомления  (может получать уведомления)
    User
}
