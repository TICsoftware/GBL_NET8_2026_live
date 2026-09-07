using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.BLL.Manage;
using Core_project_BusinessLogic.Entity.Manage;
using GBL_MVC.Models.Manage_Model;
using Microsoft.AspNetCore.Mvc;
using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;


namespace GBL_MVC.Controllers.Manage
{
    [Authorize]
    [SessionAuthorize]
    public class MediaManagerController : Controller
    {
        private readonly MediaManager_bal _repo;
        private readonly IWebHostEnvironment _env;

        public MediaManagerController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _repo = new MediaManager_bal(configuration);
            _env = env;
        }

        [HttpGet]
        public IActionResult Index(string sortOrder, string fileType, string search, int page = 1)
        {
            int pageSize = 12;

            var items = _repo.GetAll_Media_bal(
                sortOrder ?? "DateDesc",
                fileType ?? "All",
                search,
                page,
                pageSize
            );

            var vm = new MediaManagerViewModel
            {
                SortOrder = sortOrder ?? "DateDesc",
                FileType = fileType ?? "All",
                SearchTerm = search,
                CurrentPage = page,
                Items = items,
                HasMore = items.Count == pageSize
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(List<IFormFile> files, string fileType)
        {
            try
            {
                if (string.IsNullOrEmpty(fileType))
                    return Json(new { success = false, message = "Please select file type." });

                if (files == null || files.Count == 0)
                    return Json(new { success = false, message = "No files selected." });

                string folderName = fileType.ToLower() switch
                {
                    "masthead" => "masthead",
                    "thumbnail" => "thumbnail",
                    "background" => "background",
                    "icon" => "icons",
                    "pdf" => "pdf",
                    "video" => "video",
                    "doc" => "documents",
                    "investor" => "investor",
                    "press" => "pressrelease",
                    _ => "others"
                };

                string basePath = Path.Combine(_env.WebRootPath, "uploads", folderName);

                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                foreach (var file in files)
                {
                    string ext = Path.GetExtension(file.FileName);
                    string newName = file.FileName;  // use same name
                    string fullPath = Path.Combine(basePath, newName);

                    string dbPath = $"/uploads/{folderName}/{newName}";

                    var media = new MediaItem
                    {
                        media_file_name = file.FileName,
                        file_path = dbPath,
                        file_type = ext.Replace(".", ""),
                        //file_size = Math.Round(file.Length / 1024.0, 2) + " KB",
                        file_size = file.Length.ToString(),
                        Created_UserID = 1,
                        status = 2
                    };



                    int result = _repo.Add_Media_bal(media);

                    if (result <= 0)
                        return Json(new { success = false, message = "File already exists!" });

                    using var fs = new FileStream(fullPath, FileMode.Create);
                    await file.CopyToAsync(fs);
                }

                return Json(new { success = true, message = "Files uploaded successfully!" });

            }
            catch (Exception ex)
            {
                FileLogger.LogError("/Upload :", ex);
                return Json(new { success = true, message = "Files uploaded faileds!" + ex.Message.ToString() });
            }
            finally
            {
                //_repo.Dispose();
            }
        }


        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Upload(List<IFormFile> files, string fileType)
        // {
        //     int returnval = 0;
        //     if (string.IsNullOrEmpty(fileType))
        //     {
        //         TempData["Error"] = "Please select file type.";
        //         return RedirectToAction("Index");
        //     }

        //     if (files == null || files.Count == 0)
        //     {
        //         TempData["Error"] = "No files selected.";
        //         return RedirectToAction("Index");
        //     }

        //     // Map dropdown values to folders
        //     string folderName = fileType.ToLower() switch
        //     {
        //         "masthead" => "masthead",
        //         "thumbnail" => "thumbnail",
        //         "background" => "background",
        //         "icon" => "icons",
        //         "pdf" => "pdf",
        //         "doc" => "documents",
        //         "investor" => "investor",
        //         "press" => "pressrelease",
        //         _ => "others"
        //     };

        //     // Root upload path
        //     string basePath = Path.Combine(_env.WebRootPath, "uploads", folderName);

        //     // Create directory if not exists
        //     if (!Directory.Exists(basePath))
        //         Directory.CreateDirectory(basePath);

        //     foreach (var file in files)
        //     {
        //         if (file == null || file.Length == 0)
        //             continue;

        //         string ext = Path.GetExtension(file.FileName);
        //         //string newName = Guid.NewGuid() + ext;
        //         string newName = file.FileName;
        //         string fullPath = Path.Combine(basePath, newName);



        //         // Correct DB path
        //         string dbPath = $"/uploads/{folderName}/{newName}";

        //         var media = new MediaItem
        //         {
        //             media_file_name = Path.GetFileName(file.FileName), // original name
        //             file_path = dbPath,
        //             file_type = ext.Replace(".", ""),
        //             file_size = Math.Round(file.Length / 1024.0, 2) + " KB",
        //             Created_UserID = 1,
        //             status = 2
        //         };

        //         returnval = _repo.Add_Media_bal(media);

        //         if (returnval > 0)
        //         {
        //             // Save file
        //             using var fs = new FileStream(fullPath, FileMode.Create);
        //             await file.CopyToAsync(fs);
        //         }
        //         else
        //         {
        //             TempData["Message"] = "Files already exists!";
        //             return RedirectToAction("Index");
        //         }
        //     }

        //     TempData["Message"] = "Files uploaded successfully!";
        //     return RedirectToAction("Index");
        // }


        [HttpPost]
        public JsonResult UpdateMedia([FromBody] MediaTemp model)
        {
            try
            {
                int insertedMedia = _repo.Update_Media_bal(model);

                return Json(new
                {
                    success = insertedMedia > 0
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false
                });
            }
        }




        [HttpPost]
        public JsonResult AddMediaTemp([FromBody] MediaTemp model)
        {
            try
            {
                MediaTemp insertedMedia = _repo.Add_Media_temp_bal(model);

                return Json(new
                {
                    success = insertedMedia.ID > 0,
                    data = insertedMedia
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }



        // [HttpPost]
        // public JsonResult AddMediaTemp([FromBody] MediaTemp model)
        // {
        //     try
        //     {
        //         int insertedId = _repo.Add_Media_temp_bal(model);

        //         // If your BAL returns 0 or negative for failure, adjust accordingly
        //         bool isSuccess = insertedId > 0;

        //         return Json(new
        //         {
        //             success = isSuccess,
        //             id = insertedId
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return Json(new
        //         {
        //             success = false,
        //             message = ex.Message
        //         });
        //     }
        // }



        [HttpPost]
        public IActionResult DeleteMedia(int id)
        {
            try
            {
                var media = _repo.Get_MediaById_bal(id);

                if (media == null)
                {
                    return Json(new { success = false, message = "File not found" });
                }

                if (!string.IsNullOrEmpty(media.file_path))
                {
                    string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                    string sourcePath = Path.Combine(rootPath, media.file_path.TrimStart('/'));

                    // 👉 Deleted folder
                    string deletedFolder = Path.Combine(rootPath, "uploads/Deleted");
                    if (!Directory.Exists(deletedFolder))
                    {
                        Directory.CreateDirectory(deletedFolder);
                    }

                    if (System.IO.File.Exists(sourcePath))
                    {
                        string originalFileName = Path.GetFileName(media.file_path);

                        // ✅ ALWAYS NEW NAME
                        string newFileName = $"{Guid.NewGuid()}_{originalFileName}";

                        string destPath = Path.Combine(deletedFolder, newFileName);

                        // ✅ MOVE FILE WITH NEW NAME
                        System.IO.File.Move(sourcePath, destPath);

                        // ✅ UPDATE PATH (VERY IMPORTANT)
                        media.file_path = "/uploads/Deleted/" + newFileName;
                    }
                }

                // 👉 update DB
                _repo.DeleteMedia_bal(id, 1, media.file_path);

                return Json(new
                {
                    success = true,
                    message = "File moved to deleted folder"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        public IActionResult IndexPartial(string sortOrder, string fileType, string search, int page = 1)
        {
            int pageSize = 12;

            var items = _repo.GetAll_Media_bal(
                sortOrder ?? "DateDesc",
                fileType ?? "All",
                search,
                page,
                pageSize
            );

            var vm = new MediaManagerViewModel
            {
                Items = items,
                CurrentPage = page,
                HasMore = items.Count == pageSize
            };

            return PartialView("media_partial", vm);
        }


        public IActionResult SearchMedia(string sortOrder, string fileType, string search, int page = 1)
        {
            int pageSize = 12;

            var items = _repo.GetAll_Media_bal(
                sortOrder ?? "DateDesc",
                fileType ?? "All",
                search,
                page,
                pageSize
            );

            var vm = new MediaManagerViewModel
            {
                Items = items,
                CurrentPage = page,
                HasMore = items.Count == pageSize
            };

            return PartialView("_MediaGridPartial", vm);
        }


        [HttpGet]
        public IActionResult LoadMoreMedia(string sortOrder, string fileType, string search, int page = 1)
        {
            int pageSize = 12;

            var items = _repo.GetAll_Media_bal(
                sortOrder ?? "DateDesc",
                fileType ?? "All",
                search,
                page,
                pageSize
            );

            var vm = new MediaManagerViewModel
            {
                Items = items,
                CurrentPage = page,
                HasMore = items.Count == pageSize
            };

            return PartialView("_MediaGridPartial", vm); // ✅ NEW PARTIAL
        }

        // public IActionResult IndexPartial()
        // {
        //     var vm = new MediaManagerViewModel
        //     {
        //         Items = _repo.GetAll_Media_bal("DateDesc", "All", "")
        //     };

        //     return PartialView("media_partial", vm); // return partial
        // }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error");
        }

        [HttpPost]
        [Route("Media/ReplaceImage")]
        public async Task<IActionResult> ReplaceImage([FromForm] IFormFile file, [FromForm] int mediaId)
        {
            try
            {
                if (file == null)
                    return Json(new { success = false, message = "File is missing" });

                // 🔥 get existing media (for folder reference)
                var media = _repo.Get_MediaById_bal(mediaId);

                if (media == null)
                    return Json(new { success = false, message = "Media not found" });

                // 👉 get folder from existing file path
                string existingPath = media.file_path; // /uploads/masthead/abc.jpg
                string folderName = Path.GetDirectoryName(existingPath)?.Replace("\\", "/"); // /uploads/masthead

                if (string.IsNullOrEmpty(folderName))
                    folderName = "/uploads/others";

                string basePath = Path.Combine(_env.WebRootPath, folderName.TrimStart('/'));

                // ✅ ensure directory exists
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                // 🔥 generate NEW file name (important)
                string ext = Path.GetExtension(file.FileName);
                string newFileName = $"{Guid.NewGuid()}{ext}";

                string fullPath = Path.Combine(basePath, newFileName);
                string dbPath = $"{folderName}/{newFileName}".Replace("\\", "/");

                // ✅ save file
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // ✅ insert into DB (same like Upload)
                var newMedia = new MediaItem
                {
                    media_file_name = newFileName,
                    file_path = dbPath,
                    file_type = ext.Replace(".", ""),
                    file_size = file.Length.ToString(),
                    Created_UserID = 1,
                    status = 2
                };

                int result = _repo.Add_Media_bal(newMedia);

                if (result <= 0)
                    return Json(new { success = false, message = "DB insert failed" });

                // return Json(new
                // {
                //     success = true,
                //     message = "Image uploaded successfully",
                //     filePath = dbPath // 👉 return new image path (important for UI)
                // });

                return Json(new
                {
                    success = true,
                    message = "Image uploaded successfully",
                    redirectUrl = Url.Action("Index") // send URL instead
                });


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // [HttpPost]
        // public async Task<IActionResult> ReplaceImage(IFormFile file, int mediaId)
        // {
        //     if (file == null || mediaId == 0)
        //         return Json(new { success = false, message = "Invalid request" });

        //     // 🔥 get existing media from DB
        //     var media = _repo.Get_MediaById_bal(mediaId);

        //     if (media == null)
        //         return Json(new { success = false, message = "Media not found" });

        //     // full physical path
        //     string fullPath = Path.Combine(_env.WebRootPath, media.file_path.TrimStart('/'));

        //     try
        //     {
        //         // 🔥 overwrite file
        //         using (var stream = new FileStream(fullPath, FileMode.Create))
        //         {
        //             await file.CopyToAsync(stream);
        //         }

        //         return Json(new { success = true });
        //     }
        //     catch (Exception ex)
        //     {
        //         return Json(new { success = false, message = ex.Message });
        //     }
        // }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCropped(IFormFile file, string fileType)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false });

            string folderName = fileType?.ToLower() switch
            {
                "masthead" => "masthead",
                "thumbnail" => "thumbnail",
                "background" => "background",
                "icon" => "icons",
                _ => "others"
            };

            string basePath = Path.Combine(_env.WebRootPath, "uploads", folderName);

            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            string fileName = Guid.NewGuid() + ".png";
            string fullPath = Path.Combine(basePath, fileName);
            string dbPath = $"/uploads/{folderName}/{fileName}";

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var media = new MediaItem
            {
                media_file_name = fileName,
                file_path = dbPath,
                file_type = "png",
                file_size = file.Length.ToString(),
                Created_UserID = 1,
                status = 2
            };

            _repo.Add_Media_bal(media);

            return Json(new { success = true, filePath = dbPath });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplaceMedia(IFormFile file, int mediaId)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false });

            //var existing = _repo.GetById(mediaId);

            var existing = _repo.GetAll_Media_bal("DateDesc", "All", "", 1, 12);


            if (existing == null)
                return Json(new { success = false });

            // string fullPath = Path.Combine(_env.WebRootPath, existing.file_path.TrimStart('/'));


            // using (var stream = new FileStream(fullPath, FileMode.Create))
            // {
            //     await file.CopyToAsync(stream);
            // }

            return Json(new { success = true });
        }




    }
}
