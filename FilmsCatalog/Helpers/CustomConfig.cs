using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Helpers
{
    public class CustomConfig
    {
        public string DirectoryPath { get; set; }
        public int FilmsOnPage { get; set; }
        public string[] FileExtensions { get; set; }

    }
}
