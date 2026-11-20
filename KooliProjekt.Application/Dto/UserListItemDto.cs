using System;
using System.Collections.Generic;

namespace KooliProjekt.Application.Dto
{
    public class UserListItemDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
    }
}
