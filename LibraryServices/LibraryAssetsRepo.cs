using LibraryDara;
using LibraryDara.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class LibraryAssetsRepo : ILibraryAssetRepo
    {
        private LibraryContext _context;

        public LibraryAssetsRepo(LibraryContext context)
        {
            _context = context;
        }

        public void Add(LibraryAsset Asset)
        {

            _context.Add(Asset); // EF сама знайде до якої таблиці добавити сутність
            _context.SaveChanges();
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            // навігаційні властивості не підтягуються, так як лінива загрузка (virtual) тому вказуємо їх ще окремо 
            // і тоді буде енергічна загрузка
            // ВАЖЛИВО: вказувати потрібно, коли це віртуальна властивість, або коли це навігаційна властивість (тобто в основі містить обєкт, який лежить в іншій таблиці)
            return _context.LibraryAssets
                .Include(asset => asset.Status)
                .Include(asset => asset.Location);
        }

        public string GetAuthorOrDirector(int id)
        {

            var isBook = _context.LibraryAssets.OfType<Book>().Where(a => a.Id == id).Any();
            var isVideo = _context.LibraryAssets.OfType<Video>().Where(a => a.Id == id).Any();
            // var isMagazine = ... etc;
            return isBook ?
                _context.Books.FirstOrDefault(b => b.Id == id).Author :
                _context.Videos.FirstOrDefault(v => v.Id == id).Director
                ?? "Nothing";
            // 2 WAY:
            //string type = GetType(id); // doesn't work because г compare int num (id == int)
            //if (type.GetType() == typeof(Book))
            //{
            //    return _context.Books.Where(b => b.Id == id).Select(b => b.Author).FirstOrDefault();
            //}
            //return _context.Videos.Where(v => v.Id == id).Select(v => v.Director).FirstOrDefault();
        }

        public LibraryAsset GetById(int id)
        {
            // don't forget about navigation properties
            return _context.LibraryAssets
                .Include(asset => asset.Status)
                .Include(asset => asset.Location)
                //.Where(a => a.Id == id)
                //.Select(a => a).FirstOrDefault();
                .FirstOrDefault(a => a.Id == id);
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
            //return _context.LibraryAssets.Where(a => a.Id == id).Select(a => a.Location).FirstOrDefault();
            // BETTER WAY: ВАЖЛИВО: ФІЧА: LINQ 
            return _context.LibraryAssets.FirstOrDefault(a => a.Id == id).Location;
            // OR:
            //return GetById(id).Location;
        }

        public string GetDeweyIndex(int id)
        {
            //Discriminator - то тут потрібно зробити перевірку
            //var isBook = _context.LibraryAssets.OfType<Book>().Where(b => b.Id == id).Any(); // можна і так поверне bool
            if (_context.Books.Any(book => book.Id == id))
            {
                return _context.Books.FirstOrDefault(b => b.Id == id).DeweyIndex;
            }
            else return string.Empty;
        }

        public string GetIsbn(int id)
        {
            //Discriminator
            if (_context.Books.Any(book => book.Id == id))
            {
                return _context.Books.FirstOrDefault(b => b.Id == id).ISBN;
            }
            else return string.Empty;
        }

        public string GetTitle(int id)
        {
            return _context.LibraryAssets.FirstOrDefault(a => a.Id == id).Title;
        }

        public string GetType(int id)
        {
            var isBook = _context.LibraryAssets.OfType<Book>().Where(b => b.Id == id).Any();
            return isBook ? "Book" : "Video";
        }
    }
}
