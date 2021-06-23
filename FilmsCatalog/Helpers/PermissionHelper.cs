using FilmsCatalog.Models;
using FilmsCatalog.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Helpers
{
    public class PermissionHelper
    {
        public static bool CheckFilmCreator(User user, Film film)
        {
            if (user != null && user.Id.ToString() == film.UserId)
                return true;
            return false;            
        }
    }
}
