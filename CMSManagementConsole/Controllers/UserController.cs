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
    public class UserController : Controller
    {
        private string apiBaseUrl = "";
        HttpClient client;
        int pageSize;

        public UserController()
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

            List<UserView> users = new List<UserView>();
            List<District> districts = new List<District>();
            List<SDC> sdcs = new List<SDC>();
            List<int> districtIds = new List<int>();
            List<int> sdcIds = new List<int>();
            Dictionary<int, string> sdcList = new Dictionary<int, string>();
            Dictionary<int, string> districtsList = new Dictionary<int, string>();

            var responseMessage = await client.GetAsync(apiBaseUrl + "/accounts/users");
            if (responseMessage.IsSuccessStatusCode)
                {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<UserView>>(responseData);

                if (!ViewData.ContainsKey("Districts"))
                    {
                    foreach (var user in users)
                        {
                        districtIds.Add(user.DistrictId);
                        sdcIds.Add(user.SDCId);
                        }
                    //Getting district names for district ids in the user table
                    responseMessage = await client.PostAsJsonAsync(apiBaseUrl + "/District/TakeMany", districtIds);
                    if (responseMessage.IsSuccessStatusCode)
                        {
                        responseData = responseMessage.Content.ReadAsStringAsync().Result;
                        districts = JsonConvert.DeserializeObject<List<District>>(responseData);
                        foreach (var district in districts)
                            {
                            districtsList.Add(district.Id, district.Name);
                            }
                        TempData["Districts"] = districtsList;
                        }

                    //Getting sdc names for sdc ids in the user table
                    responseMessage = await client.PostAsJsonAsync(apiBaseUrl + "/SDC/TakeMany", sdcIds);
                    if (responseMessage.IsSuccessStatusCode)
                        {
                        responseData = responseMessage.Content.ReadAsStringAsync().Result;
                        sdcs = JsonConvert.DeserializeObject<List<SDC>>(responseData);
                        foreach (var sdc in sdcs)
                            {
                            sdcList.Add(sdc.Id, sdc.Title);
                            }
                        TempData["SDCs"] = sdcList;
                        }
                    }
                else
                    {
                    var districtsData = TempData["Districts"];
                    districtsList = (Dictionary<int, string>)districtsData;
                    var sdcsData = TempData["SDCs"];
                    sdcList = (Dictionary<int, string>)sdcsData;
                    }

                foreach(var user in users)
                    {
                    user.District = districtsList[user.DistrictId];
                    user.SDC = sdcList[user.SDCId];
                    }
                }

            if (searchString != null)
                {
                users = (from user in users
                              where user.FullName.ToLowerInvariant().Contains(searchString.ToLowerInvariant())
                              select user).ToList();
                }

            ViewBag.SearchValue = searchString;
            int pageNumber = (page ?? 1);
            return View(users.ToPagedList(pageNumber, pageSize));
            }

        public ActionResult Create()
            {
            ViewBag.Title = "New User";
            SelectList districts = new SelectList(new List<string>());
            SelectList sdcs = new SelectList(new List<string>());
            SelectList roles = new SelectList(new List<string>());
            ViewBag.DistrictId = districts;
            ViewBag.SDCId = sdcs;
            ViewBag.RoleName = roles;
            return View();
            }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateUserBindingModel user)
            {
            if (!ModelState.IsValid)
                {
                return View("Create");
                }

            var response = await client.PostAsJsonAsync(apiBaseUrl + "/accounts/create", user);
            if (response.IsSuccessStatusCode)
                {
                return RedirectToAction("Index");
                }

            SelectList districts = new SelectList(new List<string>());
            SelectList sdcs = new SelectList(new List<string>());
            SelectList roles = new SelectList(new List<string>());
            ViewBag.DistrictId = districts;
            ViewBag.SDCId = sdcs;
            ViewBag.RoleName = roles;
            ViewBag.Error = response.ReasonPhrase;
            return View(user);
            }

        public async Task<ActionResult> Edit(int? id)
            {
            if (id == null)
                {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            var response = await client.GetAsync(apiBaseUrl + "/User/" + id.ToString());
            UserView user = null;
            if (response.IsSuccessStatusCode)
                {
                var data = response.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<UserView>(data);
                }
            ViewBag.Title = "Edit User";
            return View(user);
            }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, UserView user)
            {
            ViewBag.Error = null;
            if (!ModelState.IsValid)
                {
                return View();
                }
            try
                {
                var response = await client.PutAsJsonAsync(apiBaseUrl + "/User/" + id.ToString(), user);
                if (response.IsSuccessStatusCode)
                    {
                    ViewData["Message"] = "New User added successfully";
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
            return View();
            }

        public ActionResult Delete(string id)
            {
            if (id == null)
                {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            var response = client.DeleteAsync(apiBaseUrl + "/User/" + id.ToString()).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
                {
                TempData["Message"] = "Selected user deleted successfully.";
                }
            else
                {
                TempData["Error"] = response.ReasonPhrase;
                }

            return RedirectToAction("Index");
            }

        public JsonResult PopulateDistricts()
            {
            List<District> districtList = null;
            if (TempData["districtList"] != null)
                {
                districtList = JsonConvert.DeserializeObject<List<District>>(TempData["districtList"].ToString());
                }
            else
                {
                var response = client.GetAsync(this.apiBaseUrl + "District").GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                    {
                    string responseData = response.Content.ReadAsStringAsync().Result;
                    districtList = JsonConvert.DeserializeObject<List<District>>(responseData);
                    TempData["districtList"] = JsonConvert.SerializeObject(districtList);
                    }
                }
            return Json(districtList, JsonRequestBehavior.AllowGet);
            }

        public JsonResult PopulateSDCs(int id)
            {
            List<SDC> sdcList = new List<SDC>();
            List<SDC> resultList = new List<SDC>();
            if (TempData.ContainsKey("SDCsList"))
                {
                resultList = JsonConvert.DeserializeObject<List<SDC>>(TempData["SDCsList"].ToString());
                sdcList = (from sdc in resultList
                           where sdc.DistrictId.Equals(id)
                           select sdc).ToList();
                }
            else
                {
                var response = client.GetAsync(this.apiBaseUrl + "SDC").GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                    {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(responseData))
                        {
                        resultList = JsonConvert.DeserializeObject<List<SDC>>(responseData);
                        sdcList = (from sdc in resultList
                                   where sdc.DistrictId.Equals(id)
                                   select sdc).ToList();
                        }
                    }
                TempData["SDCsList"] = JsonConvert.SerializeObject(resultList); 
                }
            return Json(sdcList, JsonRequestBehavior.AllowGet);
            }

        public JsonResult PopulateRoles()
            {
            List<Role> rolesList = new List<Role>();
            var response = client.GetAsync(this.apiBaseUrl + "accounts/roles").GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
                {
                var responseData = response.Content.ReadAsStringAsync().Result;
                rolesList = JsonConvert.DeserializeObject<List<Role>>(responseData);
                }
            return Json(rolesList, JsonRequestBehavior.AllowGet);
            }
    }
}