using Library_ILS.Models.Branch;
using LibraryDara;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_ILS.Controllers
{
    public class BranchController : Controller
    {
        private ILibraryBranch _branch;

        public BranchController(ILibraryBranch branch)
        {
            _branch = branch;
        }

        public IActionResult Index()
        {
            var branches = _branch.GetAll().Select(branch => new BranchDetailModel
            {
                Id = branch.Id,
                Name = branch.Name,
                IsOpen = _branch.IsBranchOpen(branch.Id),
                NumberOfPatrons = _branch.GetPatrons(branch.Id).Count(),
                NumberOfAssets = _branch.GetAssets(branch.Id).Count(),
            });

            var model = new BranchIndexModel
            {
                Branches = branches
            };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var branch = _branch.Get(id);

            var model = new BranchDetailModel
            {
                Id = branch.Id,
                Address = branch.Address,
                Name = branch.Name,
                OpenDate = branch.OpenDate.ToString("yyyy:MM:dd"),
                Telephone = branch.Telephone,
                IsOpen = _branch.IsBranchOpen(branch.Id), // branch.Id == id
                Description = branch.Description,
                NumberOfPatrons = _branch.GetPatrons(id).Count(),
                NumberOfAssets = _branch.GetAssets(id).Count(),
                TotalAssetValue = _branch.GetAssets(id).Sum(asset => asset.Cost),
                ImageUrl = branch.ImageUrl,
                HoursOpen = _branch.GetBranchHours(id)
            };

            return View(model);

        }


    }
}
