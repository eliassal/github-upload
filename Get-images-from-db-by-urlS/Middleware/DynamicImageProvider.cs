using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// https://www.codeproject.com/Articles/5278879/Serving-Images-Stored-in-a-Database-through-a-Stat?msg=5762516#xx5762516xx

namespace Get_images_from_db_by_urlS.Middleware
{
    public class DynamicImageProvider
    {
        private readonly RequestDelegate next;
        public IServiceProvider service;
        private string pathCondition = "/dynamic/images/";

        private static readonly string[] suffixes = new string[] {".png", ".jpg", ".gif"};
        public DynamicImageProvider(RequestDelegate next)
        {
            this.next = next;
        }

        private bool IsImagePath(PathString path)
        {
            if (path == null || !path.HasValue)
                return false;

            return suffixes.Any
                   (x => path.Value.EndsWith(x, StringComparison.OrdinalIgnoreCase));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var path = context.Request.Path;
                if (IsImagePath(path) && path.Value.Contains(pathCondition))
                {
                    byte[] buffer = null;
                    var id = Guid.Parse(Path.GetFileName(path).Split(".")[0]);

                    using (var db = new EFContext())
                    {
                        var imageFromDb =
                           await db.Images.SingleOrDefaultAsync(x => x.Id == id);
                        buffer = imageFromDb.Data;
                    }

                    if (buffer != null && buffer.Length > 1)
                    {
                        context.Response.ContentLength = buffer.Length;
                        context.Response.ContentType = "image/jpeg";
                            //+ Path.GetExtension(path).Replace(".", "");
                        await context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
                    }
                    else
                        context.Response.StatusCode = 404;
                }
            }
            finally
            {
                if (!context.Response.HasStarted)
                    await next(context);
            }
        }
    }
}
