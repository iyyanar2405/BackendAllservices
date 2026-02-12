using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AuthProvider.ViewModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Duende.IdentityServer.EntityFramework.Mappers;
using AuthProvider.Models;
using System.Security.Claims;
using Duende.IdentityServer.EntityFramework.Entities;
using AuthProvider.Configuration;
using System.Collections;
using AutoMapper.Internal;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System.Data;

namespace AuthProvider.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Produces("application/json")]
    [Route("api/Dashboard")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
