
using FilmsCatalog.Models.EFModels;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace FilmsCatalog.Models.ViewModels
{
    public class FilmViewModel
    {
        public Guid FilmId { get; set; }
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Required]
        [Range(1, 9999)]
        [Display(Name = "Год выпуска")]
        public int ReleaseYear { get; set; }
        [Required]
        [Display(Name = "Режиссер")]
        public string Producer { get; set; }
        [Required]
        public IFormFile Poster { get; set; }
        public string Path { get; set; }
        public FilmViewModel() { }
        public FilmViewModel(Film film)
        {
            FilmId = film.FilmId;
            Name = film.Name;
            Description = film.Description;
            ReleaseYear = film.ReleaseYear;
            Producer = film.Producer;
            Path = film.Path;
        }
    }
}
