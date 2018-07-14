using LibraryDara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDara
{
    public interface ILibraryAssetRepo
    {
        IEnumerable<LibraryAsset> GetAll();
        LibraryAsset GetById(int id); // pass primary key

        void Add(LibraryAsset Asset);
        string GetAuthorOrDirector(int id);
        string GetDeweyIndex(int id);
        string GetType(int id); // book or video?
        string GetTitle(int id);
        string GetIsbn(int id);

        LibraryBranch GetCurrentLocation(int id);
    }
}
