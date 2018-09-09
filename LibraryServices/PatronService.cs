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
    public class PatronService : IPatron
    {
        private LibraryContext _context;

        public PatronService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(Patron newPatron)
        {
            _context.Add(newPatron);
            _context.SaveChanges();
        }

        public Patron Get(int id)
        {
            return _context.Patrons
                .Include(p => p.LibraryCard)
                .Include(p => p.HomeLibraryBranch)
                .FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Patron> GetAll()
        {
            return _context.Patrons
                .Include(p => p.LibraryCard)
                .Include(p => p.HomeLibraryBranch);
                //.ToList();
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int patronId)
        {
            // Спочатку потрібно знайти cardId для нашого patronId - і потім по cardId знайти CheckoutHistories
            // бо CheckoutHistories в собі містять cardId, а не patronId
            //var cardId = _context.Patrons
            //    .Include(p => p.LibraryCard)
            //    .FirstOrDefault(p => p.Id == patronId)
            //    .LibraryCard.Id;
            var cardId = Get(patronId).LibraryCard.Id;

            return _context.CheckoutHistories
                .Include(ch => ch.LibraryCard)
                .Include(ch => ch.LibraryAsset)
                .Where(ch => ch.LibraryCard.Id == cardId)
                .OrderByDescending(co => co.ChekedOut); // отримаємо найновіші ChekedOut взяття книжок
        }

        public IEnumerable<Checkout> GetCheckouts(int patronId)
        {
            // ЗАмінемо ось цей код:
            //var cardId = _context.Patrons
            //    .Include(p => p.LibraryCard)
            //    .FirstOrDefault(p => p.Id == patronId)
            //    .LibraryCard.Id;

            var cardId = Get(patronId).LibraryCard.Id;

            return _context.Checkouts
                .Include(co => co.LibraryCard)
                .Include(co => co.LibraryAsset)
                .Where(co => co.LibraryCard.Id == cardId);
        }

        public IEnumerable<Hold> GetHolds(int patronId)
        {
            var cardId = Get(patronId).LibraryCard.Id;

            return _context.Holds
                .Include(h => h.LibraryCard)
                .Include(h => h.LibraryAsset)
                .Where(h => h.LibraryCard.Id == cardId)
                .OrderByDescending(h => h.HoldPlaced);
        }
    }
}
