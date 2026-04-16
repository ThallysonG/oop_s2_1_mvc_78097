using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using oop_s2_1_mvc_78097.ViewModels;

namespace oop_s2_1_mvc_78097.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: /Admin/Roles
        public IActionResult Roles()
        {
            var viewModel = new RoleManagementViewModel
            {
                Roles = _roleManager.Roles
                    .Select(r => r.Name!)
                    .OrderBy(r => r)
                    .ToList()
            };

            return View(viewModel);
        }

        // POST: /Admin/CreateRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(RoleManagementViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.NewRoleName))
            {
                ModelState.AddModelError(nameof(viewModel.NewRoleName), "Role name is required.");
            }

            if (ModelState.IsValid)
            {
                bool roleExists = await _roleManager.RoleExistsAsync(viewModel.NewRoleName);

                if (!roleExists)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(viewModel.NewRoleName));

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    else
                    {
                        return RedirectToAction(nameof(Roles));
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(viewModel.NewRoleName), "This role already exists.");
                }
            }

            viewModel.Roles = _roleManager.Roles
                .Select(r => r.Name!)
                .OrderBy(r => r)
                .ToList();

            return View("Roles", viewModel);
        }

        // POST: /Admin/DeleteRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return RedirectToAction(nameof(Roles));
            }

            if (roleName == "Admin")
            {
                TempData["ErrorMessage"] = "The Admin role cannot be deleted.";
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