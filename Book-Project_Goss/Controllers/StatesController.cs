using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Book_Project_Goss.Models;

namespace BookProject.Controllers
{
    public class StatesController : Controller
    {
        // GET: States
        public ActionResult AllStates(string id, string filter = "All", int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<State> states = context.States.ToList();

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc == true)
                        {
                            states = context.States.OrderByDescending(s => s.StateName).ToList();
                        }
                        else
                        {
                            states = context.States.OrderBy(s => s.StateName).ToList();
                        }
                        break;
                    }

                default:
                    {
                        if (isDesc == true)
                        {
                            states = context.States.OrderByDescending(s => s.StateCode).ToList();
                        }
                        else
                        {
                            states = context.States.OrderBy(s => s.StateCode).ToList();
                        }
                        break;
                    }
            }

            if (id != null)
            {
                states = SearchStates(id, filter, states);
            }

            states = states.Where(s => s.IsDeleted == false).ToList();

            return View(states);
        }

        [HttpGet]
        public ActionResult StateUpsert(string id)
        {
            BooksEntities context = new BooksEntities();
            State state = context.States.Where(c => c.StateCode == id).FirstOrDefault();

            if (state == null)
            {
                state = new State();
            }

            return View(state);
        }

        [HttpPost]
        public ActionResult StateUpsert(State state)
        {
            BooksEntities context = new BooksEntities();

            try
            {
                if (context.States.Where(s => s.StateCode == state.StateCode).Count() > 0)
                {
                    //States already exists
                    var check = context.States.Where(s => s.StateCode == state.StateCode).FirstOrDefault();

                    check.StateName = state.StateName;
                    check.IsDeleted = false;
                }
                else
                {
                    context.States.Add(state);
                }

                context.SaveChanges();
            }
            catch (Exception er)
            {

                throw (er);
            }

            return RedirectToAction("AllStates");
        }

        [HttpGet]
        public ActionResult StateDelete(string id)
        {
            BooksEntities context = new BooksEntities();

            try
            {
                State state = context.States.Where(s => s.StateCode == id).FirstOrDefault();
                state.IsDeleted = true;
                context.SaveChanges();
            }
            catch (Exception er)
            {

                throw (er);
            }

            return RedirectToAction("AllStates");
        }

        private List<State> SearchStates(string id, string filter, List<State> states)
        {
            id = id.ToLower().Trim();

            if (filter == "All")
            {
                states = states.Where(s =>
                    s.StateCode.ToLower().Contains(id) ||
                    s.StateName.ToLower().Contains(id)).ToList();
            }
            else if (filter == "Code")
            {
                states = states.Where(s =>
                    s.StateCode.ToLower().Contains(id)).ToList();
            }
            else if (filter == "Name")
            {
                states = states.Where(s =>
                    s.StateName.ToLower().Contains(id)).ToList();
            }

            return states;
        }
    }
}