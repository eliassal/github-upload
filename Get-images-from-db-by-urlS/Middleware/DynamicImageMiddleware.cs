using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Get_images_from_db_by_urlS.Middleware
{
    public static class DynamicImageMiddleware
    {
        public static IApplicationBuilder UseDynamicImageMiddleware
                                          (this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DynamicImageProvider>();
        }
    }
}
