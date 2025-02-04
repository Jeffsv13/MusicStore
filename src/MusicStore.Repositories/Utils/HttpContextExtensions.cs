﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MusicStore.Repositories.Utils;

public static class HttpContextExtensions
{
    public async static Task InsertarPaginacionHeader<T>(this HttpContext httpContext, IQueryable<T> queryable)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }
        List<T> totalRecords = await queryable.ToListAsync();
        httpContext.Response.Headers.Add("TotalRecordsQuantity", totalRecords.Count.ToString());//ojo el nombre del parametro que usaremos, más adelante lo usaré en CORS
    }
}