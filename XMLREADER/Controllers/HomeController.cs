using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XMLReader.Models;
using XMLReader.Services.Interfaces;
using XMLREADER.Models;

namespace XMLREADER.Controllers
{
    public class HomeController : Controller
    {
       // private readonly ILogger<HomeController> _logger;
        private  readonly IProductService _productService;
        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _productService = productService;
        }

       
        
        [HttpPost]
        public async Task<RedirectToActionResult> UploadXml(IFormFile file)
        {
            await _productService.ReadXMLWriteDB(file);
            return RedirectToAction("Index");
        }
        public IActionResult UploadXml()
        {
            return View();
        }



        public async Task<IActionResult> Index(int page=2, int pageSize=5)
        {
            int offset = (page - 1) * pageSize;
            int fetch = pageSize;
            IEnumerable<ProductModel> items = await _productService.GetProducts(offset,fetch);
            PageViewModel pageViewModel = new PageViewModel(_productService.Count, page, pageSize);
            ViewBag.PageViewModel = pageViewModel;
            ViewBag.Users = items;
            return View();
        }



















        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
