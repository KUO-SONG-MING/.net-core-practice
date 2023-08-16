using AutoMapper;
using Todo.Dtos;
using Todo.Models;

namespace Todo.Profiles
{
    //model自動映射轉換檔
    public class TodoListProfile : Profile
    {
        public TodoListProfile() 
        {
            CreateMap<TodoList, TodoListSelectDto>();
        }
    }
}
