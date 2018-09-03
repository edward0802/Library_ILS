using AutoMapper;
using Library_ILS.Models.Catalog;
using Library_ILS.Models.CheckoutModels;
using LibraryDara;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_ILS.Controllers
{
    public class CatalogController : Controller
    {
        private ILibraryAssetRepo _repository;
        private ICheckout _checkout;

        public CatalogController(ILibraryAssetRepo repository, ICheckout checkout)
        {
            _repository = repository;
            _checkout = checkout;


        }

        
        public IActionResult Index()
        {
            var assetsModel = _repository.GetAll();

            // Select Воно змепить наші елементи в колоекції до типу нашої моделі AssetIndexViewModel, 
            // МОжна було б це зробити через AutoMapper 
            var resultList = assetsModel
                .Select(asset => new AssetIndexListingViewModel()
                {
                    Id = asset.Id,
                    ImageUrl = asset.ImageUrl,
                    AuthorOrDirector = _repository.GetAuthorOrDirector(asset.Id),
                    DeweyCallNumber = _repository.GetDeweyIndex(asset.Id),
                    Title = asset.Title,
                    Type = _repository.GetType(asset.Id)
                });
            //OR
            //.Select(asset => Mapper.Map<AssetIndexListingViewModel>(asset) ); // Але це не вийде, бо потрібно ще методи викликати для деяких властивостей, тому краще попредній варіант
            // хоча можна скомбінувати і те і те

            var model = new AssetIndexViewModel()
            {
                Assets = resultList
            };


            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var asset = _repository.GetById(id);

            // ВАЖЛИВО: !!! так як наш .GetCurrentHolds(id) повертає колекцію  IEnumerable<Hold>
            // а нам потрібно IEnumerable<AssetHoldModel> - то МИ ПОМІЩАЄМО наші обєкти колекції
            // в нові обєкти AssetHoldModel в методі Select
            var currentHolds = _checkout.GetCurrentHolds(id)
                .Select(hold => new AssetHoldModel
                {
                    // ВАЖЛИВО: тут в метод передається hold.Id !!! тобто кожен елемент з колекції
                    HoldPlaced = _checkout.GetCurrentHoldPlaced(hold.Id).ToString("d"),
                    PatronName = _checkout.GetCurrentHoldPatronName(hold.Id)
                });

            var model = new AssetDetailModel
            {
                AssetId = id,
                Title = asset.Title,
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageUrl = asset.ImageUrl,
                AuthorOrDirector = _repository.GetAuthorOrDirector(id),
                CurrentLocation = _repository.GetCurrentLocation(id).Name,
                DeweyCallNumber = _repository.GetDeweyIndex(id),
                ISBN = _repository.GetIsbn(id),
                // допишемо новий функціонал для нашого репозитарію, для керування checkout system and hold system
                CheckoutHistory = _checkout.GetCheckoutHistory(id),
                LatestCheckout = _checkout.GetLatestCheckout(id),
                PatronName = _checkout.GetCurrentCheckoutPatron(id),
                CurrentHolds = currentHolds

            };
            return View(model);
        }

        // ВАЖЛИВО !!! :цей метод Checkout снабжає наш допоміжний метод PlaceCheckout нашою моделлю CheckoutModel, 
        // в якій міститься наш AssetId, який потім в формі через: ( <input type="hidden" asp-for="AssetId"/> ) 
        // передається в Post метод 
        public IActionResult Checkout(int id)
        {
            var asset = _repository.GetById(id);

            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkout.IsCheckedOut(id)
            };

            return View(model);
        }

        // ВАЖЛИВО: передаємо саме id, бо EF очікує саме таку назву, бо ми писали в атрибуті asp-route-id, а не asp-route-asserId 
        public IActionResult CheckIn(int id) // бо в Detail.cshtml ми викликаємо CheckIn 
        {
            _checkout.CheckInItem(id);
            return RedirectToAction(nameof(CatalogController.Detail), new { id = id });
        }

        public IActionResult Hold(int id)
        {
            var asset = _repository.GetById(id);

            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkout.IsCheckedOut(id),
                HoldCount = _checkout.GetCurrentHolds(id).Count()
            };

            return View(model);
        }

        // ВАЖЛИВО: саме int id, бо ми написали asp-route-id, тому середовище його і очікує 
        public IActionResult MarkFound(int id)
        {
            _checkout.MarkFound(id);
            return RedirectToAction(nameof(CatalogController.Detail), new { id = id });
        }

        public IActionResult MarkLost(int id)
        {
            _checkout.MarkLost(id);
            return RedirectToAction(nameof(CatalogController.Detail), new { id = id });
        }

        // коли ми будемо на сторінці Checkout, то що ми хочемо зробити? ми хочемо встановити наш Checkout, 
        // і для цього зробимо метод PlaceCheckout
        [HttpPost] // а цей метод ізвлекає потрібні параметри (assetId та libraryCardId) з переданої йому моделі CheckoutModel з метода Checkout 
        public IActionResult PlaceCheckout(int assetId, int libraryCardId)
        {
            // так як ми будемо оновлювати БД, то викличемо CheckInItem в якому викликається .Update БД
            // тобто змінемо стан обєкта
            _checkout.CheckOutItem(assetId, libraryCardId);
            return RedirectToAction(nameof(CatalogController.Detail), new { id = assetId}); // передаємо обовязковий параметр id метода Detail
        }

        [HttpPost]
        public IActionResult PlaceHold(int assetId, int libraryCardId)
        {
            _checkout.PlaceHold(assetId, libraryCardId);
            return RedirectToAction(nameof(CatalogController.Detail), new { id = assetId });
        }


    }
}
