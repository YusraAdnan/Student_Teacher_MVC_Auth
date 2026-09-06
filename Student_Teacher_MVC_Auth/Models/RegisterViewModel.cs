namespace Student_Teacher_MVC_Auth.Models
{
    public class RegisterViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Student" or "Teacher"
    }
}
