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
        /// <summary>
        ///     Displays a list of LineItems to the user. Allows user to search through the  list
        ///     for all or with filters and sorts list either ascending or descending
        /// </summary>
        /// <param name="id"> search term - string </param>
        /// <param name="filter"> filter selection - string </param>
        /// <param name="sortBy"> sort column - int </param>
        /// <param name="isDesc"> sort direction - boolean </param>
        /// <returns> A list of LineItems that have been included </returns>
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

        /// <summary>
        ///     grabs a specific Line by url and id using two as there is a composite key, and redirects to an new page
        ///     where an update can be preformed
        /// </summary>
        /// <param name="prodID"> string </param>
        /// <param name="inID"> int </param>
        /// <returns> Update View with grabed information </returns>
        [HttpGet]
        public ActionResult LineItemUpsert(string prodID, int inID)
        {
            BooksEntities context = new BooksEntities();
            LineItemUpsertModel model = new LineItemUpsertModel()
            {
                LineItem = context.InvoiceLineItems.Where(l => l.InvoiceID == inID && l.ProductCode == prodID).FirstOrDefault(),
                ProductList = context.Products.ToList(),
                InvoiceList = context.Invoices.ToList()
            };

            if (model.LineItem == null)
            {
                model.LineItem = new InvoiceLineItem();
            }

            return View(model);
        }

        /// <summary>
        ///     Gets the updated info from the update page, adds the new lineItem to the list 
        ///     and returns the list page with the updated information
        /// </summary>
        /// <param name="model"> model that is updating </param>
        /// <returns> Updated list of info </returns>
        [HttpPost]
        public ActionResult LineItemUpsert(LineItemUpsertModel model)
        {
            BooksEntities context = new BooksEntities();
            InvoiceLineItem lineItem = model.LineItem;

            try
            {
                if (context.InvoiceLineItems.Where(i => i.InvoiceID == lineItem.InvoiceID && i.ProductCode == lineItem.ProductCode).Count() > 0)
                {
                    //States already exists
                    var check = context.InvoiceLineItems.Where(i => i.InvoiceID == lineItem.InvoiceID && i.ProductCode == lineItem.ProductCode).FirstOrDefault();

                    check.ItemTotal = lineItem.ItemTotal;
                    check.Quantity = lineItem.Quantity;
                    check.UnitPrice = lineItem.UnitPrice;
                    check.IsDeleted = false;
                }
                else
                {
                    context.InvoiceLineItems.Add(lineItem);
                }

                context.SaveChanges();
            }
            catch (Exception er)
            {

                throw (er);
            }

            return RedirectToAction("AllLineItems");
        }

        /// <summary>
        ///     Soft deletes a LineItem based on the id that is sent, and redirects back to 
        ///     the list page with an updated list of LineItems
        /// </summary>
        /// <param name="inID"> int </param>
        /// <param name="prodID"> String </param>
        /// <returns> Updated list view </returns>
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

        /// <summary>
        ///     Gets the sent id and filter and searches for lineItems that match the
        ///     search value and returns an updated list
        /// </summary>
        /// <param name="id"> Value being filtered </param>
        /// <param name="filter"> Section being filtered </param>
        /// <param name="lineItems"> List of Lineitems </param>
        /// <returns> an updated list of LineItems </returns>
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