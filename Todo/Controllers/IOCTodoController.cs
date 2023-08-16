using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Todo.Dtos;
using Todo.Interface;
using Todo.Parameters;

namespace Todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles="admin")]
    public class IOCTodoController : Controller
    {
        private readonly IEnumerable<ITodoList> _todoListService;

        public IOCTodoController(IEnumerable<ITodoList> todoListService) 
        {
            _todoListService = todoListService;
        }

      
        [HttpGet]
        public IActionResult getTodoList([FromQuery] TodoParameters todoParameters)
        {
            ITodoList todoService;
            if (!string.IsNullOrEmpty(todoParameters.type) && todoParameters.type.Equals("mock"))
                todoService = _todoListService.FirstOrDefault(x => x.type == "mock");
            else
                todoService = _todoListService.FirstOrDefault(x => x.type == "real");

            List<TodoListSelectDto> reuslt = todoService.getTodoList(todoParameters);
            return Ok(reuslt);
        }
    }
}
