using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GBL_MVC.Models.Manage_Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;
namespace GBL_MVC.Controllers.Manage
{

[Authorize]
[SessionAuthorize]
    public class CounterCardViewController : Controller
    {
        private readonly ILogger<CounterCardViewController> _logger;

        public CounterCardViewController(ILogger<CounterCardViewController> logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {
            var model = new OurStoryViewModel();

            // If no data from DB
            if (model.CounterCards == null || !model.CounterCards.Any())
            {
                model.CounterCards = new List<CounterCardViewModel>
                {
                    new CounterCardViewModel()
                };
            }

            return View(model);
        }


        // public async Task<IActionResult> Index()
        // {
        //     var ourStory = await _context.OurStories
        //         .Include(x => x.CounterCards)
        //         .FirstOrDefaultAsync();

        //     var model = new OurStoryViewModel();

        //     if (ourStory != null)
        //     {
        //         model.CounterCards = ourStory.CounterCards
        //             .OrderBy(x => x.DisplayOrder)
        //             .Select(x => new CounterCardViewModel
        //             {
        //                 Id = x.Id,
        //                 Title = x.Title,
        //                 Value = x.Value,
        //                 Postfix = x.Postfix,
        //                 DisplayOrder = x.DisplayOrder
        //             }).ToList();
        //     }

        //     return View(model);
        // }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        public IActionResult LoadCounterCard(int index)
        {
            var model = new CounterCardViewModel
            {
                DisplayOrder = index
            };

            ViewData.TemplateInfo.HtmlFieldPrefix = $"CounterCards[{index}]";

            return PartialView("_CounterCardPartial", model);
        }

        [HttpPost]
        public IActionResult SaveOurStory(OurStoryViewModel model)
        {
            // model.CounterCards will bind correctly
            return View(model);
        }

    }
}