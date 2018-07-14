using AutoMapper;
using Library_ILS.Models.Catalog;
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

        public CatalogController(ILibraryAssetRepo repository)
        {
            _repository = repository;
            
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

    }
}
