using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CommonLayer.RequestModel
{
    public class ImageUploadModel
    {
       public int StudenId { get; set; }
       public IFormFile File { get; set; }
    }
}
