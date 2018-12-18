using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestVueWebApplication.Interface;
using TestVueWebApplication.Models;
using TestVueWebApplication.Repositories;

namespace TestVueWebApplication.Controllers
{
    public class TodoController : ApiController
    {
        static readonly ITodoListRepository repository = new TodoListRepository();
        public IEnumerable GetAllTodo()
        {
            return repository.GetAll();
        }

        public TodoList PostTodo(TodoList item)
        {
            return repository.Add(item);
        }

        public IEnumerable PutTodo(int id, TodoList todo)
        {
            todo.ID = id;
            if (repository.Update(todo))
            {
                return repository.GetAll();

            }
            else
            {
                return null;
            }
        }

        public bool DeleteTodo(int id)
        {
            if (repository.Delete(id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
