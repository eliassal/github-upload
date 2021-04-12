using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Get_images_from_db_by_urlS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Get_images_from_db_by_urlS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {

        [HttpPost("image")]
        public async Task<Guid> Image(IFormFile image)
        {
            using (var db = new EFContext())
            {
                using (var ms = new MemoryStream())
                {
                    image.CopyTo(ms);
                    var img = new Image()
                    {
                        Suffix = Path.GetExtension(image.FileName),
                        Data = ms.ToArray()
                    };

                    db.Images.Add(img);
                    await db.SaveChangesAsync();
                    return img.Id;
                }
            }
        }
    }
}
