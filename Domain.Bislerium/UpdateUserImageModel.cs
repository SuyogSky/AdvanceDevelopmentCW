using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium
{
    public class UpdateUserImageModel
    {
        public IFormFile? ImageFile { get; set; }
    }
}