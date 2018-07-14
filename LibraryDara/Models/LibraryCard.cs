using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDara.Models
{
    public class LibraryCard
    {
        public int Id { get; set; }
        public decimal Fees { get; set; } // overdue fees - просрочена оплата
        public DateTime Created { get; set; }

        public virtual IEnumerable<Checkout> Checkouts { get; set; }
    }
}
