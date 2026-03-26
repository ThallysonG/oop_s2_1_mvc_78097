using CommunityLibraryDesk.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CommunityLibraryDesk.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Roles()
        {
            var vm = new RoleManagementViewModel
            {
                Roles = _roleManager.Roles.OrderBy(r => r.Name).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(RoleManagementViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.NewRoleName))
            {
                TempData["Error"] = "Informe o nome do papel.";
                return RedirectToAction(nameof(Roles));
            }

            if (!await _roleManager.RoleExistsAsync(vm.NewRoleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(vm.NewRoleName));
            }

            return RedirectToAction(nameof(Roles));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            if (roleName == "Admin")
            {
                TempData["Error"] = "A role Admin não pode ser removida.";
                return RedirectToAction(nameof(Roles));
            }

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }

            return RedirectToAction(nameof(Roles));
        }
    }
}