using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDara.Models
{
    public class Checkout
    {
        public int Id { get; set; }

        [Required]
        public LibraryAsset LibraryAsset { get; set; } // one-to-one

        public LibraryCard LibraryCard { get; set; } // one-to-one

        // ПОЯСНЕННЯ: ці DateTime показують, коли (Since)наш обєкт бібліотеки (книга, відео) 
        // були взяті кимось (на Checkout - тобто читання), і коли повинні бути здані Until 
        public DateTime Since { get; set; }
        public DateTime Until { get; set; } // вказуємо до якого числа треба повернути книгу
    }
}
