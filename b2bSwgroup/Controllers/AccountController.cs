using System;
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
using b2bSwgroup.Models.ModelsForView;

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
        private ApplicationRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }

        public ActionResult Register()
        {
            return View();
        } 

        public async Task<ActionResult> ConfirmEmail(string userId,string code)
        {
            var user = await UserManager.FindByIdAsync(userId);
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            ViewResultConfirm resultCon = new ViewResultConfirm();
            if (result.Succeeded)
            {
                resultCon.Message = "Аккаунт успешно подтвержден.";
                return View("ConfirmEmail",resultCon );
            }
            else
            {
                resultCon.Message = "Ошибка! Ссылка недействительна!";
                return View("ConfirmEmail", resultCon );
            }

        }
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new CustomerApplUser { UserName = model.Email, Email = model.Email,DateRegistration=DateTime.Now,DateLastLogin=DateTime.Now };
                
                if(!IsValidEmail(user.Email))
                {
                    ModelState.AddModelError("", "Email введен некорректно");
                    return View(model);
                }

                if(await UserManager.FindByEmailAsync(user.Email)!=null)
                {
                    ModelState.AddModelError("", "Пользователь с таким Email уже существует");
                    return View(model);
                }
                if (await UserManager.FindByNameAsync(user.UserName) != null)
                {
                    ModelState.AddModelError("", "Пользователь с таким именем уже существует");
                    return View(model);
                }
                

                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {                    
                    await UserManager.AddToRoleAsync(user.Id, "User");
                    // генерируем токен для подтверждения регистрации
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var code1 = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // создаем ссылку для подтверждения
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code },
                               protocol: Request.Url.Scheme);
                    // отправка письма
                    await UserManager.SendEmailAsync(user.Id, "Подтверждение электронной почты",
                               "Для завершения регистрации перейдите по ссылке:: <a href=\""
                                                               + callbackUrl + "\">завершить регистрацию</a>");


                    ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);
                    return RedirectToAction("Index", "PositionCatalogs");       
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
        [Authorize(Roles ="Admin")]
        public ActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateUser(RegisterModel model)
        {
            if (ModelState.IsValid)
            {

                ApplicationUser user; 

                if(model.userType==UserType.Customer)
                {
                    user = new CustomerApplUser { UserName = model.Email, Email = model.Email, DateRegistration = DateTime.Now, DateLastLogin = DateTime.Now };
                }
                else
                {
                    user = new DistributorApplicationUser { UserName = model.Email, Email = model.Email, DateRegistration = DateTime.Now, DateLastLogin = DateTime.Now };
                }

                if (!IsValidEmail(user.Email))
                {
                    ModelState.AddModelError("", "Email введен некорректно");
                    return View(model);
                }

                if (await UserManager.FindByEmailAsync(user.Email) != null)
                {
                    ModelState.AddModelError("", "Пользователь с таким Email уже существует");
                    return View(model);
                }
                if (await UserManager.FindByNameAsync(user.UserName) != null)
                {
                    ModelState.AddModelError("", "Пользователь с таким именем уже существует");
                    return View(model);
                }


                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if(user is CustomerApplUser)
                    {
                        await UserManager.AddToRoleAsync(user.Id, "User");
                    };
                    if (user is DistributorApplicationUser)
                    {
                        await UserManager.AddToRoleAsync(user.Id, "Distributor");
                    };

                    //// генерируем токен для подтверждения регистрации
                    //var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    ////var code1 = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //// создаем ссылку для подтверждения
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code },
                    //           protocol: Request.Url.Scheme);
                    //// отправка письма
                    //await UserManager.SendEmailAsync(user.Id, "Подтверждение электронной почты",
                    //           "Для завершения регистрации перейдите по ссылке:: <a href=\""
                    //                                           + callbackUrl + "\">завершить регистрацию</a>");


                    //ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user,
                    //                        DefaultAuthenticationTypes.ApplicationCookie);
                    //AuthenticationManager.SignOut();
                    //AuthenticationManager.SignIn(new AuthenticationProperties
                    //{
                    //    IsPersistent = true
                    //}, claim);
                    return RedirectToAction("Index", "Admin");
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
                await RoleManager.CreateAsync(new ApplicationRole() { Name="Admin",Description="Admin" });
                await RoleManager.CreateAsync(new ApplicationRole() { Name = "User", Description = "User" });
                await RoleManager.CreateAsync(new ApplicationRole() { Name = "Distributor", Description = "Distributor" });
                var userAdmin = new ApplicationUser();
                userAdmin.Email = "kadet635@gmail.com";
                userAdmin.UserName = "admin";
                userAdmin.EmailConfirmed = true;
                userAdmin.DateLastLogin = DateTime.Now;
                userAdmin.DateRegistration = DateTime.Now;
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
                    if(!UserManager.IsInRole(user.Id,"Admin"))
                    {
                        if(!UserManager.IsInRole(user.Id, "Distributor"))
                        {
                            if (!user.EmailConfirmed && (DateTime.Now - user.DateRegistration) > TimeSpan.FromHours(24))
                            {
                                ModelState.AddModelError("", "Email не подтвержден, аккаунт заблокирован");
                                return View(model);
                            }
                        }
                    }
                    
                    ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);
                    user.DateLastLogin = DateTime.Now;
                    await UserManager.UpdateAsync(user);
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
            return RedirectToAction("Index","PositionCatalogs");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]        
        public async Task<ActionResult> ChangePassword(ViewResetPassword model)
        {
            if (ModelState.IsValid)
            {
                var thisUser = await UserManager.FindByNameAsync(User.Identity.Name);
                
                if(await UserManager.CheckPasswordAsync(thisUser,model.OldPassword))
                {
                    var resultChande = await UserManager.ChangePasswordAsync(thisUser.Id,model.OldPassword,model.Password);
                    if(resultChande.Succeeded)
                    {
                        return View("CompleteChangePassword");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Изменить пароль не удалось, обратитесь в службу поддержки");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Старый пароль указан неверно");
                    return View(model);
                }
                
            }
            return View(model);
        }

        public ActionResult SendConfirmEmail()
        {
            return View();
        }
        [HttpPost]
        public async Task SendConfirmEmail(ApplicationUser user)
        {
            var myUser = await UserManager.FindByEmailAsync(user.Email);
            if(myUser!=null)
            {
                // генерируем токен для подтверждения регистрации
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(myUser.Id);
                //var code1 = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // создаем ссылку для подтверждения
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = myUser.Id, code = code },
                           protocol: Request.Url.Scheme);
                // отправка письма
                await UserManager.SendEmailAsync(myUser.Id, "Подтверждение электронной почты",
                           "Для завершения регистрации перейдите по ссылке:: <a href=\""
                                                           + callbackUrl + "\">завершить регистрацию</a>");
            }
            RedirectToAction("Login");
        }

    }
}