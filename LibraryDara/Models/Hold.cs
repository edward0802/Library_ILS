using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDara.Models
{
    // represents an asset (book, etc.) for which a hold has been requested (книга на яку записались в чергу)
    public class Hold
    {
        public int Id { get; set; }
        public LibraryAsset LibraryAsset { get; set; }
        public LibraryCard LibraryCard { get; set; }
        public DateTime HoldPlaced { get; set; } // дата, коли зайняли чергу (коли був зроблений запит на читання зайнятої кимось книги)
    }
}
