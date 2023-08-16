using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Todo.Dtos;
using Todo.Models;
using Todo.Parameters;

namespace Todo.Services
{
    public class TodoListService
    {
        private readonly TodoContext _todoContext;
        private readonly IMapper _iMapper;

        public TodoListService(TodoContext todoContext, IMapper iMapper) 
        {
            this._todoContext = todoContext;
            this._iMapper = iMapper;
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

        public TodoListSelectDto getTodoListById(Guid id)
        {
            var result = (from x in _todoContext.TodoLists
                          join InsertEmployee in _todoContext.Employees on x.InsertEmployeeId equals InsertEmployee.EmployeeId
                          join UpdateEmployee in _todoContext.Employees on x.UpdateEmployeeId equals UpdateEmployee.EmployeeId
                          where id == x.TodoId
                          select new TodoListSelectDto
                          {
                              TodoId = x.TodoId,
                              Name = x.Name,
                              InsertTime = x.InsertTime,
                              UpdateTime = x.UpdateTime,
                              Enable = x.Enable,
                              Orders = x.Orders,
                              InsertEmployeeName = InsertEmployee.Name,
                              UpdateEmployeeName = UpdateEmployee.Name
                          }).SingleOrDefault();

            return result;
        }

        public IList<TodoListSelectDto> GetWithSQL(string name)
        {
            string sql = @"select TodoList.*, a.Name as InsertEmployeeName, b.Name as UpdateEmployeeName from TodoList
                           inner join Employee a on TodoList.InsertEmployeeId = a.EmployeeId 
                           inner join Employee b on TodoList.UpdateEmployeeId = b.EmployeeId where 1 = 1";

            if (!string.IsNullOrEmpty(name))
            {
                sql += " and TodoList.Name like N'%" + name + "%'";
            }
            var result = _todoContext.TodoListSelectDtos.FromSqlRaw(sql);
            return result.ToList();
        }

        public TodoList Post(TodoList value) 
        {
            DateTime time = DateTime.Now;
            TodoList todoList = new TodoList
            {
                Name = value.Name,
                InsertTime = time,
                UpdateTime = time,
                Enable = value.Enable,
                Orders = value.Orders,
                InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            };

            _todoContext.TodoLists.Add(todoList);
            _todoContext.SaveChanges();
            return todoList;
        }

        public TodoList PostByCurrentValues(TodoPostData value) 
        {
            DateTime time = DateTime.Now;
            TodoList todoList = new TodoList
            {
                InsertTime = time,
                UpdateTime = time,
                InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            };

            _todoContext.TodoLists.Add(todoList).CurrentValues.SetValues(value);
            _todoContext.SaveChanges();
            return todoList;
        }

        public void PostBySql(TodoPostData value) 
        {
            DateTime time = DateTime.Now;
            string sql = @"insert into TodoList (Name,InsertTime,UpdateTime,Enable,Orders,InsertEmployeeId,UpdateEmployeeId) 
                           values (@Name,@InsertTime,@UpdateTime,@Enable,@Orders,@InsertEmployeeId,@UpdateEmployeeId)";
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("Name",value.Name),
                new SqlParameter("InsertTime",time),
                new SqlParameter("UpdateTime",time),
                new SqlParameter("Enable",value.Enable),
                new SqlParameter("Orders",value.Orders),
                new SqlParameter("InsertEmployeeId", Guid.Parse("00000000-0000-0000-0000-000000000001")),
                new SqlParameter("UpdateEmployeeId", Guid.Parse("00000000-0000-0000-0000-000000000001"))
            };

            _todoContext.Database.ExecuteSqlRaw(sql, param);
            _todoContext.SaveChanges();
        }

        public TodoList Put(Guid id, TodoPostData value) 
        {
            //_todoContext.TodoLists.Update(value); 此方法需要與資料表同型別的參數傳入
            //_todoContext.SaveChanges();

            TodoList update = _todoContext.TodoLists.Find(id);
            if (update == null)
            {
                return null;
            }

            DateTime time = DateTime.Now;

            update.Name = value.Name;
            update.Orders = value.Orders;
            update.Enable = value.Enable;
            update.InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            update.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            update.InsertTime = time;
            update.UpdateTime = time;
            _todoContext.SaveChanges();
            return update;
        }

        public TodoList Patch(Guid id, JsonPatchDocument value) 
        {
            TodoList update = _todoContext.TodoLists.Find(id);
            if (update == null)
            {
                return null;
            }

            DateTime time = DateTime.Now;

            update.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            update.UpdateTime = time;

            //把局部傳來的參數加入既有的數據中
            value.ApplyTo(update);

            _todoContext.SaveChanges();
            return update;
        }

        public void Delete(Guid id) 
        {
            TodoList target = (from data in _todoContext.TodoLists
                               where data.TodoId == id
                               select data).SingleOrDefault();
            if (target != null)
            {
                _todoContext.TodoLists.Remove(target);
                _todoContext.SaveChanges();
            }
        }

        public void Delete(string ids)
        {
            //運用json轉物件轉換路徑上的json格式字串
            List<Guid> deleteIds = JsonSerializer.Deserialize<List<Guid>>(ids);

            List<TodoList> target = (from data in _todoContext.TodoLists
                                     where deleteIds.Contains(data.TodoId)
                                     select data).ToList();

            if (target.Count() > 0)
            {
                _todoContext.TodoLists.RemoveRange(target);
                _todoContext.SaveChanges();
            }
        }
    }
}
