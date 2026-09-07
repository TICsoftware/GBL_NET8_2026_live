using Microsoft.AspNetCore.Mvc;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic.Entity;
using GBL_MVC.Models.Manage_Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;
namespace GBL_MVC.Controllers.Manage
{
    [Authorize]
[SessionAuthorize]
    public class MainComponentDetailsController : Controller
    {
        private readonly ComponentDetail_BAL bal;
        //private readonly maincomponentmasterFields_BAL _bal;

        public MainComponentDetailsController(IConfiguration config)
        {
            bal = new ComponentDetail_BAL(config);
        }



        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(ComponentMasterField model)
        {
            if (ModelState.IsValid)
            {
                model.Created_UserID = 1; // Example
                //bal.Insert(model);
                TempData["msg"] = "Field created successfully";
                return RedirectToAction("Index");
            }
            return View(model);
        }



        public IActionResult Index(string compId, string returnUrl)
        {
            if (string.IsNullOrEmpty(compId))
                return BadRequest();

            int ctrId = Convert.ToInt32(CryptoEngine.Decrypt(compId));
            ViewBag.compId = compId;

            var model = new ComponentModel();

            model = bal.GetComponent_bal(ctrId);

            ViewBag.Fields = model.Fields;
            ViewBag.Template = model.Template;

            if (model.Fields == null || !model.Fields.Any())
            {
                model.Fields = new List<ComponentFieldDefinition>
                { 
                    new ComponentFieldDefinition()
                };
            }

            return View(model);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

    }
}
