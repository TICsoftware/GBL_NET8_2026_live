using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic.Entity;
using GBL_MVC.Models.Manage_Model;
using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;
namespace GBL_MVC.Controllers.Manage
{  [Authorize]
[SessionAuthorize]
    public class TemplateMasterController : Controller
    {

        private readonly TemplateMaster_BAL _bal;
        
        private readonly ILogger<ContentController> _logger;
        private readonly IConfiguration objconfig;
        public TemplateMasterController(ILogger<ContentController> logger, IConfiguration configuration)
        {
            _logger = logger;
            objconfig = configuration;
            _bal = new TemplateMaster_BAL(configuration);
        }

     

          // GET: /Manage/Template_Master or /Manage/Template_Master/5
        public IActionResult AddTemplateMaster(int? id)
        {
            Model_Template_Master model = new Model_Template_Master();

          ViewBag.Languages = _bal.GetLanguages();
            if (id.HasValue && id.Value > 0)
            {
                TemplateMaster entity = _bal.GetTemplateById(id.Value);

                if (entity != null)
                {
                    model.ID = entity.ID;
                    model.Language_Master_ID = entity.Language_Master_ID;
                    model.Name = entity.Name;
                    model.Status = entity.Status;
                    model.Created_UserID = "1";
                    
                    model.Updated_UserID = Convert.ToInt32("1");
                  
                }
                else
                {
                    // If no record found, show an empty form
                    TempData["AlertMessage"] = "No record found with the given ID.";
                }
            }
            else
            {
                // Create mode
                model = new Model_Template_Master
                {
                    Template_Master_Save_Action = 1 // for "Add"
                };
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTemplateMaster(Model_Template_Master model)
        {
            if (ModelState.IsValid)
            {
                var entity = new TemplateMaster
                {
                    ID = model.ID,
                    Language_Master_ID = model.Language_Master_ID,
                    Name = model.Name,
                    Status = model.Status,
                    Created_UserID = int.TryParse(model.Created_UserID, out var uid) ? uid : (int?)null
                };

                if (model.ID > 0)
                {
                    _bal.UpdateTemplate(entity);
                    TempData["AlertMessage"] = "Template updated successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    _bal.AddTemplate(entity);
                    TempData["AlertMessage"] = "Template added successfully!";
                    return RedirectToAction("Index");
                }

                 return RedirectToAction("Index");
            }
           if (!ModelState.IsValid)
            {
                 var errors = ModelState.Values.SelectMany(v => v.Errors);
            }
            TempData["AlertMessage"] = "Validation failed. Please check your input.";
            return View(model);
        }
        public IActionResult Edit(int id)
        {
            var template = _bal.GetTemplateById(id);
            ViewBag.Languages = _bal.GetLanguages();
            return View(template);
        }

        [HttpGet]
public IActionResult EditTemplateMaster(string Id)
{
    int realId = Convert.ToInt32(CryptoEngine.Decrypt(Id));

       var template = _bal.GetTemplateById(realId);
       ViewBag.Languages = _bal.GetLanguages();
 
    if (template == null)
        return NotFound();
    ModelState.Remove("ID");
    return View(template);
}

[HttpPost]


public IActionResult EditTemplateMaster(TemplateMaster model)
{
    if (ModelState.IsValid)
    {
        _bal.UpdateTemplate(model);
        ViewBag.Languages = _bal.GetLanguages();
        TempData["AlertMessage"] = "Template updated successfully!";
        return RedirectToAction("Index");
    }
     
    TempData["AlertMessage"] = "Validation failed.";
    return View(model);
}
public IActionResult Index()
{
    var templates = _bal.GetAllTemplates();
    return View(templates);
}

//   public IActionResult Delete(int id)
//         {
//             _bal.DeleteTemplate(id);
//              return RedirectToAction("Index");
//         }

         public IActionResult Deactivate(string encId)
        {
            int realId = Convert.ToInt32(CryptoEngine.Decrypt(encId));
            _bal.DeleteTemplate(realId);
             TempData["AlertMessage"] = "Template Deactivated!";
            return RedirectToAction("Index");
        }

    }




    
}
