using System.Web.Mvc;
using System;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{

    public class HomeController : Controller
    {


        string defaultmagUrl = "magnet:?xt=urn:btih:d45307ed6ff65d70e55dc21fa7f3218ab968604d&tr=http%3a%2f%2ft.nyaatracker.com%2fannounce&tr=http%3a%2f%2ftracker.kamigami.org%3a2710%2fannounce&tr=http%3a%2f%2fshare.camoe.cn%3a8080%2fannounce&tr=http%3a%2f%2fopentracker.acgnx.se%2fannounce&tr=http%3a%2f%2fanidex.moe%3a6969%2fannounce&tr=http%3a%2f%2ft.acg.rip%3a6699%2fannounce&tr=https%3a%2f%2ftr.bangumi.moe%3a9696%2fannounce&tr=udp%3a%2f%2ftr.bangumi.moe%3a6969%2fannounce&tr=http%3a%2f%2fopen.acgtracker.com%3a1096%2fannounce&tr=udp%3a%2f%2ftracker.opentrackr.org%3a1337%2fannounce";
        static IntPtr windowhandle = IntPtr.Zero;

        public ActionResult Index()
        {
            /*ThunderAgentLib.Agent agent = new ThunderAgentLib.Agent();
            agent.AddTask(defaultmagUrl, "", "", "", "", 1);
            agent.CommitTasks();*/
            var windows = WindowUtils.GetAllWindows();
            return View(windows);
        }

        [HttpPost]
        public void getHandle(string hWnd)
        {
            windowhandle = new IntPtr(Int64.Parse(hWnd));
        }

        [HttpPost]
        public void generateclickforWindow(string hWnd)
        {
            windowhandle = new IntPtr(Int64.Parse(hWnd));
            WindowUtils.SetXunleiDownloadDialogToFrontAndClickSubmit(windowhandle);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page. Process Handle: " + WindowUtils.getProcessIdByWindowHandle(windowhandle);
            var elements = WindowUtils.GetAllSubWindows(windowhandle);
            return View(elements);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page. " + windowhandle;
            var res = WindowUtils.GetAllElements(windowhandle);
            return View(res);
        }

        public ActionResult Chrome()
        {
            //ViewBag.Message = "Your application description page. Process Handle: " + WindowUtils.getProcessIdByWindowHandle(windowhandle);
            var elements = WindowUtils.DisplayAllELementsFromChromeWindow(WindowUtils.getProcessIdByWindowHandle(windowhandle));
            return View(elements);
        }
    }
}
