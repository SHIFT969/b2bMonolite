﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using b2bSwgroup.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;

namespace b2bSwgroup.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user;
                if (model.userType==UserType.Customer)
                {
                    user = new CustomerApplUser { UserName = model.Email, Email = model.Email };
                }
                else
                {
                    user = new DistributorApplicationUser { UserName = model.Email, Email = model.Email };
                }
                
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, "User");
                    //// генерируем токен для подтверждения регистрации
                    //var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //// создаем ссылку для подтверждения
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code },
                    //           protocol: Request.Url.Scheme);
                    //// отправка письма
                    //await UserManager.SendEmailAsync(user.Id, "Подтверждение электронной почты",
                    //           "Для завершения регистрации перейдите по ссылке:: <a href=\""
                    //                                           + callbackUrl + "\">завершить регистрацию</a>");
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            return View(model);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public async Task<ActionResult> Login(string returnUrl)
        {
            
            ViewBag.returnUrl = returnUrl;
            if(UserManager.FindByName("admin")==null)
            {
                var userAdmin = new ApplicationUser();
                userAdmin.Email = "kadet635@gmail.com";
                userAdmin.UserName = "admin";
                IdentityResult result = await UserManager.CreateAsync(userAdmin, "krak91635");
                await UserManager.AddToRoleAsync(userAdmin.Id, "User");
                await UserManager.AddToRoleAsync(userAdmin.Id, "Admin");
                await UserManager.AddToRoleAsync(userAdmin.Id, "Distributor");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindAsync(model.Email, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль.");
                }
                else
                {
                    ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);
                    if (String.IsNullOrEmpty(returnUrl))
                        return RedirectToAction("Index", "PositionCatalogs");
                    return Redirect(returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login");
        }

      
    }
}