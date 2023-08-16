using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Todo.Parameters;

namespace Todo.ValidationAttributes
{
    public class TodoPostDataAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            TodoPostData todoPostData = (TodoPostData)value;

            if (todoPostData.Name.Contains("幹")) 
            {
                return new ValidationResult("名字不能有髒話", new List<string> { "TodoPostData.Name" });
            }
            return ValidationResult.Success;
        }
    }
}
