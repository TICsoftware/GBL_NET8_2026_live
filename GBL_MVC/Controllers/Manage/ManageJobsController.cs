using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using GBL_MVC.Filters;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.Entity;
using Microsoft.AspNetCore.Components.Routing;
using GBL_MVC.Helpers;
using Core_project_BusinessLogic.Entity.Manage;



namespace GBL_MVC.Controllers
{
    // [Authorize]
    //  [SessionAuthorize]
    public class ManageJobsController : Controller
    {
        private readonly IConfiguration objconfig;
       // private int userid = 0;
        private int pageSize = 20;
        public ManageJobsController(IConfiguration configuration)
        {
            // HttpContext.Session.SetString("userid", "1");
          //  userid = Convert.ToInt32(User.GetUserId());
            //userid=1;
            objconfig = configuration;
        }
        [HttpGet]
        public IActionResult AddNew()
        {
            Career Model = new();
            return View(Model);
        }

        [HttpPost]
        public IActionResult AddNew(Career Model)
        {
            using Career_BAL objBal = new(objconfig);
            CareerMaster_CMS obj = new();
            int job_id = 0;
            try
            {
                if (ModelState.IsValid)
                {
                    obj.Role = Model.Role;
                    obj.Education = Model.Education;
                    obj.Experience = Model.Experience;
                    obj.Job_Description = Model.Job_Description;
                    obj.Location = Model.Location;
                    obj.Salary_range = Model.Salary_range;
                    obj.About_the_Role = Model.About_the_Role;
                    obj.Workmode = Model.Workmode;
                    obj.Expiry_date = Model.Expiry_date;
                    job_id = objBal.Job_careers_Insert_BAL(obj,  Convert.ToInt32(User.GetUserId()));
                    TempData["alert_message"] = "You have successfully added job.";
                    return RedirectToAction("Drafts", "ManageJobs");
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Post: ", ex);
                ModelState.AddModelError("", "Something went wrong. Please try again");
            }
            finally
            {
                objBal.Dispose();
            }
            return View(Model);
        }

        [HttpGet]
        public IActionResult Drafts()
        {
            if (TempData["alert_message"] != null)
            {
                ViewBag.alert_message = TempData["alert_message"];
            }
            CMS_job_list Model = new();
            using Career_BAL objBal = new(objconfig);
            Job_List obj = new();
            try
            {
                obj = objBal.Job_Career_list_BAL(2, 1, pageSize);
                if (obj.jobList != null && obj.jobList.Count > 0)
                {
                    Model.joblists = new();
                    foreach (var item in obj.jobList)
                    {
                        Model.joblists.Add(new Career()
                        {
                            Encrypt_job_Id = CryptoEngine.Encrypt(item.Job_Id.ToString()),
                            Role = item.Role,
                            Education = item.Education,
                            Experience = item.Experience,
                            Job_Description = item.Job_Description,
                            Location = item.Location,
                            Salary_range = item.Salary_range,
                            About_the_Role = item.About_the_Role,
                            Workmode = item.Workmode,
                            Expiry_date = item.Expiry_date,
                            Created_date = item.Created_date
                        });
                    }
                    Model.objpaging = new()
                    {
                        PageNumber = 1,
                        PageSize = pageSize,
                        TotalRecords = obj.TotalRecords
                    };
                }
                else
                {
                    Model.joblists = null;
                    Model.objpaging = null;
                }
                Model.status = 2;
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Get : ", ex);
                ModelState.AddModelError("", "Something went wrong. Please try again");
            }
            finally
            {
                objBal.Dispose();
            }
            return View(Model);
        }

        [HttpPost]
        public IActionResult Drafts(string page_no, string status, CMS_job_list Model, IFormCollection form, string action)
        {
            if (TempData["alert_message"] != null)
            {
                ViewBag.alert_message = TempData["alert_message"];
            }
            using Career_BAL objBal = new(objconfig);
            Job_List obj = new();
            int job_Id = 0;
            try
            {
                if (!string.IsNullOrWhiteSpace(action))
                {
                    var selectedValues = form["JobItems"];
                    if (action == "Activate")
                    {
                        foreach (var value in selectedValues)
                        {
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                job_Id = Convert.ToInt32(CryptoEngine.Decrypt(value));
                                objBal.Update_Job_Status_BAL(job_Id, 2);
                            }
                            ViewBag.alert_message = "Selected job(s) are activated succeessfully";
                        }
                        status = "1";
                        page_no = "1";
                        Model.status = 1;
                    }
                    else if (action == "Deactivate")
                    {
                        foreach (var value in selectedValues)
                        {
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                job_Id = Convert.ToInt32(CryptoEngine.Decrypt(value));
                                objBal.Update_Job_Status_BAL(job_Id, 1);
                            }
                            ViewBag.alert_message = "Selected job(s) are deactivated succeessfully";
                        }
                        status = "2";
                        page_no = "1";
                        Model.status = 2;
                    }

                }
                page_no = string.IsNullOrWhiteSpace(page_no) ? "1" : page_no;
                obj = objBal.Job_Career_list_BAL(string.IsNullOrWhiteSpace(status) ? 2 : Convert.ToInt32(status), Convert.ToInt32(page_no), pageSize, string.IsNullOrWhiteSpace(Model.searchquery) ? null : Model.searchquery);
                if (obj.jobList != null && obj.jobList.Count > 0)
                {
                    Model.joblists = new();
                    foreach (var item in obj.jobList)
                    {
                        Model.joblists.Add(new Career()
                        {
                            Encrypt_job_Id = CryptoEngine.Encrypt(item.Job_Id.ToString()),
                            Role = item.Role,
                            Education = item.Education,
                            Experience = item.Experience,
                            Job_Description = item.Job_Description,
                            Location = item.Location,
                            Salary_range = item.Salary_range,
                            About_the_Role = item.About_the_Role,
                            Workmode = item.Workmode,
                            Expiry_date = item.Expiry_date,
                        });
                    }
                    Model.objpaging = new()
                    {
                        PageNumber = Convert.ToInt32(page_no),
                        PageSize = pageSize,
                        TotalRecords = obj.TotalRecords
                    };
                }
                else
                {
                    Model.joblists = null;
                    Model.objpaging = null;
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Post : ", ex);
                ModelState.AddModelError("", "Something went wrong. Please try again");
            }
            finally
            {
                objBal.Dispose();
            }
            return View(Model);
        }

        [HttpGet]
        public IActionResult Edit(string Id)
        {
            Career Model = new();
            using Career_BAL objBal = new(objconfig);
            CareerMaster_CMS obj = new();
            int job_Id = 0;
            try
            {
                if (string.IsNullOrWhiteSpace(Id))
                {
                    TempData["alert_message"] = "Job not found";
                    return RedirectToAction("Drafts", "ManageJobs");
                }
                job_Id = Convert.ToInt32(CryptoEngine.Decrypt(Id.Trim()));
                obj = objBal.Job_Career_Get_BAL(job_Id);
                if (obj != null)
                {
                    Model.Encrypt_job_Id = Id.Trim();
                    Model.Role = obj.Role;
                    Model.Education = obj.Education;
                    Model.Experience = obj.Experience;
                    Model.Job_Description = obj.Job_Description;
                    Model.Location = obj.Location;
                    Model.Salary_range = obj.Salary_range;
                    Model.About_the_Role = obj.About_the_Role;
                    Model.Workmode = obj.Workmode;
                    Model.Expiry_date = obj.Expiry_date;
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Post : ", ex);
                ModelState.AddModelError("", "Something went wrong. Please try again");
            }
            finally
            {
                objBal.Dispose();
            }
            return View(Model);
        }


        [HttpPost]
        public IActionResult Edit(Career Model)
        {
            using Career_BAL objBal = new(objconfig);
            CareerMaster_CMS obj = new();
            try
            {
                if (ModelState.IsValid && !string.IsNullOrWhiteSpace(Model.Encrypt_job_Id))
                {
                    obj.Job_Id = Convert.ToInt32(CryptoEngine.Decrypt(Model.Encrypt_job_Id.Trim()));
                    obj.Role = Model.Role;
                    obj.Education = Model.Education;
                    obj.Experience = Model.Experience;
                    obj.Job_Description = Model.Job_Description;
                    obj.Location = Model.Location;
                    obj.Salary_range = Model.Salary_range;
                    obj.About_the_Role = Model.About_the_Role;
                    obj.Workmode = Model.Workmode;
                    obj.Expiry_date = Model.Expiry_date;
                    objBal.Job_careers_Update_BAL(obj,  Convert.ToInt32(User.GetUserId()));
                    TempData["alert_message"] = "You have successfully updated job.";
                    return RedirectToAction("Drafts", "ManageJobs");
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Post: ", ex);
                ModelState.AddModelError("", "Something went wrong. Please try again");
            }
            finally
            {
                objBal.Dispose();
            }
            return View(Model);
        }

    }
}
