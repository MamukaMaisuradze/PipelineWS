using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Silverlight_ImageUpload.Web.DTO
{
    public class Picture
    {
        public int ImageID { get; set; }
        public string ImageTitle { get; set; }
        public byte[] ImageStream { get; set; }
    }
}
