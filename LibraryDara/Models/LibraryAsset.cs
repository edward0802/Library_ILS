using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDara.Models
{
    public abstract class LibraryAsset // Main (Parent) class for lib. assets (books, videos, magazines, cd, etc)
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public int Year { get; set; }

        // navigation 
        [Required]
        public Status Status { get; set; }

        [Required]
        public decimal Cost { get; set; }

        public string ImageUrl { get; set; }

        public int  NumberOfCopies { get; set; }

        // navigation 
        public virtual LibraryBranch Location { get; set; }
        // generate: LibraryBranchId for this Location navigation - в сумі ці дві властив. будуть зовнішнім ключем на колекцію в LibraryBranch
    }
}
