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
    public class CheckoutService : ICheckout
    {

         private LibraryContext _context;

        public CheckoutService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(Checkout newCheckout)
        {
            _context.Add(newCheckout);
            _context.SaveChanges();
        }

        // коли здають книгу
        public void CheckInItem(int assetId)
        {
            var now = DateTime.Now;

            var item = _context.LibraryAssets
                .FirstOrDefault(i => i.Id == assetId);

            _context.Update(item); // потрібно перевести обєкт в стан Modified, щоб можна було змінювати властивості сутності

            // 1. далі нам потрібно видалити відповідний обєкт Checkout, бо ми здали книгу
            RemoveExistingCheckouts(assetId);

            // 2. закрити історію CheckoutHistory
            CloseExistingCheckoutHistory(assetId, now);

            // 3. переглянемо існуючу чергу (hold) на наш обєкт 
            var currentHolds = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == assetId);

            // 3.1. і якщо є черга, то перевести взяття книги (Checkout) на наступного в черзі
            if (currentHolds.Any()) // currentHolds != null
            {
                // метод, який створює чекаут (тобто передає книгу) 
                // наступному члену клуба, що найперший в черзі (hold)
                CheckoutToearliestHold(assetId, currentHolds);
                return; // ВАЖЛИВО: return !!! щоб воно вийшло з метода, бо інкаше піде далі і оновить 
                        // нашу книгу до статусу Available, а це не правильно

                //// ------------- to CheckoutToearliestHold method ----------------
                //var earliesthold = currentHolds
                //    //.Include(h => h.LibraryCard)
                //    //.Include(h => h.LibraryAsset)
                //    .OrderBy(h => h.HoldPlaced)
                //    .FirstOrDefault();

                ////var checkout = new Checkout
                ////{
                ////    LibraryAsset = item,
                ////    LibraryCard = earliesthold.LibraryCard,
                ////    Since = DateTime.Now
                ////};

                //var card = earliesthold.LibraryCard;

                //_context.Remove(earliesthold); // потрібно видалати обєкт, бо він вже не в черзі, а отримав книгу

                ////_context.Add(checkout);

                //_context.SaveChanges();
                //CheckOutItem(id, card.Id);
                //// ---------------------------------------------------------------


            }

            // 3.2. інакше, якщо черги немає, то зробимо статус нашої книги як доступний (Available)
            UpdateAssetStatus(assetId, "Available");


            _context.SaveChanges();
        }

        // коли беруть книгу CheckoutItem - взята книга
        public void CheckOutItem(int assetId, int libraryCardId)
        {

            var now = DateTime.Now; // щоб було однакове для всього методу 
            // перевіримо, чи книга вільна, щоб можна було її видати (на Checkout)
            if (IsCheckedOut(assetId))
            {
                return;
                // add logic to handle feedback to the user
            }

            var item = _context.LibraryAssets
                .FirstOrDefault(i => i.Id == assetId);

            // нам потрібно оновити статус обєкта книги
            //_context.Update(item); // use our method:
            UpdateAssetStatus(assetId, "Checked Out");

            // далі отримаємо картку члена клубу, з списком його Checkout'ів 
            // щоб добавити саме йому новий Checkout
            var libraryCard = _context.LibraryCards
                .Include(card => card.Checkouts)
                .FirstOrDefault(card => card.Id == libraryCardId);

            var checkout = new Checkout
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDefaultCheckoutTime(now) // вказуємо до якого числа треба повернути книгу
            };

            _context.Add(checkout);

            // Далі потрібно додати нову CheckoutHistory
            var checkoutHistory = new CheckoutHistory
            {
                ChekedOut = now,
                LibraryAsset = item,
                LibraryCard = libraryCard
            };

            _context.Add(checkoutHistory);
            _context.SaveChanges();

        }

        private void CheckoutToearliestHold(int assetId, IQueryable<Hold> currentHolds)
        {
            var earliestHold = currentHolds
                .OrderBy(h => h.HoldPlaced)
                .FirstOrDefault();

            var card = earliestHold.LibraryCard;

            _context.Remove(earliestHold);
            _context.SaveChanges();
            CheckOutItem(assetId, card.Id);
        }



        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30); // на 30 днів можна взяти книгу
        }

        public bool IsCheckedOut(int assetId)
        {
            // щоб перевірити чи книга вільна, перевіримо чи вона є в списку виданих (в списку Checkout'ів)
            return _context.Checkouts
                .Where(co => co.LibraryAsset.Id == assetId)
                .Any(); // Any - повертає bool, return true - якщо елемент є в списку Checkout


        }

        // цей метод поверне всі обєкти Checkout з БД (тобто поверне всіх, хто вязв книгу)
        public IEnumerable<Checkout> GetAll()
        {
            return _context.Checkouts;
        }

        public Checkout GetById(int id)
        {
            return _context.Checkouts
                .FirstOrDefault(checkout => checkout.Id == id); // or GetAll().FirstOrDefault(...)
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int id)
        {
            return _context.CheckoutHistories
                .Include(history => history.LibraryAsset) // щоб воно підтянуло обєкт LibraryAsset
                .Include(history => history.LibraryCard) // щоб воно підтянуло обєкт LibraryCard
                .Where(history => history.LibraryAsset.Id == id);
        }


        public string GetCurrentHoldPatronName(int holdId)
        {
            var hold = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.Id == holdId);

            var cardId = hold?.LibraryCard.Id;
            var patron = _context.Patrons.Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return $"{patron?.FirstName} {patron?.LastName}";
        }

        public DateTime GetCurrentHoldPlaced(int holdId)
        {
            return  _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.Id == holdId)
                .HoldPlaced;
        }

        // добавити людину в чергу
        public void PlaceHold(int assetId, int libraryCardId)
        {
            var now = DateTime.Now;

            var asset = _context.LibraryAssets
                .Include(la => la.Status)
                .FirstOrDefault(a => a.Id == assetId);
            var card = _context.LibraryCards
                .FirstOrDefault(c => c.Id == libraryCardId);

            // at first - we'll check does it available 
            if (asset.Status.Name == "Available")
            {
                UpdateAssetStatus(assetId, "On Hold");
            }

            var hold = new Hold
            {
                HoldPlaced = now,
                LibraryAsset = asset,
                LibraryCard = card
            };

            _context.Add(hold);
            _context.SaveChanges();
        }


        // в цьому методі повертаємо колекцію всіх запитів (hold) від членів клубу
        // на певну книгу (по id книги) 
        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            return _context.Holds
                .Include(h => h.LibraryAsset) 
                .Where(h => h.LibraryAsset.Id == id); 
        }

        // показує, хто останній взяв книгу
        public Checkout GetLatestCheckout(int id)
        {
            return _context.Checkouts
                .Where(c => c.LibraryAsset.Id == id)
                .OrderByDescending(c => c.Since)
                .FirstOrDefault();
        }




        public string GetCurrentPatron(int id)
        {
            throw new NotImplementedException();
        }

        // ці два методи MarkFound та MarkLost - оновлюють статус (Status) обєкта бібліотеки (книги і тд)
        public void MarkFound(int id)
        {
            var now = DateTime.Now;

            // так як ми часто юзаємо Update статусу обєктів, то створимо метод для цього:
            UpdateAssetStatus(id, "Available");
            // ЗАМІСТЬ ЦЬОГО:
            //var item = _context.LibraryAssets
            //    .FirstOrDefault(i => i.Id == id);
            //_context.Update(item);
            //item.Status = _context.Statuses.FirstOrDefault(s => s.Name == "Available");


            // put code below in method RemoveExistingCheckouts(int id)
            //// remove any existing checkouts on the item:
            RemoveExistingCheckouts(id);

            //var checkout = _context.Checkouts
            //    .FirstOrDefault(co => co.LibraryAsset.Id == id);

            //if (checkout != null)
            //    _context.Remove(checkout);


            // close any existing checkout history:
            CloseExistingCheckoutHistory(id, now);
            //// && h.ChekedIn == null - тобто обєкт, який не був зданий (бо він був загублений, тому в нього ChekedIn == null
            //var history = _context.CheckoutHistories
            //    .FirstOrDefault(h => h.LibraryAsset.Id == id && h.ChekedIn == null);

            //// оновимо історію цієї книги (чи відео) дописавши час її повернення ChekedIn
            //if (history != null)
            //{
            //    _context.Update(history);
            //    history.ChekedIn = now;
            //}


            _context.SaveChanges();
        }

        private void UpdateAssetStatus(int id, string status)
        {
            var item = _context.LibraryAssets
                   .FirstOrDefault(i => i.Id == id);

            _context.Update(item);

            item.Status = _context.Statuses.FirstOrDefault(s => s.Name == status);
        }

        private void CloseExistingCheckoutHistory(int id, DateTime now)
        {
            // close any existing checkout history:
            // && h.ChekedIn == null - тобто обєкт, який не був зданий (бо він був загублений, тому в нього ChekedIn == null
            var history = _context.CheckoutHistories
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.LibraryAsset.Id == id && h.ChekedIn == null);

            // оновимо історію цієї книги (чи відео) дописавши час її повернення ChekedIn
            if (history != null)
            {
                _context.Update(history);
                history.ChekedIn = now;
            }
        }

        private void RemoveExistingCheckouts(int id)
        {
            // remove any existing checkouts on the item:
            var checkout = _context.Checkouts
                .Include(co => co.LibraryAsset)
                .Include(co => co.LibraryCard)
                .FirstOrDefault(co => co.LibraryAsset.Id == id);

            if (checkout != null)
                _context.Remove(checkout);
        }

        public void MarkLost(int id)
        {
            // ЗАМІНЕМО КОД: на функції:
            // --------
            //var item = _context.LibraryAssets
            //    .FirstOrDefault(i => i.Id == id);

            //_context.Update(item); // EF буде відсліжувати всі властивості item і вони будуть 
            //// позначені як EntityState.Modified  і воно оновить сутність, коли буде викличено SaveChanges
            //item.Status = _context.Statuses.FirstOrDefault(s => s.Name == "Lost");

            //var item.Status = _context.Statuses.FirstOrDefault(s => s.Name == "Lost");
            // --------

            UpdateAssetStatus(id, "Lost");
            _context.SaveChanges();
        }

        public string GetCurrentCheckoutPatron(int assetId)
        {
            var checkout = GetCheckoutByAssetId(assetId);
            if (checkout == null)
            {
                return ""; // "Not checked out."
            }

            var cardId = checkout.LibraryCard.Id;
            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return $"{patron?.FirstName} {patron?.LastName}";
        }

        private Checkout GetCheckoutByAssetId(int assetId)
        {
            return _context.Checkouts // return checkout
                .Include(co => co.LibraryAsset)
                .Include(co => co.LibraryCard)
                .FirstOrDefault(co => co.LibraryAsset.Id == assetId);
        }
    }
}
