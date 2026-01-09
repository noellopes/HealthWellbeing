namespace HealthWellbeing.ViewModel
{
    public class AdminUserListItemVM
    {
        public string UserId { get; set; } = "";
        public string Email { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Roles { get; set; } = "";
        public bool EmailConfirmed { get; set; }
    }

    public class AdminUsersListVM
    {
        public string Search { get; set; } = "";
        public List<AdminUserListItemVM> Users { get; set; } = new();
    }

    public class EditUserRolesVM
    {
        public string UserId { get; set; } = "";
        public string Email { get; set; } = "";
        public List<string> AvailableRoles { get; set; } = new();

        // apenas 1 role selecionada
        public string? SelectedRole { get; set; }
    }
}
