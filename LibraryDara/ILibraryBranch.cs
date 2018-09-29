using LibraryDara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDara
{
    public interface ILibraryBranch
    {
        IEnumerable<LibraryBranch> GetAll();
        IEnumerable<Patron> GetPatrons(int branchId); // get users from particular branch
        IEnumerable<LibraryAsset> GetAssets(int branchId);
        LibraryBranch Get(int branchId);
        void Add(LibraryBranch newBranch);
        IEnumerable<string> GetBranchHours(int branchId);
        bool IsBranchOpen(int branchId);
    }
}
