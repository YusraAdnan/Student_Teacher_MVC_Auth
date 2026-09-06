using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Student_Teacher_MVC_Auth.Models;
using System.Text;
using System.Text.Json.Serialization;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public AccountController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var client = _clientFactory.CreateClient();
        client.BaseAddress = new Uri("https://localhost:7261/");

        var json = JsonConvert.SerializeObject(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("api/auth/register", content);

        if (response.IsSuccessStatusCode)
        { 
            return RedirectToAction("Login"); 
        }

        ModelState.AddModelError("", "Registration failed");
        return View(model);
    }

    [HttpGet]
    public IActionResult Login() => View();

    /* The browser is the client here. The MVC is the one answering
     * Plain form submission - No cookie exists at line 43-44 */
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        /* MVC server now becomes the client - plays role of the one sending a request to the API server
         * Client is acting as the "browser" here on line 49
         * It is the thing sending a request and waiting for a response 
         (just like a real browser does with a website)
        */
        var client = _clientFactory.CreateClient();
        client.BaseAddress = new Uri("https://localhost:7261/");

        var json = JsonConvert.SerializeObject(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //At this point the API runs its Login action
        var response = await client.PostAsync("api/auth/login", content);

        //MVC server reads that cookie issued by the API server 
        if (response.IsSuccessStatusCode)
        {
            /* Because MVC was behaving like the "client" to the API, it now holds that response 
             including the Set-Cookie header in memory 

             * This line 69 reaches into the API's response and pulls out that cookie string
             as plain text, so the MVC server can decide what to do with it.*/
            var cookie = response.Headers.GetValues("Set-Cookie").First();
            
            /* MVC server stores that cookie string somwehre it can find later 
             This is where Session comes in. 
             * The MVC server needs to remember that "this specific visitor relates to this
             specific API cookie" across many future requests 
             * HttpContext.Session is ASP.NET core's built in place to stash per visitor data, server side
             (it lives in the MVC servers own memory, not sent to the browser)
            */
            HttpContext.Session.SetString("ApiAuthCookie", cookie);

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Invalid login");
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Remove("ApiAuthCookie");
        return RedirectToAction("Login");
    }
}