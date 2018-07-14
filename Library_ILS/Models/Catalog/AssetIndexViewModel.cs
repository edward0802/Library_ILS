using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_ILS.Models.Catalog
{
    public class AssetIndexViewModel
    {
        public IEnumerable<AssetIndexListingViewModel> Assets { get; set; }
    }
}
