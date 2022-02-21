using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToDoAPI.API.Models;
using ToDoAPI.DATA.EF;
using System.Web.Http.Cors;

namespace ToDoAPI.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ToDoController : ApiController
    {
        ToDoEntities db = new ToDoEntities();

        public IHttpActionResult GetToDos()
        {
            List<ToDoViewModel> todos = db.TodoItems.Include("Category").Select(td => new ToDoViewModel()
            {
                ToDoId = td.TodoId,
                Action = td.Action,
                Done = td.Done,
                CategoryId = td.CategoryId,
                Category = new CategoryViewModel()
                {
                    CategoryId = td.CategoryId,
                    Name = td.Category.Name,
                    Description = td.Category.Description
                }
            }).ToList<ToDoViewModel>();

            if (todos.Count == 0)
            {
                return NotFound();
            }
            return Ok(todos);
        }//end GetTodo

        public IHttpActionResult GetToDo(int id)
        {
            ToDoViewModel toDo = db.TodoItems.Include("Category").Where(td => td.TodoId == id).Select(td => new ToDoViewModel()
            {

                ToDoId = td.TodoId,
                Action = td.Action,
                Done = td.Done,
                CategoryId = td.CategoryId,
                Category = new CategoryViewModel()
                {
                    CategoryId = td.CategoryId,
                    Name = td.Category.Name,
                    Description = td.Category.Description
                }
            }).FirstOrDefault();

            if (toDo == null)
                return NotFound();
            return Ok(toDo);
        }//end GetTodo

        public IHttpActionResult PostToDo(ToDoViewModel toDo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            TodoItem newTodo = new TodoItem()
            {
                Action = toDo.Action,
                Done = toDo.Done,
                CategoryId = toDo.CategoryId,
            };

            db.TodoItems.Add(newTodo);
            db.SaveChanges();
            return Ok(newTodo);
        }//end PostTodo

        public IHttpActionResult PutTodo(ToDoViewModel toDo)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Data");

            TodoItem exsistingToDo = db.TodoItems.Where(td => td.TodoId == toDo.ToDoId).FirstOrDefault();

            if (exsistingToDo != null)
            {
                exsistingToDo.TodoId = toDo.ToDoId;
                exsistingToDo.Action = toDo.Action;
                exsistingToDo.Done = toDo.Done;
                exsistingToDo.CategoryId = toDo.CategoryId;
                    return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end PutToDo

       public IHttpActionResult DeleteToDo(int id)
        {
            TodoItem todo = db.TodoItems.Where(td => td.TodoId == id).FirstOrDefault();

            if (todo != null)
            {
                db.TodoItems.Remove(todo);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end DeleteToDo

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }//end class
}//end namespace
