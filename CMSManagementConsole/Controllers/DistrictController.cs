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
using CMSManagementConsole.Helpers;

namespace CMSManagementConsole.Controllers
{
    [AuthorizationFilter]
    public class DistrictController : Controller
    {
        private string apiBaseUrl = "";
        HttpClient client;
        int pageSize;

        public DistrictController()
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
            ViewBag.Error = null;
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

            List<District> districts = new List<District>();
            var responseMessage = await client.GetAsync(apiBaseUrl + "/District");
            if (responseMessage.IsSuccessStatusCode)
                {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                districts = JsonConvert.DeserializeObject<List<District>>(responseData);
                }

            if (searchString != null)
                {
                districts = (from district in districts
                              where district.Name.ToLowerInvariant().Contains(searchString.ToLowerInvariant())
                              select district).ToList();
                }

            ViewBag.SearchValue = searchString;
            int pageNumber = (page ?? 1);
            return View(districts.ToPagedList(pageNumber, pageSize));
            }

        public ActionResult Create()
            {
            ViewBag.Title = "Add New District";
            return View();
            }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(District district)
            {
            if (!ModelState.IsValid)
                {
                return View("Create");
                }

            var response = await client.PostAsJsonAsync(apiBaseUrl + "/District", district);
            if (response.IsSuccessStatusCode)
                {
                return RedirectToAction("Index");
                }
            ViewBag.Error = response.ReasonPhrase;
            return RedirectToAction("Create");
            }

        public async Task<ActionResult> Edit(int? id)
            {
            if (id == null)
                {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            var response = await client.GetAsync(apiBaseUrl + "/District/" + id.ToString());
            District district = null;
            if (response.IsSuccessStatusCode)
                {
                var data = response.Content.ReadAsStringAsync().Result;
                district = JsonConvert.DeserializeObject<District>(data);
                }
            
            ViewBag.Title = "Edit District";
            return View(district);
            }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, District district)
            {
            ViewBag.Error = null;
            List<District> districts = new List<District>();
            if (!ModelState.IsValid)
                {
                return View();
                }
            try
                {
                var response = await client.PutAsJsonAsync(apiBaseUrl + "/District/" + id.ToString(), district);
                if (response.IsSuccessStatusCode)
                    {
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
                }
            return View();
            }

        public ActionResult Delete(string id)
            {
            if (id == null)
                {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            var response = client.DeleteAsync(apiBaseUrl + "/District/" + id.ToString()).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
                {
                TempData["Message"] = "Selected district deleted successfully.";
                }
            else
                {
                TempData["Error"] = response.ReasonPhrase;
                }

            return RedirectToAction("Index");
            }
    }
}