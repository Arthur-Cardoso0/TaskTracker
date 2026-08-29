using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TaskTracker.Models
{
    public class Admin
    {
        public int Id {get; set;}
        public string Username {get; set;} = string.Empty;
        public string SenhaHash {get; set;} = string.Empty;
      
    }
}