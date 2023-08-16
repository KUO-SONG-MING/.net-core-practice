using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Xml.Linq;
using Todo.Dtos;
using Todo.Models;
using Todo.Parameters;
using Todo.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _todoContext;
        private readonly TodoListService _todoListService;

        //依賴注入DI
        public TodoController(TodoContext todoContext,
                              TodoListService todoListService)
        {
            _todoContext = todoContext;
            _todoListService = todoListService;      
        }

        //用Include方法配合Entity的主外key自動帶入另一個資料表的所有值到TodoList類別中
        //request parameters帶入專用model，model屬性可以用regular express來接收parameters
        [HttpGet]
        public IActionResult Get([FromQuery] TodoParameters todoParameters)
        {
            var todoListSelectDtoList = _todoListService.getTodoList(todoParameters);

            if (todoListSelectDtoList == null || todoListSelectDtoList.Count() == 0)
            {
                return NotFound("data is not fund or it is zero size");
            }

            return Ok(todoListSelectDtoList);
        }

        //line與linq lambda的比較
        [HttpGet("Keyword/{name}")]
        public IList<TodoList> GetByKeyword(string name)
        {
            var linq = from x in _todoContext.TodoLists
                       where x.Name == "去開會"
                       select x;

            List<TodoList> lingLambda = _todoContext.TodoLists.Where(x => x.Name == "去開會").ToList();

            return linq.ToList();
        }

        //用linq來join
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            TodoListSelectDto result = _todoListService.getTodoListById(id);

            if (result == null)
                return NotFound("Data is not found");

            return Ok(result);
        }

        //用sql碼撈
        //如果撈出來不是原生entity產生的model，就要手動在context裡面創一個
        [HttpGet("getwithsql")]
        public IActionResult GetWithSQL(string name)
        {
            IList<TodoListSelectDto> result = _todoListService.GetWithSQL(name);

            if (result == null)
                return NotFound("no data is found");

            return Ok(result);
        }

        // entity的post寫法
        [HttpPost]
        public IActionResult Post([FromBody] TodoList value)
        {
            TodoList todoList = _todoListService.Post(value);
            TodoListSelectDto todoListSelectDto = _todoListService.getTodoListById(todoList.TodoId);

            //以註解是可以直接呼叫另一個控制器的寫法，但通常呼叫邏輯層方法即可
            //return CreatedAtAction(nameof(GetById), new { id = todoList.TodoId }, todoList);
            return Ok(todoListSelectDto);
        }

        // 後端給值+post數據自動轉換
        //post數據類型需要跟資料庫寫入類形同才會自動轉換
        [HttpPost("auto")]
        public IActionResult PostByCurrentValues([FromBody] TodoPostData value)
        {
            TodoList todoList = _todoListService.PostByCurrentValues(value);
            TodoListSelectDto todoListSelectDto = _todoListService.getTodoListById(todoList.TodoId);
            return Ok(todoListSelectDto);
        }

        //用sql post資料+防sql injection
        [HttpPost("sql")]
        public IActionResult PostBySql([FromBody] TodoPostData value)
        {
            _todoListService.PostBySql(value);
            return Ok("用sql新增一筆ToLists");
        }

        // entity put寫法
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] TodoPostData value)
        {
            TodoList todoList = _todoListService.Put(id, value);
           
            if (todoList == null) 
            {
                return NotFound("找不到要更新的資料");
            }
            TodoListSelectDto todoListSelectDto = _todoListService.getTodoListById(todoList.TodoId);
            return Ok(todoListSelectDto);
        }

        //需要用NuGet安裝 => Microsoft.AspNetCore.JsonPatch , Microsoft.AspNetCore.Mvc.Newton
        //patch傳來的json格式 => [{"op":"replace","path":"/name","value":"patch局部更新"}]
        //更新的id => 9694c6d4-8cc7-4f13-b600-5f5f18fe53c5
        [HttpPatch("{id}")]
        public IActionResult patch(Guid id, [FromBody] JsonPatchDocument value) 
        {
            TodoList todoList = _todoListService.Patch(id, value);

            if (todoList == null)
            {
                return NotFound("找不到要更新的資料");
            }
            TodoListSelectDto todoListSelectDto = _todoListService.getTodoListById(todoList.TodoId);
            return Ok(todoListSelectDto);
        }

        // DELETE 單筆資料
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _todoListService.Delete(id);
            return Ok("已刪除");
        }

        // DELETE 多筆資料
        [HttpDelete("list/{ids}")]
        public IActionResult Delete(string ids)
        {
            _todoListService.Delete(ids);
            return Ok("已刪除");
        }
    }
}
