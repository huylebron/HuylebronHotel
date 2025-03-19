using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelProject . Domain . Model . Users ;

namespace HotelProject.Domain.Model.Users
{
    public class AssignPermissionsViewModel
    {
        public Guid RoleId { get; set; }
        public List<PermissionViewModel> Permissions { get; set; }
    }
}
