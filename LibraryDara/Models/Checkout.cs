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

        // fees time overdue fees. Membership card works from (since) date to (Until) date
        public DateTime Since { get; set; }
        public DateTime Until { get; set; }
    }
}
