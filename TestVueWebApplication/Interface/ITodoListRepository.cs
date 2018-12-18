using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestVueWebApplication.Models;

namespace TestVueWebApplication.Interface
{
    interface ITodoListRepository
    {
        IEnumerable<TodoList> GetAll();
        TodoList Get(int id);
        TodoList Add(TodoList item);
        bool Update(TodoList item);
        bool Delete(int id);
    }
}
