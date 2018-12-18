using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestVueWebApplication.Interface;
using TestVueWebApplication.Models;

namespace TestVueWebApplication.Repositories
{
    public class TodoListRepository : ITodoListRepository
    {
        private TestVueWebApplicationContext db = new TestVueWebApplicationContext();

        public IEnumerable<TodoList> GetAll()
        {
            return db.TodoLists.ToList();
        }

        public TodoList Get(int id)
        {

            return db.TodoLists.Find(id);
        }

        public TodoList Add(TodoList item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            db.TodoLists.Add(item);
            db.SaveChanges();
            return item;
        }

        public bool Update(TodoList item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            var todo = db.TodoLists.Single(t => t.ID == item.ID);
            todo.Name = item.Name;
            todo.Task = item.Task;
            db.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            // TO DO : Code to remove the records from database
            TodoList ts = db.TodoLists.Find(id);
            db.TodoLists.Remove(ts);
            db.SaveChanges();
            return true;
        }
    }
}