namespace WebAPIDemo.Models
{
    public interface IAppUser
    {
        string Name { get; set; }

        string Email { get; set; }

        int Id { get; set; }
    }
}