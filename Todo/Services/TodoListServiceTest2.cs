using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Todo.Dtos;
using Todo.Interface;
using Todo.Parameters;
using System.Security.Claims;

namespace Todo.Services
{
    public class TodoListServiceTest2 : ITodoList
    {
        private IHttpContextAccessor _httpContextAccessor;

        public TodoListServiceTest2(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string type => "mock";
        public List<TodoListSelectDto> getTodoList(TodoParameters todoParameters)
        {
            //取得當初加入jwt的使用者資訊
            List<Claim> claims = _httpContextAccessor.HttpContext.User.Claims.ToList();
            var EmployeeId = claims.Where(user => user.Type == "EmployeeId").FirstOrDefault().Value;
            var EmployeeId2 = claims.Where(user => user.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            
            return new List<TodoListSelectDto> { new TodoListSelectDto{
                Name = "測試service",
                InsertTime = new DateTime(1912,01,01),
                UpdateTime = new DateTime(1912,01,01),
                Enable = false,
                Orders = 999,
                InsertEmployeeName = EmployeeId,
                UpdateEmployeeName = EmployeeId2
            }};   
        }
    }
}
