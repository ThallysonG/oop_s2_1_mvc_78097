using Microsoft.AspNetCore.Identity;

namespace CommunityLibraryDesk.Models.ViewModels
{
    public class RoleManagementViewModel
    {
        public string NewRoleName { get; set; } = string.Empty;
        public IEnumerable<IdentityRole> Roles { get; set; } = new List<IdentityRole>();
    }
}