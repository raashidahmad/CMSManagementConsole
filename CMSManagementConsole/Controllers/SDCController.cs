using CMSManagementConsole.Helpers;
using CMSManagementConsole.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using PagedList;
using System.Net;

namespace CMSManagementConsole.Controllers
{
    [AuthorizationFilter]
    public class SDCController : Controller
    {
        private string apiBaseUrl = "";
        HttpClient client;
        int pageSize;

        public SDCController()
            {
            string accessToken = null;
            if (System.Web.HttpContext.Current.Session["accessToken"] != null)
                {
                accessToken = System.Web.HttpContext.Current.Session["accessToken"].ToString();
                }
            apiBaseUrl = WebConfigurationManager.AppSettings["ApiBaseUrl"];
            pageSize = Convert.ToInt32(WebConfigurationManager.AppSettings["PageSize"]);
            client = new HttpClient();
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            }

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
            {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_asc" : "";
            if (searchString != null)
                {
                page = 1;
                }
            else
                {
                searchString = currentFilter;
                }

            ViewBag.CurrentFilter = searchString;

            List<SDCView> sdcs = new List<SDCView>();
            var responseMessage = await client.GetAsync(apiBaseUrl + "/SDC");
            if (responseMessage.IsSuccessStatusCode)
                {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                sdcs = JsonConvert.DeserializeObject<List<SDCView>>(responseData);
                }

            if (searchString != null)
                {
                sdcs = (from sdc in sdcs
                              where sdc.Title.ToLowerInvariant().Contains(searchString.ToLowerInvariant())
                              select sdc).ToList();
                }

            ViewBag.SearchValue = searchString;
            int pageNumber = (page ?? 1);
            return View(sdcs.ToPagedList(pageNumber, pageSize));
            }

        public ActionResult Create()
            {
            ViewBag.Title = "New SDC";
            SelectList districts = new SelectList(new List<string>());
            ViewBag.DistrictId = districts;
            return View();
            }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SDC sdc)
            {
            if (!ModelState.IsValid)
                {
                return View("Create");
                }

            var response = await client.PostAsJsonAsync(apiBaseUrl + "/SDC", sdc);
            if (response.IsSuccessStatusCode)
                {
                return RedirectToAction("Index");
                }
            SelectList districts = new SelectList(new List<string>());
            ViewBag.DistrictId = districts;
            ViewBag.Error = response.ReasonPhrase;
            return View(sdc);
            }

        public async Task<ActionResult> Edit(int? id)
            {
            if (id == null)
                {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            var response = await client.GetAsync(apiBaseUrl + "/SDC/" + id.ToString());
            SDC sdc = null;
            if (response.IsSuccessStatusCode)
                {
                var data = response.Content.ReadAsStringAsync().Result;
                sdc = JsonConvert.DeserializeObject<SDC>(data);
                }
            SelectList districts = new SelectList(new List<string>());
            ViewBag.DistrictId = districts;
            ViewBag.Title = "Edit SDC";
            return View(sdc);
            }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, SDC sdc)
            {
            ViewBag.Error = null;
            List<SDC> sdcs = new List<SDC>();
            if (!ModelState.IsValid)
                {
                return View();
                }
            try
                {
                var response = await client.PutAsJsonAsync(apiBaseUrl + "/SDC/" + id.ToString(), sdc);
                if (response.IsSuccessStatusCode)
                    {
                    ViewData["Message"] = "New SDC added successfully";
                    return RedirectToAction("Index");
                    }
                else
                    {
                    ViewBag.Error = response.Content.ReadAsStringAsync().Result;
                    }
                }
            catch (Exception ex)
                {
                ViewBag.Error = ex.Message;
                return View("Edit");
                }
            return View("Index", sdcs);
            }

        public ActionResult Delete(string id)
            {
            if (id == null)
                {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            var response = client.DeleteAsync(apiBaseUrl + "/SDC/" + id.ToString()).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
                {
                TempData["Message"] = "Selected sdc deleted successfully.";
                }
            else
                {
                TempData["Error"] = response.ReasonPhrase;
                }

            return RedirectToAction("Index");
            }

        public JsonResult PopulateDistricts()
            {
            List<District> districts = new List<District>();
            var responseMessage = client.GetAsync(apiBaseUrl + "/district").GetAwaiter().GetResult();
            if (responseMessage.IsSuccessStatusCode)
                {
                var responseData = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                districts = JsonConvert.DeserializeObject<List<District>>(responseData);
                }
            return Json(districts, JsonRequestBehavior.AllowGet);
            }
    }
}