using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GBL_MVC.Models;
using GBL_MVC.Classes;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic;
using GBL_MVC.Helpers;

namespace GBL_MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly Homepage_BAL _bal;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _bal = new Homepage_BAL(configuration);

    }


    public IActionResult Index()
    {
       
        //string connstr1 = "user id=sa;data source=192.168.2.6;persist security info=True;initial catalog=GBL_2026;password=26%TiC@SqL20;Encrypt=True;TrustServerCertificate=True";
        string connstr1 = "user id=sa;data source=tic_dbnet;persist security info=True;initial catalog=GBL_2026;password=DB#SqL2023TiC;Encrypt=True;TrustServerCertificate=True";
        string connstrlive = "user id=sa;data source=godavaribiorefineries\\SQLEXPRESS;persist security info=True;initial catalog=GBL_2026;password=T!5h0@A7i4rD;Encrypt=True;TrustServerCertificate=True";
         string connstr2 = "user id=sa;data source=tic_dbnet;persist security info=True;initial catalog=godavaribiorefineries_2026_phase2;password=DB#SqL2023TiC;Encrypt=True;TrustServerCertificate=True";
 
        //ViewBag.encryptstr = Core_project_BusinessLogic.CryptoEngine.Encrypt(connstr);
        //  string connstr = "user id=sa;data source=49.50.111.21;persist security info=True;initial catalog=Oncopath_2026;password=D#$%%6QWe@@#4;Encrypt=True;TrustServerCertificate=True";
        string encryptstr = Core_project_BusinessLogic.CryptoEngine.Encrypt(connstr1);




        var data = _bal.GetHomepage_BAL(1, 1);
        //  ViewBag.TestFinder = _Testbal.Fetch_Tests_Details_BAL();
        return View(data);
    }

    public IActionResult indexhtml()
    {
        return View();
    }
    public IActionResult encruptIndex()
    {
        //CryptoEngine CryptoObj = new CryptoEngine();    
        // string connstr = "user id=sa;data source=49.50.111.21;persist security info=True;initial catalog=COP_2026_Live;password=D#$%%6QWe@@#4;Encrypt=True;TrustServerCertificate=True";
        string connstr = "user id=sa;data source=192.168.2.25;persist security info=True;initial catalog=cop_live_backup_13_5;password=TicWorks@2026!A;Encrypt=True;TrustServerCertificate=True";
        //"constr":  "LYHwUvWZBu-mf-hePWJNeNSK0LoqcOEVhAICADizA2lXXNZ5E7PAzkQeH4nugWKijXvgYWaznBEpMI9-jyoYGkq7py---n6luJGp5cEf2AgXxDPuO_hFd-y0eD8oNxJpcBDJCwzkxRs4wRfnYY4HsdFgxzK2dkK1z7-z8_fbw-RQmrw6_Eedj0fvm4AUppfDPFHSZFig81Sc1O0GnvT4FQ$$"


        //string encryptstr = "LYHwUvWZBu+mf+hePWJNeCH+5BHoYIHtVPrC/+vryoPfvOQSeDFoRhR1ZwqD4oPWYHp2eHNgDrY0t7DPHc5hhIr3EewLc+wDs6tnRj82flElL3ZhamThahSd1hRZnNSCG/zzKaLSIlwfUPkUF/jYCdoui/NpyQpOjU4kCPe6mQoTCvvNHLhOmtdQ6GFL3NXaGvZ2OpACpZC1m8Me21POxQ==";
        // string connstr = "user id=sa;data source=tic_dbnet;persist security info=True;initial catalog=ticcorecms_2026;password=DB#SqL2023TiC;Encrypt=True;TrustServerCertificate=True";    
        ViewBag.encryptstr = Core_project_BusinessLogic.CryptoEngine.Encrypt(connstr);
        //ViewBag.decryptstr = CryptoEngine.Decrypt(encryptstr);
        return View();
    }


    public IActionResult GetCancerDiagnosisDetails(int contentId, string groupId)
    {
        var model = _bal.GetContentComponentById_BAL(contentId, groupId);

        var group = model.Components.FirstOrDefault();

        if (group == null)
            return Json(null);

        var dict = group.Fields
            .GroupBy(x => x.FieldName)
            .ToDictionary(g => g.Key, g => g.First());

        // string GetValue(string key)
        // {
        //     return dict.ContainsKey(key) ? dict[key].FieldValue : "";
        // }

        return Json(new
        {
            popuptitle = Config_Application_Website.GetValue(dict, "Popup Display title")?
        .CleanParagraphTags(),

            popupcontent = Config_Application_Website.GetValue(dict, "popup content")?
        .CleanParagraphTags()


        });
    }




    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


}
