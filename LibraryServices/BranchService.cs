using LibraryDara;
using LibraryDara.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryServices
{
    public class BranchService : ILibraryBranch
    {
        private LibraryContext _context;

        public BranchService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(LibraryBranch newBranch)
        {
            _context.Add(newBranch);
            _context.SaveChanges();
        }

        public LibraryBranch Get(int branchId)
        {
            //return _context.LibraryBranches
            //    .Include(lb => lb.Patrons)
            //    .Include(lb => lb.LibraryAssets)
            //    .FirstOrDefault(lb => lb.Id == branchId);

            // OR: do some refactoring:
            return GetAll().FirstOrDefault(lb => lb.Id == branchId);
        }

        public IEnumerable<LibraryBranch> GetAll()
        {
            return _context.LibraryBranches
                .Include(lb => lb.Patrons)
                .Include(lb => lb.LibraryAssets);
        }

        public IEnumerable<LibraryAsset> GetAssets(int branchId)
        {
            //return _context.LibraryAssets
            //    .Include(la => la.Location)
            //    .Include(la => la.Status)
            //    .Where(la => la.Location.Id == branchId);
            // OR: 
            return _context.LibraryBranches
                .Include(lb => lb.LibraryAssets)
                .FirstOrDefault(b => b.Id == branchId)
                .LibraryAssets; // LibraryAssets - ICollection<LibraryAsset> !!!
        }

        public IEnumerable<string> GetBranchHours(int branchId)
        {
            var hours = _context.BranchHours
                 .Include(bh => bh.Branch)
                 .Where(bh => bh.Branch.Id == branchId);

            return DateHelpers.NormalizeBranchHours(hours); // DateHelpers - OUR class which we implement in LibraryServices
        }

        public IEnumerable<Patron> GetPatrons(int branchId)
        {
            return _context.LibraryBranches
                .Include(lb => lb.Patrons)
                .FirstOrDefault(lb => lb.Id == branchId)
                .Patrons;
        }

        public bool IsBranchOpen(int branchId)
        {
            var nowHour = DateTime.Now.Hour;
            var nowDayOfWeek = (int)DateTime.Now.DayOfWeek + 1; // because it starts from 0 to 6
            var hours = _context.BranchHours
                 .Include(bh => bh.Branch)
                 .Where(bh => bh.Branch.Id == branchId);
            var daysHours = hours.FirstOrDefault(h => h.DayOfWeek == nowDayOfWeek);

            var isOpen = nowHour < daysHours.CloseTime && nowHour > daysHours.OpenTime;

            return isOpen;

        }
    }
}
