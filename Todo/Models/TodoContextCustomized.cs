using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Todo.Dtos;

#nullable disable

namespace Todo.Models
{
    public partial class TodoContext : DbContext
    {
        public virtual DbSet<TodoListSelectDto> TodoListSelectDto { get; set; }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {          
            modelBuilder.Entity<TodoListSelectDto>().HasNoKey();
        }
        
    }
}
