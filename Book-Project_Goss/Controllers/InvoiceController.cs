using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Book_Project_Goss.Models;

namespace BookProject.Controllers
{
    public class InvoiceController : Controller
    {
        // GET: Invoice
        /// <summary>
        /// 
        /// </summary>
        /// <returns> List of Invoices </returns>
        public ActionResult AllInvoices(string id, string filter = "All", int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<Invoice> invoices = context.Invoices.ToList();

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc == true)
                        {
                            invoices = context.Invoices.OrderByDescending(i => i.Customer.Name).ToList();
                        }
                        else
                        {
                            invoices = context.Invoices.OrderBy(i => i.Customer.Name).ToList();
                        }
                        break;
                    }
                case 2:
                    {
                        if (isDesc == true)
                        {
                            invoices = context.Invoices.OrderByDescending(i => i.ProductTotal).ToList();
                        }
                        else
                        {
                            invoices = context.Invoices.OrderBy(i => i.ProductTotal).ToList();
                        }
                        break;
                    }
                case 3:
                    {
                        if (isDesc == true)
                        {
                            invoices = context.Invoices.OrderByDescending(i => i.SalesTax).ToList();
                        }
                        else
                        {
                            invoices = context.Invoices.OrderBy(i => i.SalesTax).ToList();
                        }
                        break;
                    }
                case 4:
                    {
                        if (isDesc == true)
                        {
                            invoices = context.Invoices.OrderByDescending(i => i.InvoiceTotal).ToList();
                        }
                        else
                        {
                            invoices = context.Invoices.OrderBy(i => i.InvoiceTotal).ToList();
                        }
                        break;
                    }
                case 5:
                    {
                        if (isDesc == true)
                        {
                            invoices = context.Invoices.OrderByDescending(i => i.InvoiceDate).ToList();
                        }
                        else
                        {
                            invoices = context.Invoices.OrderBy(i => i.InvoiceDate).ToList();
                        }
                        break;
                    }
                default:
                    {
                        if (isDesc == true)
                        {
                            invoices = context.Invoices.OrderByDescending(i => i.InvoiceID).ToList();
                        }
                        else
                        {
                            invoices = context.Invoices.OrderBy(i => i.InvoiceID).ToList();
                        }
                        break;
                    }
            }

            if (id != null)
            {
                invoices = InvoiceSearch(id, filter, invoices);
            }

            invoices = invoices.Where(i => i.IsDeleted == false).ToList();

            return View(invoices);
        }

        [HttpGet]
        public ActionResult InvoiceUpsert(int id)
        {
            BooksEntities context = new BooksEntities();
            InvoiceUpsertModel model = new InvoiceUpsertModel()
            {
                CustomerList = context.Customers.ToList(),
                Invoice = context.Invoices.Where(i => i.InvoiceID == id).FirstOrDefault()
            };

            if (model.Invoice == null)
            {
                model.Invoice = new Invoice();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult InvoiceUpsert(InvoiceUpsertModel model)
        {
            BooksEntities context = new BooksEntities();
            Invoice invoice = CalculateIvoice(model.Invoice);

            try
            {
                if (context.Invoices.Where(i => i.InvoiceID == invoice.InvoiceID).Count() > 0)
                {
                    //States already exists
                    var check = context.Invoices.Where(i => i.InvoiceID == invoice.InvoiceID).FirstOrDefault();

                    check.CustomerID = invoice.CustomerID;
                    check.InvoiceDate = invoice.InvoiceDate;
                    check.InvoiceTotal = invoice.InvoiceTotal;
                    check.ProductTotal = invoice.ProductTotal;
                    check.SalesTax = invoice.SalesTax;
                    check.Shipping = invoice.Shipping;
                }
                else
                {
                    context.Invoices.Add(invoice);
                }

                context.SaveChanges();
            }
            catch (Exception er)
            {

                throw (er);
            }

            return RedirectToAction("AllInvoices");
        }
        
        /// <summary>
        ///     Calculates the totals for the updated / added invoice
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        private Invoice CalculateIvoice(Invoice invoice)
        {
            BooksEntities context = new BooksEntities();

            List<InvoiceLineItem> items = context.InvoiceLineItems.Where(i => i.InvoiceID == invoice.InvoiceID && i.IsDeleted == false).ToList();

            invoice.ProductTotal = 0;
            foreach (var item in items)
            {
                invoice.ProductTotal = invoice.ProductTotal + (item.ItemTotal * item.Quantity);
            }

            invoice.InvoiceTotal = invoice.ProductTotal + invoice.Shipping + invoice.SalesTax;
            return invoice;
        }

        [HttpGet]
        public ActionResult InvoiceDelete(int id)
        {
            BooksEntities context = new BooksEntities();

            try
            {
                Invoice invoice = context.Invoices.Where(i => i.InvoiceID == id).FirstOrDefault();
                invoice.IsDeleted = true;
                context.SaveChanges();
            }
            catch (Exception er)
            {

                throw (er);
            }

            return RedirectToAction("AllInvoices");
        }

        private List<Invoice> InvoiceSearch(string id, string filter, List<Invoice> invoices)
        {
            id = id.Trim().ToLower();

            if (filter == "All")
            {
                if (int.TryParse(id, out int num))
                {
                    invoices = invoices.Where(i =>
                        i.InvoiceID.ToString().Contains(num.ToString()) ||
                        i.SalesTax.ToString().Contains(num.ToString()) ||
                        i.ProductTotal.ToString().Contains(num.ToString()) ||
                        i.InvoiceTotal.ToString().Contains(num.ToString()) ||
                        i.InvoiceDate.ToString().Contains(num.ToString())).ToList();
                }
                else
                {
                    invoices = invoices.Where(i =>
                        i.InvoiceDate.ToString().ToLower().Contains(id) ||
                        i.Customer.Name.ToLower().Contains(id)).ToList();
                }
            }
            else if (filter == "Id")
            {
                double.TryParse(id, out double num);

                invoices = invoices.Where(c =>
                    c.InvoiceID.ToString().Contains(id.ToString())).ToList();
            }
            else if (filter == "Cust")
            {
                invoices = invoices.Where(c =>
                    c.Customer.Name.ToLower().Contains(id.ToString())).ToList();
            }
            else if (filter == "ProTotal")
            {
                double.TryParse(id, out double num);

                invoices = invoices.Where(c =>
                    c.ProductTotal.ToString().Contains(id.ToString())).ToList();
            }
            else if (filter == "InTotal")
            {
                double.TryParse(id, out double num);

                invoices = invoices.Where(c =>
                    c.InvoiceTotal.ToString().Contains(id.ToString())).ToList();
            }
            else if (filter == "Tax")
            {
                double.TryParse(id, out double num);

                invoices = invoices.Where(c =>
                    c.SalesTax.ToString().Contains(id.ToString())).ToList();
            }
            else if (filter == "Date")
            {
                invoices = invoices.Where(c =>
                    c.InvoiceDate.ToString().ToLower().Contains(id.ToString())).ToList();
            }

            return invoices;
        }
    }
}