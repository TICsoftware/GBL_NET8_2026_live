using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.BLL;
using Core_project_BusinessLogic.Entity.Manage;
using GBL_MVC.Filters;
using GBL_MVC.Helpers;
using GBL_MVC.Models.Manage_Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using OtpNet;
using QRCoder;
using SimpleBase;

namespace GBL_MVC.Controllers.Manage
{
    public class ManageController : Controller
    {
        private readonly ILogger<ManageController> _logger;
        private readonly IConfiguration objconfig;
        private readonly User_bal _bal;

        public ManageController(ILogger<ManageController> logger, IConfiguration configuration)
        {
            _logger = logger;
            objconfig = configuration;
            _bal = new User_bal(configuration);
        }


        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Username and password are required.";
                return View();
            }

            try
            {
                var user = _bal.Users_Login_bal(username);

                if (user == null)
                {
                    _bal.InsertLoginLog_bal(username, ipAddress, "Login Failed");
                    ViewBag.Error = "Invalid username or password";
                    return View();
                }

                var hasher = new PasswordHasher<string>();

                var result = hasher.VerifyHashedPassword(null, user.password, password);

                if (result != PasswordVerificationResult.Success)
                {
                    _bal.InsertLoginLog_bal(username, ipAddress, "Invalid Password");
                    ViewBag.Error = "Invalid username or password";
                    return View();
                }

                if (!user.Is2FAEnabled)
                {
                    _bal.InsertLoginLog_bal(username, ipAddress, "2FA Not Enabled");
                    ViewBag.Error = "2FA is required to access this system.";
                    return View();
                }

                HttpContext.Session.SetString("UserId", user.user_id.ToString());
                HttpContext.Session.SetString("UserSecret", user.secret ?? "");
                HttpContext.Session.SetString("Username", user.username ?? "");
                HttpContext.Session.SetString("UserTypeId", user.UserType_Id.ToString());

                _bal.InsertLoginLog_bal(username, ipAddress, "Login Success");

                if (user.UserType_Id == 5)
                {
                    return RedirectToAction("SetSuperadmin", "Manage");
                }

                //live
                return RedirectToAction("Verify2FA", "Manage");

                //return RedirectToAction("Dashboard", "Manage");
            }
            catch (Exception ex)
            {
                _bal.InsertLoginLog_bal(username, ipAddress, "Login Error: " + ex.Message);
                ViewBag.Error = "Something went wrong. Please try again.";
                return View();
            }
        }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public IActionResult Login(string username, string password)
        // {
        //     string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

        //     if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        //     {
        //         ViewBag.Error = "Username and password are required.";
        //         return View();
        //     }

        //     try
        //     {

        //         var hasher = new PasswordHasher<string>();
        //         string hashedPassword = hasher.HashPassword(null, password);

        //         var entity = new User
        //         {
        //             username = username,
        //             password = hashedPassword
        //         };

        //         var user = _bal.Users_Login_bal(entity);


        //         var result = hasher.VerifyHashedPassword(
        //                 null,
        //                 user.password,
        //                 password
        //             );

        //         if (result == PasswordVerificationResult.Success)
        //         {
        //             // login success
        //         }


        //         if (user == null)
        //         {
        //             _bal.InsertLoginLog_bal(username, ipAddress, "Login Failed");
        //             ViewBag.Error = "Invalid username or password";
        //             return View();
        //         }


        //         if (!user.Is2FAEnabled)
        //         {
        //             _bal.InsertLoginLog_bal(username, ipAddress, "2FA Not Enabled");
        //             ViewBag.Error = "2FA is required to access this system.";
        //             return View();
        //         }

        //         // ✅ Store TEMP session (NOT logged in yet)
        //         HttpContext.Session.SetString("UserId", user.user_id.ToString());
        //         HttpContext.Session.SetString("UserSecret", user.secret ?? string.Empty);
        //         HttpContext.Session.SetString("Username", user.username ?? string.Empty);
        //         HttpContext.Session.SetString("UserTypeId", user.UserType_Id.ToString());

        //         // ✅ Log success
        //         _bal.InsertLoginLog_bal(username, ipAddress, "Login Success");

        //         // ✅ Redirect to OTP screen
        //         return RedirectToAction("Verify2FA", "Manage");
        //     }
        //     catch (Exception ex)
        //     {
        //         _bal.InsertLoginLog_bal(username, ipAddress, "Login Error: " + ex.Message);

        //         ViewBag.Error = "Something went wrong. Please try again.";
        //         return View();
        //     }
        // }


        [Authorize]
        [SessionAuthorize]

        //[Authorize(Roles = "Admin")] 
        public IActionResult Dashboard()
        {
            //int test = Convert.ToInt32(User.GetUserId());
            return View();
        }

        [Authorize]
        [SessionAuthorize]
        public IActionResult Register()
        {
            ViewBag.UserType = _bal.UserType_Drowpdown_bal();

            return View();
        }

        [Authorize]
        [SessionAuthorize]
        [HttpPost]
        public IActionResult Register(User account)
        {
            if (!ModelState.IsValid)
                return View(account);

            if (!IsValidPassword(account.password))
                return View(account);


            string secret = GenerateSecret();

            var hasher = new PasswordHasher<string>();
            string hashedPassword = hasher.HashPassword(null, account.password);

            var entity = new User
            {
                username = account.username,
                password = hashedPassword,
                secret = secret,
                Email = account.Email,
                PhoneNo = account.PhoneNo,
                UserType_Id = account.UserType_Id,
                CreatedBy = 1
            };

            // var entity = new User
            // {
            //     username = account.username,
            //     password = CryptoEngine.Encrypt(account.password),
            //     secret = secret,
            //     Email = account.Email,
            //     PhoneNo = account.PhoneNo,
            //     UserType_Id = account.UserType_Id,
            //     CreatedBy = 1
            // };


            int result = _bal.InsertUsers_bal(entity);

            if (result == 0)
            {
                ModelState.AddModelError("", "Username or Email already exists");
                return View(account);
            }

            TempData["Success"] = "User registered successfully!";
            return RedirectToAction("UserList");
        }


        public bool IsValidPassword(string password)
        {
            if (password.Length < 8)
                return false;

            if (!password.Any(char.IsUpper))
                return false;

            if (!password.Any(char.IsDigit))
                return false;

            if (!password.Any(ch => !char.IsLetterOrDigit(ch))) // special char
                return false;

            return true;
        }

        [Authorize]
        [SessionAuthorize]
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _bal.GetUserById_bal(id);

            if (user == null)
                return NotFound();

            ViewBag.UserType = _bal.UserType_Drowpdown_bal();

            return View(user);
        }

        [Authorize]
        [SessionAuthorize]
        [HttpPost]
        public IActionResult EditUser(UserEdit account)
        {
            ModelState.Remove("Password");

            if (!ModelState.IsValid)
            {
                ViewBag.UserType = _bal.UserType_Drowpdown_bal();
                return View(account);
            }

            var hasher = new PasswordHasher<string>();

            var entity = new UserEdit
            {
                user_id = account.user_id,
                username = account.username,
                password = string.IsNullOrWhiteSpace(account.password)
                    ? null
                    : hasher.HashPassword(null, account.password),
                Email = account.Email,
                PhoneNo = account.PhoneNo,
                UserType_Id = account.UserType_Id,
                UpdatedBy = 1
            };

            int result = _bal.UpdateUsers_bal(entity);

            if (result == 0)
            {
                ModelState.AddModelError("", "Username or Email already exists");
                ViewBag.UserType = _bal.UserType_Drowpdown_bal();
                return View(account);
            }
            else if (result == 1)
            {
                ModelState.AddModelError("", "User not found");
                ViewBag.UserType = _bal.UserType_Drowpdown_bal();
                return View(account);
            }

            TempData["Success"] = "User updated successfully!";
            return RedirectToAction("UserList");
        }

        [Authorize]
        [SessionAuthorize]
        public IActionResult UserList()
        {
            List<User> users = _bal.GetUser_bal();
            return View(users);
        }



        public IActionResult Setup2FA(string username)
        {
            string secret = _bal.GetUserSecret_bal(username);

            // If no secret then generate
            if (string.IsNullOrEmpty(secret))
            {
                secret = GenerateSecret();

                _bal.UpdateUserSecret_bal(username, secret);
            }

            string qr = GenerateQr(username, secret);

            ViewBag.QR = qr;
            ViewBag.Secret = secret;
            ViewBag.Email = username;

            return PartialView("_Setup2FAPartial");
        }


        public IActionResult Reset2FA(string username)
        {
            string newSecret = GenerateSecret();

            _bal.UpdateUserSecret_bal(username, newSecret);

            string qr = GenerateQr(username, newSecret);

            ViewBag.QR = qr;
            ViewBag.Secret = newSecret;
            ViewBag.username = username;

            return PartialView("_Setup2FAPartial");
        }



        public IActionResult Disable2FA(string username)
        {
            _bal.Disable2FA_bal(username);

            return RedirectToAction("UserList");
        }

        public IActionResult LoginLogs(string email)
        {
            var logs = _bal.GetLoginLogs_bal(email);

            return View(logs);
        }


        public async Task<IActionResult> SetSuperadmin()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var secret = HttpContext.Session.GetString("UserSecret");
            var username = HttpContext.Session.GetString("Username");
            var userTypeIdStr = HttpContext.Session.GetString("UserTypeId");

            if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Manage");
            }

            try
            {

                int userTypeId = string.IsNullOrEmpty(userTypeIdStr) ? 0 : Convert.ToInt32(userTypeIdStr);
                string roleName = ClaimsExtensions.MapRole(userTypeId);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username ?? ""),
                    new Claim(ClaimTypes.Role, roleName),
                    new Claim(ClaimTypes.NameIdentifier, userId)
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");

                await HttpContext.SignInAsync(
                    "MyCookieAuth",
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                    });

                // ✅ Clear session after success
                HttpContext.Session.Remove("UserSecret");
                HttpContext.Session.Remove("UserTypeId");

                return RedirectToAction("Dashboard", "Manage");
            }
            catch
            {
                ViewBag.Error = "Something went wrong during verification.";
                return View();
            }
        }


        public IActionResult Verify2FA()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Verify2FA(string otp)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var secret = HttpContext.Session.GetString("UserSecret");
            var username = HttpContext.Session.GetString("Username");
            var userTypeIdStr = HttpContext.Session.GetString("UserTypeId");

            if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Manage");
            }

            try
            {
                var key = Base32.Rfc4648.Decode(secret);
                var totp = new Totp(key);

                bool valid = totp.VerifyTotp(otp, out _, new VerificationWindow(2, 2));

                if (!valid)
                {
                    ViewBag.Error = "Invalid OTP";
                    return View();
                }

                int userTypeId = string.IsNullOrEmpty(userTypeIdStr) ? 0 : Convert.ToInt32(userTypeIdStr);
                string roleName = ClaimsExtensions.MapRole(userTypeId);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username ?? ""),
                    new Claim(ClaimTypes.Role, roleName),
                    new Claim(ClaimTypes.NameIdentifier, userId)
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");

                await HttpContext.SignInAsync(
                    "MyCookieAuth",
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                    });

                // ✅ Clear session after success
                HttpContext.Session.Remove("UserSecret");
                HttpContext.Session.Remove("UserTypeId");

                return RedirectToAction("Dashboard", "Manage");
            }
            catch
            {
                ViewBag.Error = "Something went wrong during verification.";
                return View();
            }
        }


        // [HttpPost]
        // public async Task<IActionResult> Verify2FA(string otp)
        // {
        //     // ✅ Get session values (set during login step)
        //     var userId = HttpContext.Session.GetString("UserId");
        //     var secret = HttpContext.Session.GetString("UserSecret");
        //     var username = HttpContext.Session.GetString("Username");
        //     var role = HttpContext.Session.GetString("UserRole");

        //     // ❌ Session expired / invalid
        //     if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(userId))
        //     {
        //         ViewBag.Error = "Session expired. Please login again.";
        //         return RedirectToAction("Login", "Manage");
        //     }

        //     try
        //     {
        //         var key = Base32.Rfc4648.Decode(secret);
        //         var totp = new Totp(key);

        //         // allow small time drift (recommended)
        //         bool valid = totp.VerifyTotp(otp, out long step, new VerificationWindow(2, 2));

        //         if (!valid)
        //         {
        //             ViewBag.Error = "Invalid OTP";
        //             return View();
        //         }

        //         string roleName = ClaimsExtensions.MapRole(Convert.ToInt32(role));
        //         // ✅ Create claims AFTER successful OTP
        //         var claims = new List<Claim>
        //         {
        //             new Claim(ClaimTypes.Name, username ?? ""),
        //             new Claim(ClaimTypes.Role, roleName),
        //             new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        //         };

        //         //@User.GetUserId()

        //         var identity = new ClaimsIdentity(claims, "MyCookieAuth");

        //         var authProperties = new AuthenticationProperties
        //         {
        //             IsPersistent = true,
        //             ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
        //         };

        //         // ✅ FINAL LOGIN (after OTP success)
        //         await HttpContext.SignInAsync(
        //             "MyCookieAuth",
        //             new ClaimsPrincipal(identity),
        //             authProperties
        //         );

        //         // 🔥 Clear temp session (important)
        //         HttpContext.Session.Remove("UserSecret");

        //         return RedirectToAction("Dashboard", "Manage");
        //     }
        //     catch (Exception)
        //     {
        //         ViewBag.Error = "Something went wrong during verification.";
        //         return View();
        //     }
        // }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // ✅ Clear session
            HttpContext.Session.Clear();

            // ✅ Remove authentication cookie
            await HttpContext.SignOutAsync("MyCookieAuth");

            // ✅ Redirect to login page
            return RedirectToAction("Login", "Manage");
        }





        string GenerateSecret()
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            return Base32.Rfc4648.Encode(key);
            //return Base32Encoder.Encode(key);
        }

        string GenerateQr(string username, string secret)
        {
            var url = $"otpauth://totp/MyApp:{username}?secret={secret}&issuer=godavaribiorefineries";

            using QRCodeGenerator qr = new QRCodeGenerator();
            var data = qr.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new Base64QRCode(data);

            return qrCode.GetGraphic(20);
        }



    }
}