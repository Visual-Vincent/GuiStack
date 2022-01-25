using System;
using Microsoft.AspNetCore.Mvc;

namespace GuiStack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class S3Controller : Controller
    {
        [HttpGet]
        public ActionResult Get()
        {
            return Json(new { test = "ok" });
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
