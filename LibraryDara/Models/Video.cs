using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDara.Models
{
    public class Video : LibraryAsset
    {
        // We don't need the Id prop< because we inherited it from LibraryAsset !!!
        [Required]
        public string Director { get; set; } // person who recorded video
    }
}
