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
using CMSManagementConsole.Helpers;

namespace CMSManagementConsole.Controllers
{
    [AuthorizationFilter]
    public class ComplaintController : Controller
    {
        private string apiBaseUrl = "";
        HttpClient client;
        int pageSize;

        public ComplaintController()
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
            searchString = HttpUtility.UrlDecode(searchString);
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
            List<ComplaintView> complaints = new List<ComplaintView>();
            var responseMessage = await client.GetAsync(apiBaseUrl + "/Complaint");
            if (responseMessage.IsSuccessStatusCode)
                {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                complaints = JsonConvert.DeserializeObject<List<ComplaintView>>(responseData);
                }

            if (searchString != null)
                {
                searchString = searchString.ToLower().Trim();
                complaints = (from complaint in complaints
                              where 
                              complaint.Complainant.ToLower().Contains(searchString) ||
                              complaint.Mobile.ToLower().Contains(searchString) ||
                              complaint.NIC.ToLower().Contains(searchString)
                              select complaint).ToList();
                }

            ViewBag.SearchValue = searchString;
            int pageNumber = (page ?? 1);
            return View(complaints.ToPagedList(pageNumber, pageSize));
            }

        public ActionResult Create()
            {
            ViewBag.IsError = false;
            ViewBag.Title = "New Complaint";
            SelectList categories = new SelectList(new List<string>());
            ViewBag.CategoryId = categories;
            return View();
            }

        public async Task<ActionResult> Details(int id)
            {
            int validId = 0;
            bool valid = int.TryParse(id.ToString(), out validId);
            
            if (!valid || validId == 0)
                {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

            ComplaintFullView complaint = new ComplaintFullView();
            var responseMessage = await client.GetAsync(apiBaseUrl + "/Complaint/" + id);
            if (responseMessage.IsSuccessStatusCode)
                {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                complaint = JsonConvert.DeserializeObject<ComplaintFullView>(responseData);
                if (complaint == null)
                    ViewBag.Error = responseMessage.Content.ReadAsStringAsync().Result;
                else
                    {
                    responseMessage = await client.GetAsync(apiBaseUrl + "/File/" + id);
                    if (responseMessage.IsSuccessStatusCode)
                        {
                        responseData = responseMessage.Content.ReadAsStringAsync().Result;
                        var documentsList = JsonConvert.DeserializeObject<ViewDataUploadFilesResult[]>(responseData);
                        if (documentsList != null)
                            complaint.Documents = documentsList.ToList();
                        }
                    }
                }
            else
                {
                ViewBag.Error = responseMessage.Content.ReadAsStringAsync().Result;
                }
            return View(complaint);
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(NewComplaint complaint)
            {
            ViewBag.IsError = false;
            SelectList categories = new SelectList(new List<string>());
            ViewBag.CategoryId = categories;
            complaint.DistrictId = Convert.ToInt32(System.Web.HttpContext.Current.Session["DistrictId"].ToString());
            complaint.SDCId = Convert.ToInt32(System.Web.HttpContext.Current.Session["SDCId"].ToString());
            
            if (!ModelState.IsValid)
                {
                ViewBag.IsError = true;
                return View(complaint);
                }
            
            //Check if new complainant
            if (complaint.ComplainantId == 0)
                {
                var complainant = new Complainant()
                {
                    FullName = complaint.FullName,
                    NIC = complaint.NIC,
                    Mobile = complaint.Mobile,
                    Address = complaint.Address,
                    ContactMedium = Complainant.CommunicationMedium.SMS
                };
                var complainantResponse = await client.PostAsJsonAsync(apiBaseUrl + "/Complainant", complainant);
                if (complainantResponse.IsSuccessStatusCode)
                    {
                    var resultStr = complainantResponse.Content.ReadAsStringAsync().Result;
                    int newId = 0;
                    bool isComplainant = int.TryParse(resultStr, out newId);
                    if (isComplainant)
                        complaint.ComplainantId = newId;

                    }
                }

            var response = await client.PostAsJsonAsync(apiBaseUrl + "/Complaint", complaint);
            if (response.IsSuccessStatusCode)
                {
                
                var cId = response.Content.ReadAsStringAsync().Result;
                return RedirectToAction("Upload", new { id =  cId});
                }
            
            ViewBag.Error = response.Content.ReadAsStringAsync().Result;
            return View(complaint);
            }

        public ActionResult Upload(int? id)
            {
            if (id == 0)
                {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            ViewBag.Domain = this.apiBaseUrl;
            ViewBag.ComplaintId = id;
            return View();
            }

        public async Task<ActionResult> Edit(int? id)
            {
            if (id == 0)
                {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            var response = await client.GetAsync(apiBaseUrl + "/Complaint/" + id.ToString());
            NewComplaint complaint = null;
            if (response.IsSuccessStatusCode)
                {
                var data = response.Content.ReadAsStringAsync().Result;
                complaint = JsonConvert.DeserializeObject<NewComplaint>(data);
                }
            SelectList districts = new SelectList(new List<string>());
            ViewBag.DistrictId = districts;
            ViewBag.Title = "Edit Complaint";
            return View(complaint);
            }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, NewComplaint complaint)
            {
            ViewBag.Error = null;
            List<NewComplaint> complaints = new List<NewComplaint>();
            if (!ModelState.IsValid)
                {
                return View();
                }
            try
                {
                var response = await client.PutAsJsonAsync(apiBaseUrl + "/Complaint/" + id.ToString(), complaint);
                if (response.IsSuccessStatusCode)
                    {
                    ViewData["Message"] = "New Complaint added successfully";
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
            return View("Index", complaints);
            }

        public ActionResult Delete(string id)
            {
            if (id == null)
                {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            var response = client.DeleteAsync(apiBaseUrl + "/Complaint/" + id.ToString()).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
                {
                TempData["Message"] = "Selected complaint deleted successfully.";
                }
            else
                {
                TempData["Error"] = response.ReasonPhrase;
                }

            return RedirectToAction("Index");
            }
       
        public JsonResult PopulateCategories()
            {
            List<Category> categories = new List<Category>();
            var responseMessage = client.GetAsync(apiBaseUrl + "/category").GetAwaiter().GetResult();
            if (responseMessage.IsSuccessStatusCode)
                {
                var responseData = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                categories = JsonConvert.DeserializeObject<List<Category>>(responseData);
                }
            return Json(categories, JsonRequestBehavior.AllowGet);
            }

        public JsonResult GetComplainants()
            {
            List<ComplainantList> complainants = new List<ComplainantList>();
            var responseMessage = client.GetAsync(apiBaseUrl + "/complainant/GetShortList").GetAwaiter().GetResult();
            if (responseMessage.IsSuccessStatusCode)
                {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                complainants = JsonConvert.DeserializeObject<List<ComplainantList>>(responseData);
                }
            return Json(complainants, JsonRequestBehavior.AllowGet);
            }

    }
}