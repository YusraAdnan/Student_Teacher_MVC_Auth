using Microsoft.AspNetCore.Mvc;
using Student_Teacher_MVC_Auth.Models;
using System.Diagnostics;

namespace Student_Teacher_MVC_Auth.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public HomeController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7261/");

            var authCookie = HttpContext.Session.GetString("ApiAuthCookie");
            if (authCookie != null)
            {
                client.DefaultRequestHeaders.Add("Cookie", authCookie);
            }

            var response = await client.GetAsync("api/Teacher/teacher-only");
            var message = response.IsSuccessStatusCode;
            await response.Content.ReadAsStringAsync();
            string modelMessage;

            if (response.IsSuccessStatusCode)
            {
                modelMessage = await response.Content.ReadAsStringAsync();
            }
            else
            {
                modelMessage = "Access Denied: " + response.StatusCode;
            }


            return View(model: modelMessage );
        }

        public async Task<IActionResult> StudentArea()
        {
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7261/");

            var authCookie = HttpContext.Session.GetString("ApiAuthCookie");
            if (authCookie != null)
                client.DefaultRequestHeaders.Add("Cookie", authCookie);

            var response = await client.GetAsync("api/Student/student-only");
            var message = response.IsSuccessStatusCode;
            await response.Content.ReadAsStringAsync();
            string modelMessage;

            if (response.IsSuccessStatusCode)
            {
                modelMessage = await response.Content.ReadAsStringAsync();
            }
            else
            {
                modelMessage = "Access Denied: " + response.StatusCode;
            }


            return View(model: modelMessage);
        }
    }
}
