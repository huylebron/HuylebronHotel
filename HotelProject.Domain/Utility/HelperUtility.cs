﻿using Microsoft . AspNetCore . Http ;
using Microsoft . Extensions . Primitives ;
using Newtonsoft . Json ;

namespace HotelProject.Domain.Utility ;

public static class HelperUtility
{
    public static T? GetValueHeader<T>(HttpRequest request, string key) where T : class
    {
        if (request.Headers.Any(x => x.Key == key))
        {

            StringValues valueHeader = StringValues.Empty;

            request.Headers.TryGetValue(key, out valueHeader);

            string valueHeaderJson = valueHeader.FirstOrDefault() ?? String.Empty;

            if (!string.IsNullOrEmpty(valueHeaderJson))
            {
                return JsonConvert.DeserializeObject<T>(valueHeaderJson);
            }
        }
        return default;
    }

    public static string SerializeObject<T>(T obj)
    {
        var result = JsonConvert.SerializeObject(obj);
        return result;
    }

    public static T DeserializeObject<T>(string jsonContent)
    {
        var result = JsonConvert.DeserializeObject<T>(jsonContent);
        return result;
    }
}