using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Book_Project_Goss.Models;

namespace BookProject.Controllers
{
    public class LineItemsController : Controller
    {
        // GET: LineItems
        public ActionResult AllLineItems(string id, string filter = "All", int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<InvoiceLineItem> lineItems = context.InvoiceLineItems.ToList();

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc == true)
                        {
                            lineItems = context.InvoiceLineItems.OrderByDescending(i => i.Product.Description).ToList();
                        }
                        else
                        {
                            lineItems = context.InvoiceLineItems.OrderBy(i => i.Product.Description).ToList();
                        }
                        break;
                    }
                case 2:
                    {
                        if (isDesc == true)
                        {
                            lineItems = context.InvoiceLineItems.OrderByDescending(i => i.UnitPrice).ToList();
                        }
                        else
                        {
                            lineItems = context.InvoiceLineItems.OrderBy(i => i.UnitPrice).ToList();
                        }
                        break;
                    }
                case 3:
                    {
                        if (isDesc == true)
                        {
                            lineItems = context.InvoiceLineItems.OrderByDescending(i => i.Quantity).ToList();
                        }
                        else
                        {
                            lineItems = context.InvoiceLineItems.OrderBy(i => i.Quantity).ToList();
                        }
                        break;
                    }
                case 4:
                    {
                        if (isDesc == true)
                        {
                            lineItems = context.InvoiceLineItems.OrderByDescending(i => i.ItemTotal).ToList();
                        }
                        else
                        {
                            lineItems = context.InvoiceLineItems.OrderBy(i => i.ItemTotal).ToList();
                        }
                        break;
                    }
                default:
                    {
                        if (isDesc == true)
                        {
                            lineItems = context.InvoiceLineItems.OrderByDescending(i => i.InvoiceID).ToList();
                        }
                        else
                        {
                            lineItems = context.InvoiceLineItems.OrderBy(i => i.InvoiceID).ToList();
                        }
                        break;
                    }
            }

            if (id != null)
            {
                lineItems = SearchLineItems(id, filter, lineItems);
            }

            lineItems = lineItems.Where(i => i.IsDeleted == false).ToList();

            return View(lineItems);
        }

        [HttpGet]
        public ActionResult LineItemDelete(int inID, string prodID)
        {
            BooksEntities context = new BooksEntities();

            try
            {
                InvoiceLineItem lineItem = context.InvoiceLineItems.Where(i => i.InvoiceID == inID && i.ProductCode == prodID).FirstOrDefault();
                lineItem.IsDeleted = true;
                context.SaveChanges();
            }
            catch (Exception er)
            {

                throw (er);
            }

            return RedirectToAction("AllLineItems");
        }

        private List<InvoiceLineItem> SearchLineItems(string id, string filter, List<InvoiceLineItem> lineItems)
        {
            id = id.Trim().ToLower();

            if (filter == "All")
            {
                if (int.TryParse(id, out int num))
                {
                    lineItems = lineItems.Where(l =>
                        l.InvoiceID.ToString().Contains(id.ToString()) ||
                        l.UnitPrice.ToString().Contains(id.ToString()) ||
                        l.Quantity.ToString().Contains(id.ToString()) ||
                        l.ItemTotal.ToString().Contains(id.ToString())).ToList();
                }
                else
                {
                    lineItems = lineItems.Where(l =>
                        l.Product.Description.ToLower().Contains(id)).ToList();
                }
            }
            else if (filter == "Id")
            {
                int.TryParse(id, out int num);

                lineItems = lineItems.Where(l =>
                    l.InvoiceID.ToString().Contains(id.ToString())).ToList();
            }
            else if (filter == "Des")
            {
                lineItems = lineItems.Where(l =>
                    l.Product.Description.Contains(id)).ToList();
            }
            else if (filter == "UnPrice")
            {
                int.TryParse(id, out int num);

                lineItems = lineItems.Where(l =>
                    l.UnitPrice.ToString().Contains(id.ToString())).ToList();
            }
            else if (filter == "Total")
            {
                int.TryParse(id, out int num);

                lineItems = lineItems.Where(l =>
                    l.ItemTotal.ToString().Contains(id.ToString())).ToList();
            }
            else if(filter == "Quant")
            {
                int.TryParse(id, out int num);

                lineItems = lineItems.Where(l =>
                    l.Quantity.ToString().Contains(id.ToString())).ToList();
            }

            return lineItems;
        }
    }
}