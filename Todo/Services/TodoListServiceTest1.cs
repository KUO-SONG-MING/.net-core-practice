using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Todo.Dtos;
using Todo.Interface;
using Todo.Models;
using Todo.Parameters;

namespace Todo.Services
{
    public class TodoListServiceTest1 : ITodoList
    {
        public string type => "real";

        private TodoContext _todoContext;

        public TodoListServiceTest1(TodoContext todoContext) 
        {
            this._todoContext = todoContext;
        }

        public List<TodoListSelectDto> getTodoList(TodoParameters todoParameters)
        {
            var todoListSelectDtoList = _todoContext.TodoLists.
                                       Include(x => x.InsertEmployee).Include(x => x.UpdateEmployee)
                                       .Select(x => new TodoListSelectDto
                                       {
                                           TodoId = x.TodoId,
                                           Name = x.Name,
                                           InsertTime = x.InsertTime,
                                           UpdateTime = x.UpdateTime,
                                           Enable = x.Enable,
                                           Orders = x.Orders,
                                           InsertEmployeeName = x.InsertEmployee.Name,
                                           UpdateEmployeeName = x.UpdateEmployee.Name
                                       });

            if (todoListSelectDtoList == null || todoListSelectDtoList.Count() == 0)
            {
                return new List<TodoListSelectDto>();
            }

            if (!string.IsNullOrEmpty(todoParameters.name))
            {
                todoListSelectDtoList = todoListSelectDtoList.Where(x => x.Name.Contains(todoParameters.name));
            }

            if (todoParameters.minOrder.HasValue && todoParameters.maxOrder.HasValue)
            {
                todoListSelectDtoList = todoListSelectDtoList.Where(x => x.Orders >= todoParameters.minOrder.Value && x.Orders <= todoParameters.maxOrder);
            }

            //return _iMapper.Map<IEnumerable<TodoListSelectDto>>(todoListSelectDtoList);
            return todoListSelectDtoList.ToList();
        }
    }
}
