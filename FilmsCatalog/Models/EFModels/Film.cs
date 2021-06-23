using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Models.EFModels
{
    public class Film
    {
        [Key]
        public Guid FilmId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ReleaseYear { get; set; }
        public string Producer { get; set; }
        public string Path { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public Film()
        {
            FilmId = new Guid();
        }
    }
}
