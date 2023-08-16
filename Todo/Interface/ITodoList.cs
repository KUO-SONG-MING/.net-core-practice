using System;
using System.Collections.Generic;
using Todo.Dtos;
using Todo.Parameters;

namespace Todo.Interface
{
    public interface ITodoList
    {
        string type { get; }
        List<TodoListSelectDto> getTodoList(TodoParameters todoParameters);
    }
}
