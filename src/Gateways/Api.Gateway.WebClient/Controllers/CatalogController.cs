using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Api.Gateway.WebClient.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("catalog")]
    [EnableCors("AllowOrigin")]
    public class CatalogController : Controller
    {
        //private readonly ICatalogProxy _catalogProxy;

        //public CatalogController(
        //    ICatalogProxy catalogProxy
        //)
        //{
        //    _catalogProxy = catalogProxy;
        //}
        public IActionResult Index()
        {
            return View();
        }
    }
}
