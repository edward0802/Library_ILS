using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDara.Models
{
    public class Patron
    {
        
        public int Id { get; set; }

        [Required]
        [MaxLength(30, ErrorMessage = "First Name is too long")]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(30, ErrorMessage = "Last Name is too long")]
        public string LastName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string TelephoneNumber { get; set; }

        public virtual LibraryCard LibraryCard { get; set; } // create foreign key here, FK to LibraryCard (one-to-one)
        public virtual LibraryBranch HomeLibraryBranch { get; set; } // create foreign key here, FK to LibraryBranch (one-to-one)
    }
}
