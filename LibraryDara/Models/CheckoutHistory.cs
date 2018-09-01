using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDara.Models
{
    // цей клас відображає всю історію книги, хто і коли її брав, а клас Checkout вказує, на обєкти
    // які в даний момент, хтось взяв на читання, але якщо їх здають, то обєкт Checkout - ВИДАЛЯЄТЬСЯ
    public class CheckoutHistory
    {
        public int Id { get; set; }

        [Required]
        public LibraryAsset LibraryAsset { get; set; }

        [Required]
        public LibraryCard LibraryCard { get; set; }

        // показує час коли обєкт був взятий (ChekedOut) і повернутий (ChekedIn)
        [Required]
        public DateTime ChekedOut { get; set; }

        public DateTime? ChekedIn { get; set; } // ? - cause can be a null, when patron holds asset (book)
    }
}
