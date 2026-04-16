namespace oop_s2_1_mvc_78097.ViewModels
{
    public class RoleManagementViewModel
    {
        public string NewRoleName { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new();
    }
}