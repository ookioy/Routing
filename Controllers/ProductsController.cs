using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductsWithRouting.Models;
using ProductsWithRouting.Services;

namespace ProductsWithRouting.Controllers
{
    public class ProductsController : Controller
    {
        private List<Product> myProducts;

        public ProductsController(Data data)
        {
            myProducts = data.Products;
        }

        public IActionResult Index(int? filterId, string filtername)
        {
            ViewBag.FilterId = filterId;
            ViewBag.FilterName = filtername;

            var products = myProducts.AsEnumerable();
            if (filterId.HasValue)
            {
                products = products.Where(product => product.Id == filterId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtername))
            {
                products = products.Where(product =>
                    product.Name != null &&
                    product.Name.Contains(filtername, StringComparison.OrdinalIgnoreCase));
            }

            return View(products.ToList());
        }

        public IActionResult View(int id)
        {
            var product = myProducts.FirstOrDefault(product => product.Id == id);
            return product == null ? RedirectToRoute("product-not-found") : View(product);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = myProducts.FirstOrDefault(product => product.Id == id);
            return product == null ? RedirectToRoute("product-not-found") : View(product);
        } 
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            var existingProduct = myProducts.FirstOrDefault(item => item.Id == product.Id);
            if (existingProduct == null)
            {
                return RedirectToRoute("product-not-found");
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            return RedirectToAction(nameof(Index));
        } 
        
        [HttpPost]
        public IActionResult Create(Product product)
        {
            product.Id = myProducts.Count == 0 ? 1 : myProducts.Max(item => item.Id) + 1;
            myProducts.Add(product);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Product());
        }

        public IActionResult Delete(int id)
        {
            var product = myProducts.FirstOrDefault(item => item.Id == id);
            if (product == null)
            {
                return RedirectToRoute("product-not-found");
            }

            myProducts.Remove(product);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ProductNotFound()
        {
            return View("NotFound");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
