using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Book_Project_Goss.Models;

namespace BookProject.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult AllCustomers(string id, string filter = "all", int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<Customer> customers = context.Customers.ToList();

            switch (sortBy)
            {
                case 1:
                    {
                        if(isDesc == true)
                        {
                            customers = context.Customers.OrderByDescending(c => c.Name).ToList();
                        }
                        else
                        {
                            customers = context.Customers.OrderBy(c => c.Name).ToList();
                        }
                        break;
                    }
                case 2:
                    {
                        if (isDesc == true)
                        {
                            customers = context.Customers.OrderByDescending(c => c.State).ToList();
                        }
                        else
                        {
                            customers = context.Customers.OrderBy(c => c.State).ToList();
                        }
                        break;
                    }
                case 3:
                    {
                        if (isDesc == true)
                        {
                            customers = context.Customers.OrderByDescending(c => c.Address).ToList();
                        }
                        else
                        {
                            customers = context.Customers.OrderBy(c => c.Address).ToList();
                        }
                        break;
                    }
                default:
                    {
                        if (isDesc == true)
                        {
                            customers = context.Customers.OrderByDescending(c => c.CustomerID).ToList();
                        }
                        else
                        {
                            customers = context.Customers.OrderBy(c => c.CustomerID).ToList();
                        }
                        break;
                    }
            }

            if (id != null)
            {
                customers = CustomerSearch(id, filter, customers);
            }

            customers = customers.Where(c => c.IsDeleted == false).ToList();

            return View(customers);
        }

        [HttpGet]
        public ActionResult CustomerUpsert(int id)
        {
            BooksEntities context = new BooksEntities();
            CustomerUpsertModel model = new CustomerUpsertModel()
            {
                Customer = context.Customers.Where(c => c.CustomerID == id).FirstOrDefault(),
                StateList = context.States.ToList()
            };

            if (model.Customer == null)
            {
                model.Customer = new Customer();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CustomerUpsert(CustomerUpsertModel model)
        {
            BooksEntities context = new BooksEntities();
            Customer customer = model.Customer;
            customer.ZipCode = customer.ZipCode.Trim();

            try
            {
                if (context.Customers.Where(c => c.CustomerID == customer.CustomerID).Count() > 0)
                {
                    //States already exists
                    var check = context.Customers.Where(c => c.CustomerID == customer.CustomerID).FirstOrDefault();

                    check.Address = customer.Address;
                    check.City = customer.City;
                    check.Name = customer.Name;
                    check.State = customer.State;
                    check.ZipCode = customer.ZipCode;
                    check.IsDeleted = false;
                }
                else
                {
                    context.Customers.Add(customer);
                }

                context.SaveChanges();
            }
            catch (Exception er)
            {

                throw (er);
            }

            return RedirectToAction("AllCustomers");
        }


        [HttpGet]
        public ActionResult CustomerDelete(int id)
        {
            BooksEntities context = new BooksEntities();

            try
            {
                Customer customer = context.Customers.Where(c => c.CustomerID == id).FirstOrDefault();
                customer.IsDeleted = true;
                context.SaveChanges();
            }
            catch (Exception er)
            {

                throw (er);
            }

            return RedirectToAction("AllCustomers");
        }

        /// <summary>
        ///     Gets the sent id and filter and searches for customers that match the
        ///     search value and returns an updated list
        /// </summary>
        /// <param name="id"> Value being filtered </param>
        /// <param name="filter"> Section being filtered </param>
        /// <param name="customers"> List of customers </param>
        /// <returns> an updated list of customers </returns>
        private List<Customer> CustomerSearch(string id, string filter, List<Customer> customers)
        {
            id = id.Trim().ToLower();

            if (filter == "All")
            {
                if(int.TryParse(id, out int num))
                {
                    customers = customers.Where(c => c.CustomerID.ToString().Contains(id.ToString()) || 
                        c.Address.Contains(num.ToString())).ToList();
                }
                else
                {
                    customers = customers.Where(c => 
                        c.Address.ToLower().Contains(id) ||
                        c.Name.ToLower().Contains(id) ||
                        c.State.ToLower().Contains(id)).ToList();
                }
            }
            else if(filter == "Id")
            {
                int.TryParse(id, out int num);

                customers = customers.Where(c =>
                    c.CustomerID.ToString().Contains(id.ToString())).ToList();
            }
            else if (filter == "State")
            {
                customers = customers.Where(c =>
                    c.State.ToLower().Contains(id)).ToList();
            }
            else if (filter == "Name")
            {
                customers = customers.Where(c =>
                    c.Name.ToLower().Contains(id)).ToList();
            }
            else if (filter == "Address")
            {
                customers = customers.Where(c =>
                    c.Address.ToLower().Contains(id)).ToList();
            }

            return customers;
        }
    }
}