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
        /// <summary>
        ///     Displays a list of States to the user. Allows user to search through the list
        ///     of States, for all columns or with filters and sorts list either ascending or descending
        /// </summary>
        /// <param name="id"> search term - string </param>
        /// <param name="filter"> filter selection - string </param>
        /// <param name="sortBy"> sort column - int </param>
        /// <param name="isDesc"> sort direction - boolean </param>
        /// <returns> A list of States that have been included </returns>
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

        /// <summary>
        ///     grabs a State by url and id, and redirects to an new page
        ///     where an update can be preformed with the info grabbed from the selected state object
        /// </summary>
        /// <param name="id"> string </param>
        /// <returns> Update page with grabbed info from State object </returns>
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

        /// <summary>
        ///     Gets the updated info from the update page, adds the new State to the list 
        ///     and returns the list page with the updated information
        /// </summary>
        /// <param name="state"> state being updated </param>
        /// <returns> Updated list of State info </returns>
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

        /// <summary>
        ///     Soft deletes a State based on the id that is sent, and redirects back to 
        ///     the list page with updated list of States
        /// </summary>
        /// <param name="id"> string </param>
        /// <returns> List View with updated States </returns>
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

        /// <summary>
        ///     Gets the sent id and filter and searches for the states that match the
        ///     search value and returns an updated list
        /// </summary>
        /// <param name="id"> Value being filtered </param>
        /// <param name="filter"> Section being filtered </param>
        /// <param name="states"> List of states </param>
        /// <returns> an updated list of states </returns>
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