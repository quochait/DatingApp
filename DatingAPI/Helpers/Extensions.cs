﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace DatingAPI.Helpers
{
  public static class Extensions
  {
    public static void AddApplicationError(this HttpResponse response, string message)
    {
      response.Headers.Add("Application-Error", message);
      response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
      response.Headers.Add("Access-Control-Allow-Origin", "*");
    }

    public static int CalculateAge(this DateTime theDateTime)
    {
      var age = DateTime.Today.Year - theDateTime.Year;
      if (theDateTime.AddYears(age).Year > DateTime.Today.Year)
      {
        age--;
      }

      return age;
    }

    public static void AddPagitation(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
    {
      var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
      response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader));
      response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
    }
  }
}
