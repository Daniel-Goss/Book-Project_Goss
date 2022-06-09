using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Book_Project_Goss.Models;

namespace BookProject.Controllers
{
    public class ProductController : Controller
    {
        // GET: Default
        public ActionResult AllProducts(string id, string filter = "All", int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<Product> products = context.Products.ToList();

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc == true)
                        {
                            products = context.Products.OrderByDescending(p => p.Description).ToList();
                        }
                        else
                        {
                            products = context.Products.OrderBy(p => p.Description).ToList();
                        }
                        break;
                    }
                case 2:
                    {
                        if (isDesc == true)
                        {
                            products = context.Products.OrderByDescending(p => p.UnitPrice).ToList();
                        }
                        else
                        {
                            products = context.Products.OrderBy(p => p.UnitPrice).ToList();
                        }
                        break;
                    }
                case 3:
                    {
                        if (isDesc == true)
                        {
                            products = context.Products.OrderByDescending(p => p.OnHandQuantity).ToList();
                        }
                        else
                        {
                            products = context.Products.OrderBy(p => p.OnHandQuantity).ToList();
                        }
                        break;
                    }

                default:
                    {
                        if (isDesc == true)
                        {
                            products = context.Products.OrderByDescending(p => p.ProductCode).ToList();
                        }
                        else
                        {
                            products = context.Products.OrderBy(p => p.ProductCode).ToList();
                        }
                        break;
                    }
            }

            if (id != null)
            {
                products = SearchProducts(id, filter, products);
            }

            products = products.Where(p => p.IsDeleted == false).ToList();

            return View(products);
        }

        [HttpGet]
        public ActionResult ProductUpsert(string id)
        {
            BooksEntities context = new BooksEntities();
            Product product = context.Products.Where(p => p.ProductCode == id).FirstOrDefault();

            if (product == null)
            {
                product = new Product();
            }

            return View(product);
        }

        [HttpPost]
        public ActionResult ProductUpsert(Product product)
        {
            BooksEntities context = new BooksEntities();

            try
            {
                if (context.Products.Where(p => p.ProductCode == product.ProductCode).Count() > 0)
                {
                    //States already exists
                    var check = context.Products.Where(p => p.ProductCode == product.ProductCode).FirstOrDefault();

                    check.Description = product.Description;
                    check.OnHandQuantity = product.OnHandQuantity;
                    check.UnitPrice = product.UnitPrice;
                    check.IsDeleted = false;
                }
                else
                {
                    context.Products.Add(product);
                }

                context.SaveChanges();
            }
            catch (Exception er)
            {

                throw (er);
            }

            return RedirectToAction("AllProducts");
        }

        [HttpGet]
        public ActionResult ProductDelete(string id)
        {
            BooksEntities context = new BooksEntities();

            try
            {
                Product product = context.Products.Where(p => p.ProductCode == id).FirstOrDefault();
                product.IsDeleted = true;
                context.SaveChanges();
            }
            catch (Exception er)
            {

                throw (er);
            }

            return RedirectToAction("AllProducts");
        }

        private List<Product> SearchProducts(string id, string filter, List<Product> products)
        {
            id = id.Trim().ToLower();

            if (filter == "All")
            {
                if (int.TryParse(id, out int num))
                {
                    products = products.Where(p =>
                        p.OnHandQuantity.ToString().Contains(num.ToString()) ||
                        p.UnitPrice.ToString().Contains(num.ToString())).ToList();
                }
                else
                {
                    products = products.Where(p =>
                        p.Description.ToLower().Contains(id) ||
                        p.ProductCode.ToLower().Contains(id)).ToList();
                }
            }
            else if (filter == "Code")
            {
                products = products.Where(p =>
                    p.ProductCode.ToLower().Contains(id)).ToList();
            }
            else if (filter == "Des")
            {
                products = products.Where(p =>
                    p.Description.ToLower().Contains(id)).ToList();
            }
            else if (filter == "UnPrice")
            {
                int.TryParse(id, out int num);

                products = products.Where(p =>
                    p.UnitPrice.ToString().Contains(num.ToString())).ToList();
            }
            else if (filter == "Quant")
            {
                int.TryParse(id, out int num);

                products = products.Where(p =>
                    p.OnHandQuantity.ToString().Contains(num.ToString())).ToList();
            }

            return products;
        }
    }
}