using Microsoft.AspNetCore.Mvc;

public class AboutController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        ViewBag.ContactInfo = new
        {
            Email = "TLenglish@gmail.com",
            Phone = "+84 898 165 109",
            Address = "590 Núi thành, Quận Hải Châu ,TP. Đà Nẵng, Việt Nam",
            MapIframeUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3834.1900797255637!2d108.21124551536009!3d16.039051344410394!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x314219bfe45f948f%3A0x2607f2a8747a0670!2zNTkwIE7DumkgVGjDoG5oLCBUaMOgbmggcGjhu5cgxJDDoCBO4bq1bmcsIFZp4buHdCBOYW0!5e0!3m2!1svi!2s!4v1697301995483!5m2!1svi!2s"
    };
        return View();
    }
}