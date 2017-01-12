using CMSManagementConsole.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace CMSManagementConsole.Controllers
    {
    public class HomeController : Controller
        {
        private string apiBaseUrl = "";
        HttpClient client;

        public HomeController()
            {
            apiBaseUrl = WebConfigurationManager.AppSettings["ApiBaseUrl"];
            client = new HttpClient();
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

        public ActionResult Index()
            {
            ViewBag.loginFailed = "";
            return View("Login");
            }

        public ActionResult Login()
            {
            return View();
            }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
            {
            ViewBag.loginFailed = "";
            if (!ModelState.IsValid)
                {
                return View(model);
                }

            try
                {
                var formContent = new FormUrlEncodedContent(new[]
                 {
                 new KeyValuePair<string, string>("grant_type", "password"), 
                 new KeyValuePair<string, string>("username", model.Username), 
                 new KeyValuePair<string, string>("password", model.Password), 
                 });

                HttpResponseMessage responseMessage = await client.PostAsync("/Token", formContent);

                //get access token from response body
                var responseJson = await responseMessage.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                string accessToken = "";
                if (jObject.GetValue("access_token") != null)
                    {
                    accessToken = jObject.GetValue("access_token").ToString();
                    }
                else
                    {
                    ViewBag.loginFailed = "Username/Password you entered is incorrect.";
                    return View("Login");
                    }
                Session["accessToken"] = accessToken;
                return RedirectToAction("Welcome");
                }
            catch (Exception ex)
                {
                ViewBag.loginFailed = ex.Message;
                }
            return View("Login");
            }

        public ActionResult Welcome()
            {
            return View();
            }
        
        }
    }