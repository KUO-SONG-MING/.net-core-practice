using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Todo.Models;
using Todo.ValidationAttributes;

namespace Todo.Parameters
{
    [TodoPostDataAttribute]
    public class TodoPostData:IValidatableObject
    {
        [Required]
        [RegularExpression("\\w+")]    
        public string Name { get; set; }
        public bool Enable { get; set; }
        public int Orders { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //此測試不用再入Entity
            //var todoContext = (TodoContext)validationContext.GetService(typeof(TodoContext));

            //List<ValidationResult> test = new List<ValidationResult>();
            if (Orders < 0)
            {
                //test.Add(new ValidationResult("1Orders不得小於0", new string[] { "TodoPostData.Orders" }));
                yield return new ValidationResult("1Orders不得小於0", new string[] { "TodoPostData.Orders" });
            }
            if (Orders < 0)
            {
                //test.Add(new ValidationResult("2Orders不得小於0", new string[] { "TodoPostData.Orders" }));
                yield return new ValidationResult("2Orders不得小於0", new string[] { "TodoPostData.Orders" });
            }

            //return test;

            //備註:yield功能為先return可以傳回的物件，之後繼續執行程式碼，所以return的結果可以為Enumerable
            yield return ValidationResult.Success;
            
        }
    }
}
