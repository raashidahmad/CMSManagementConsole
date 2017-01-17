using CMSManagementConsole.Helpers;
using CMSManagementConsole.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using PagedList;

namespace CMSManagementConsole.Controllers
{
    [AuthorizationFilter]
    public class CategoryController : Controller
        {
        private string apiBaseUrl = "";
        HttpClient client;
        int pageSize;

        public CategoryController()
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

            List<Category> categories = new List<Category>();
            var responseMessage = await client.GetAsync(apiBaseUrl + "/Category");
            if (responseMessage.IsSuccessStatusCode)
                {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                categories = JsonConvert.DeserializeObject<List<Category>>(responseData);
                }

            if (searchString != null)
                {
                categories = (from category in categories
                              where category.Name.ToLowerInvariant().Contains(searchString.ToLowerInvariant())
                              select category).ToList();
                }

            ViewBag.SearchValue = searchString;
            int pageNumber = (page ?? 1);
            return View(categories.ToPagedList(pageNumber, pageSize));
            }

        public ActionResult Create()
            {
            ViewBag.Title = "Add New Category";
            return View();
            }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Category category)
            {
            if (!ModelState.IsValid)
                {
                return View("Create");
                }

            var response = await client.PostAsJsonAsync(apiBaseUrl + "/Category", category);
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
            var response = await client.GetAsync(apiBaseUrl + "/Category/" + id.ToString());
            Category category = null;
            if (response.IsSuccessStatusCode)
                {
                var data = response.Content.ReadAsStringAsync().Result;
                category = JsonConvert.DeserializeObject<Category>(data);
                }
            ViewBag.Title = "Edit Category";
            return View(category);
            }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, Category category)
            {
            ViewBag.Error = null;
            List<Category> categories = new List<Category>();
            if (!ModelState.IsValid)
                {
                return View();
                }
            try
                {
                var response = await client.PutAsJsonAsync(apiBaseUrl + "/Category/" + id.ToString(), category);
                if (response.IsSuccessStatusCode)
                    {
                    var responseMessage = await client.GetAsync(apiBaseUrl + "/Category");
                    if (responseMessage.IsSuccessStatusCode)
                        {
                        var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                        categories = JsonConvert.DeserializeObject<List<Category>>(responseData);
                        }
                    }
                }
            catch (Exception ex)
                {
                ViewBag.Error = ex.Message;
                return View("Edit");
                }
            return View("Index", categories);
            }

        public ActionResult Delete(string id)
            {
            if (id == null)
                {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            var response = client.DeleteAsync(apiBaseUrl + "/Category/" + id.ToString()).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
                {
                TempData["Message"] = "Selected category deleted successfully.";
                }
            else
                {
                TempData["Error"] = response.ReasonPhrase;
                }

            return RedirectToAction("Index");
            }
        }
}