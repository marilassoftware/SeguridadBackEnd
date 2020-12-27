using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SeguridadBackEnd.Data;
using SeguridadBackEnd.Data.Entities;
using SeguridadBackEnd.Helpers;
using SeguridadBackEnd.Models;

namespace SeguridadBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;

        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;

        public AccountController(
            IUserHelper userHelper,
            UserManager<UserEntity> userManager,
            SignInManager<UserEntity> signInManager,
            IConfiguration configuration,
            IMailHelper mailHelper,
            DataContext context)
        {
            _userHelper = userHelper;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mailHelper = mailHelper;
            _context = context;
        }

        [HttpPost]
        [Route("Create")]
        [EnableCors()]
        public async Task<IActionResult> CreateUser([FromBody] UserEntity model)
        {
            if (ModelState.IsValid)
            {
                model.Email = model.UserName;
                var result = await _userManager.CreateAsync(model);
                //var result = await _userManager.CreateAsync(model, model.Email);
                if (result.Succeeded)
                {
                    var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(model);
                    var tokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        tipo = 1,
                        userid = model.Id,
                        token = myToken,
                        userName = string.Empty,
                        password = string.Empty

                    }, protocol: HttpContext.Request.Scheme);

                    string[] vecEmail = model.UserName.Split('@');

                    _mailHelper.SendMail(model.Email, "Registrarse llantas.marilas.net",
                        $"<table style = 'max-width: 600px; padding: 10px; margin:0 auto; border-collapse: collapse;'>" +
                        //$"  <tr>" +
                        //$"    <td style = 'background-color: #34495e; text-align: center; padding: 0'>" +
                        //$"       <a href = 'https://www.facebook.com/NuskeCIV/' >" +
                        //$"         <img width = '20%' style = 'display:block; margin: 1.5% 3%' src= 'https://veterinarianuske.com/wp-content/uploads/2016/10/line_separator.png'>" +
                        //$"       </a>" +
                        //$"  </td>" +
                        //$"  </tr>" +
                        //$"  <tr>" +
                        //$"  <td style = 'padding: 0'>" +
                        //$"     <img style = 'padding: 0; display: block' src = 'https://veterinarianuske.com/wp-content/uploads/2018/07/logo-nnske-blanck.jpg' width = '100%'>" +
                        //$"  </td>" +
                        //$"</tr>" +
                        $"<tr>" +
                        $" <td style = 'background-color: #ecf0f1'>" +
                        $"      <div style = 'color: #34495e; margin: 4% 10% 2%; text-align: justify;font-family: sans-serif'>" +
                        //$"            <h2 style = 'color: #e67e22; margin: 0 0 7px' > Hola " + vecEmail[0] + " </h2>" +
                        $"            <h2 style = 'color: #e67e22; margin: 0 0 7px' > Hola ADMINISTRADOR </h2>" +
                        $"                    <p style = 'margin: 2px; font-size: 15px'>" +
                        $"                      factu.marilas.net es una aplicación que hemos diseñado en MARILAS-SOFTWARE para que lleves el registro de los movimientos del inventario que haces " +
                        $"                      en tus labores diarias, estamos seguros de que te ayudará a ser mas organizado en tu trabajo " +
                        $"                      </p>" +
                        $"      <ul style = 'font-size: 15px;  margin: 10px 0'>" +
                        $"        <li> Registrate como usuario.</li>" +
                        $"        <li> Valida el inventario.</li>" +
                        $"      </ul>" +
                        $"      </div>" +
                        //$"  <div style = 'width: 100%;margin:20px 0; display: inline-block;text-align: center'>" +
                        //$"    <img style = 'padding: 0; width: 200px; margin: 5px' src = 'https://veterinarianuske.com/wp-content/uploads/2018/07/tarjetas.png'>" +
                        //$"  </div>" +
                        $"  <div style = 'color: #34495e; margin: 4% 10% 2%; text-align: justify;font-family: sans-serif'>" +
                        $"    <h2 style = 'color: #e67e22; margin: 0 0 7px' >SOLICITUD DE ACCESO </h2>" +
                        $"    <p style = 'margin: 2px; font-size: 15px'>" +
                        $"    <b>" + vecEmail[0] + " </b> ha solicitado poder ingresar a la aplicacion, por favor haz click en el siguiente link para autorizar su acceso:<br><br> " +
                        $"    </p>" +
                        $"    <a style ='text-decoration: none; border-radius: 5px; padding: 11px 23px; color: white; background-color: #3498db' href = \"{tokenLink}\">Autorizar acceso</a>" +
                        $"    <p style = 'color: #b3b3b3; font-size: 12px; text-align: center;margin: 30px 0 0' > Un producto marilas.net 2020 </p>" +
                        $"  </div>" +
                        $" </td >" +
                        $"</tr>" +
                        $"</table>");
                    //return BuildToken(model, Respuesta[0].CampoI);
                    return Ok(new { result = "Ok", token = "Mail enviado", expiration = DateTime.Now.ToString(), email = string.Empty });
                }
                else
                {
                    return Ok(new { result = "Error", token = "Messages.ErrorCrearUsuario", expiration = DateTime.Now.ToString(), email = string.Empty });
                }
            }
            else
            {
                return Ok(new { result = "Error", token = "Messages.ErrorCreandoUsuario", expiration = DateTime.Now.ToString(), email = string.Empty });
            }
        }

        [HttpPost]
        [Route("Login")]
        [EnableCors()]
        public async Task<IActionResult> Login([FromBody] Login objLogin)
        {
            if (ModelState.IsValid)
            {
                var result = _context.Users.Where(t => t.PasswordHash == objLogin.PasswordHash && t.Email == objLogin.Email && t.EmailConfirmed == true);
                var user = await _userHelper.GetUserByEmailAsync(objLogin.Email);
                user.PasswordHash = objLogin.PasswordHash;
                //var result1 = await _signInManager.PasswordSignInAsync(user, user.PasswordHash, false, lockoutOnFailure: false);

                if (result.ToList().Count > 0)
                {
                    UserEntity objUserEntity = new UserEntity();
                    objUserEntity.Email = objLogin.Email;
                    objUserEntity.PasswordHash = objLogin.PasswordHash;
                    objUserEntity.Id = result.ToList()[0].Id;
                    return BuildToken(objUserEntity);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Ok(new { result = "Error", token = "Error", expiration = DateTime.Now.ToString(), userId = "0" });
                }
            }
            else
            {
                return Ok(new { result = "Error", token = "Error ::", expiration = DateTime.Now.ToString(), userId = "0" });
            }
        }

        private IActionResult BuildToken(UserEntity userInfo)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Email),
                new Claim("Frecuencia", userInfo.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddDays(1);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: _configuration["Tokens:Issuer"],
               audience: _configuration["Tokens:Audience"],
               claims: claims,
               expires: expiration,
               signingCredentials: creds);

            return Ok(new { result = "Ok", token = new JwtSecurityTokenHandler().WriteToken(token), expiration = expiration, email = userInfo.Email, userid = userInfo.Id });
        }

        public async Task<IActionResult> ConfirmEmail(string tipo, string userId, string token, string userName, string password)
        {
            switch (tipo)
            {
                case "1":
                    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                    {
                        return NotFound();
                    }

                    var user = await _userHelper.GetUserByIdAsync(userId);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    var result = await _userHelper.ConfirmEmailAsync(user, token);
                    if (!result.Succeeded)
                    {
                        return NotFound();
                    }

                    return Ok("YA PEDES INGRESAR AL SISTEMA");
                    break;
                case "2":
                    return await this.ResetPassword("2", string.Empty, token, userName, password);
                    break;
            }
            return Ok();
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("ChangePassword")]
        [EnableCors()]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.User);
                if (user != null)
                {
                    var validar = _context.Users.Where(p => p.PasswordHash == model.OldPassword && user.UserName == model.User);
                    if (validar.ToList().Count > 0)
                    {
                        user.PasswordHash = model.NewPassword;
                        _context.Entry(user).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        return Ok(new { IsSuccess = true, Message = "Password cambiado", Result = "" });
                    }
                    else
                    {
                        return Ok(new { IsSuccess = false, Message = "No se pudo realizar el cambio.", Result = string.Empty });
                    }
                }
                else
                {
                    return Ok(new { IsSuccess = false, Message = "Usuario no encontrado", Result = "" });
                }
            }

            return Ok(new { IsSuccess = false, Message = "El modelo no es valido", Result = "" });
        }

        [HttpPost]
        [Route("RecoverPassword")]
        [EnableCors()]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    return Ok(new { IsSuccess = false, Message = "Usuario no encontrado", Result = "" });
                }
                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                var tokenLink = Url.Action(
                    "ResetPassword",
                    "Account",
                    new
                    {
                        tipo = 2,
                        userid = 0,
                        token = myToken,
                        userName = model.Email,
                        password = model.newPassword
                    }, protocol: HttpContext.Request.Scheme);

                string[] vecEmail = model.Email.Split('@');

                _mailHelper.SendMail(model.Email,
                    "Recuperar contraseña marilas.net",
                    "<table style = 'max-width: 600px; padding: 10px; margin:0 auto; border-collapse: collapse;'>" +
                            $"<tr>" +
                            $" <td style = 'background-color: #ecf0f1'>" +
                            $"      <div style = 'color: #34495e; margin: 4% 10% 2%; text-align: justify;font-family: sans-serif'>" +
                            $"            <h1 style = 'color: #e67e22; margin: 0 0 7px' > Hola " + vecEmail[0] + " </h1>" +
                            $"                    <p style = 'margin: 2px; font-size: 15px'>" +
                            $"                      Han pasado por los servidores de llantas.marilas.net y nos han dicho que quieres cambiar tu contraseña. <br> <br>" +
                            $"                      Si esto es cierto, estamos muy felices de poderte ayudar, puedes hacer click en el siguiente link para" +
                            $"                      finalizar la operación  <br> <br>" +
                            $"                      <a style ='text-decoration: none; border-radius: 5px; padding: 11px 23px; color: white; background-color: #3498db' href = \"{tokenLink}\">Cambiar contraseña</a>" +
                            $"                      <br> <br> <br> Si no hiciste esta petición no hay problema, borra este mensaje y asunto arreglado. <br> <br> <br>" +
                            $"                      Recibe un cordial saludo <br>" +
                            $"                      El equipo marilas.net <br>" +
                            $"                      Te deseamos una feliz y prospera vida. <br>" +
                            $"                      Hasta pronto.</p> " +
                            $" </td >" +
                            $"</tr>" +
                            $"</table>");
                return Ok(new { IsSuccess = true, Message = "Listo :: " + tokenLink, Result = "" });

            }

            return Ok(new { IsSuccess = false, Message = "El modelo no es valido", Result = "" });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string tipo, string userId, string token, string userName, string password)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user != null)
            {
                user.PasswordHash = password;
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                //var result = await _userHelper.ResetPasswordAsync(user, token, password);
                //if (result.Succeeded)
                //{
                //    return Ok("Contraseña actualizada");
                //}

                //return Ok("Error restaurando la contraseña");

                return Ok("Contraseña actualizada");

            }
            return Ok("Usuario no encontrado.");
        }
    }
}
