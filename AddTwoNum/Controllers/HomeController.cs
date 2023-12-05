using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using AddTwoNum.Models;
using AddTwoNum.Service;
//PM> Install-Package Microsoft.jQuery.Unobtrusive.Ajax -Version 3.2.5
namespace AddTwoNum.Controllers
{
    public class HomeController : Controller
    {
        virtualMachine vm_service;

        public HomeController()
        {
            vm_service = virtualMachine.Instance;
            
        }

        [HttpGet]
       public ActionResult Index()
       {                                   
            vm_service.startUp();
            Session["vm_memory"] = vm_service.vm_Data;

            TempData["Message"] = "Message";
            // Session["keys"] = vm_service.vm_Data.getKeys();
            return View();
        }


        [HttpGet]
        public ActionResult Executescript(string scriptArea,string ButtonType)
        {
            
            bool executeSuccess = false;

            if (ButtonType == "Reset Memory")
            {
                bool resetSuccess = false;
                ViewBag.Message = "";

                resetSuccess = vm_service.ResetMemory();
                Session["vm_memory"] = vm_service.vm_Data;
                if (resetSuccess)
                    return PartialView("Executescript", vm_service.vm_Data);
                else
                    return PartialView("Executescript", vm_service.vm_Data);
            }
            else if (ButtonType == "Execute Script")
            {
                if (scriptArea.ToString() != "")
                {

                    executeSuccess = vm_service.ExecueScript(scriptArea);

                    if (executeSuccess)
                    {
                        Session["message"] = "The last request to execute script was successful.";
                        return PartialView("Executescript", vm_service.vm_Data);
                    }
                    else
                    {
                        Session["message"] = "The last request to execute script has error in the script.";
                        return PartialView("Executescript", vm_service.vm_Data);//    May actually Redirect to Error page or return PartialView view with error
                    }
                }
                else
                {
                    Session["message"] = "The last request to execute script has no script to execute.";
                    return PartialView("Executescript", vm_service.vm_Data);
                }
            }
            else
            {
                Session["message"] = "The last request to execute script has error in the script.";
                return PartialView("Executescript", vm_service.vm_Data);//   May actually Redirect to Error page or return PartialView view with error
            }

        }
       

    }
}