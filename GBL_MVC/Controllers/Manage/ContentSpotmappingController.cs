using Microsoft.AspNetCore.Mvc;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic.Entity;
using System.Text;
using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;


namespace GBL_MVC.Controllers.Manage
{
    

    using System.Data;
    using GBL_MVC.Helpers;

    [Authorize]
[SessionAuthorize]
    public class ContentSpotmappingController : Controller
    {
        private readonly ContextReference_BAL _contextBal;
        private readonly Main_Spot_Template_Details_BAL _spotBal;
        private readonly ContentSpotMappingTemp_BAL _mapBal;

        private const int STATIC_CONTENT_ID = 1;

        public ContentSpotmappingController(IConfiguration config)
        {
            _contextBal = new ContextReference_BAL(config);
            _spotBal = new Main_Spot_Template_Details_BAL(config);
            _mapBal = new ContentSpotMappingTemp_BAL(config);
        }

        public IActionResult Index(string tempid, string contid, string templateid ,int openModal = 1)
        {
            int CONTENT_ID = 0;
            int template_id = 0;
              ViewBag.OpenModal = openModal;
            if (!string.IsNullOrEmpty(tempid) || (!string.IsNullOrEmpty(contid)))
            {
                if (string.IsNullOrEmpty(tempid))
                {
                    tempid = "";
                }

                if (string.IsNullOrEmpty(contid))
                {
                    contid = "0";
                }

                if (contid != "0")
                {
                    CONTENT_ID = Convert.ToInt32(Core_project_BusinessLogic.CryptoEngine.Decrypt(contid));
                }

                if (!string.IsNullOrEmpty(templateid))
                {
                    template_id = Convert.ToInt32(Core_project_BusinessLogic.CryptoEngine.Decrypt(templateid));
                }

                var data = _contextBal.GetAll_bytemplate(template_id);

                foreach (var r in data)
                {
                    r.HasData = _contextBal.HasData(r.ID); // Add flag for UI
                }

                ViewBag.ContextList = data;

                ViewBag.TemplateList = _spotBal.GetAll();
                ViewBag.MappingList = _mapBal.GetAll(CONTENT_ID, tempid);
                TempData["tempid"] = tempid;
                TempData["templateid_encrypted"] = templateid;
                TempData["content_id_encrypted"] = contid;
                return PartialView("Index");

            }
            else
            {

                return PartialView("Index");

            }
        }

        [HttpPost]
        public IActionResult Add(int spotId, int spotType)
        {
            _mapBal.Add(STATIC_CONTENT_ID, spotId, spotType, Convert.ToInt32(User.GetUserId()));
            return RedirectToAction("Index");
        }

        public IActionResult MoveUp(int id, int swapId)
        {
            _mapBal.MoveUp(id, swapId);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult AddMultiple([FromBody] AddSpotMappingVM model)
        {
            int cont_Id = 0;
            string? tempId = null;

            // 🔐 Decrypt only what is present
            if (!string.IsNullOrEmpty(model.contId) && model.contId != "0")
                cont_Id = Convert.ToInt32(CryptoEngine.Decrypt(model.contId));

            if (!string.IsNullOrEmpty(model.tempId))
                tempId = model.tempId;
            //tempId = Convert.ToInt32(CryptoEngine.Decrypt(model.tempId));

            foreach (var s in model.spots)
            {
                _mapBal.AddSpot(
                    cont_Id,        // nullable
                    tempId,        // nullable
                    s.SpotId,
                    s.SpotType

                );
            }

            return Json(new { success = true });
        }



        // [HttpPost]
        // public IActionResult AddMultiple([FromBody] List<AddSpotVM> items)
        // {
        //     int contId = 1; // for now static

        //     try
        //     {
        //         foreach (var item in items)
        //         {
        //             _mapBal.AddSpot(contId, item.SpotId, item.SpotType);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return Json(new { success = false, message = "Spot already added" });
        //     }

        //     return Json(new { success = true });
        // }

        [HttpPost]
        public IActionResult UpdateOrder([FromBody] List<SpotOrderVM> order)
        {
            int seq = 1;

            foreach (var item in order)
            {
                _mapBal.UpdateSequence(item.id, item.sequence);
                seq++;
            }

            return Json(new { success = true });
        }
        [HttpPost]
        public IActionResult DeleteMapping(int id, int contId)
        {
            _mapBal.DeleteMapping(id, contId);
            return Json(new { success = true });
        }
        [HttpPost]
        public IActionResult PreviewPage([FromBody] List<SpotPreviewVM> spots)
        {
            // Builds final ordered HTML
            // string html = _mapBal.BuildPreviewHtml(spots);
            var sb = new StringBuilder();
            foreach (var s in spots)
            {
                if (s.SpotType == 1)
                {
                    // Dynamic Context
                    string html = _contextBal.BuildFinalLayout(s.SpotId);
                    sb.Append(html);
                }

                else
                {

                    DataSet ds = _spotBal.GetSpotLayoutById(s.SpotId);
                    string finalLayout = _mapBal.BuildFinalLayout(ds);
                    sb.Append(finalLayout);



                }
            }

            return Content(sb.ToString(), "text/html");
        }



        // public string BuildPreviewHtml(List<SpotPreviewVM> spots)
        // {
        //     var sb = new StringBuilder();

        //     foreach (var s in spots)
        //     {
        //         if (s.SpotType == 1)
        //         {
        //             // Dynamic Context
        //             string html = _bal_CR.BuildFinalLayout(s.SpotId);
        //             sb.Append(html);
        //         }
        //         else
        //         {
        //             DataSet ds = _mainsoptbal.GetSpotLayoutById(s.SpotId);
        //             string finalLayout = BuildFinalLayout(ds);
        //             sb.Append(finalLayout);
        //         }
        //     }

        //     return sb.ToString();
        // }

        [HttpPost]
        public IActionResult Spot_Mapping_Temp_List(string contId, string Spottempid)
        {
            try
            {
                if (contId == "0")
                {
                    ViewBag.Spotlist = _mapBal.GetAll(0, Spottempid);
                }
                else
                {
                    int decrypt_cont_id = Convert.ToInt32(CryptoEngine.Decrypt(contId));
                    ViewBag.Spotlist = _mapBal.GetAll(decrypt_cont_id, Spottempid);
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Get", ex);
                ModelState.AddModelError("", "Something went wrong. Please try again");
            }
            return View("Spot_Mapping_Temp_List");
        }

        [HttpPost]
        public IActionResult Delete_Spot_Mapping(string contId, string Spottempid)
        {
            try
            {
                return Json(new { result = "success" });
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Get", ex);
                ModelState.AddModelError("", "Something went wrong. Please try again");
                return Json(new { result = "false" });
            }

        }






    }



}