using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Threading.Tasks;

namespace b2bSwgroup.Controllers
{
    public class CallBackController : Controller
    {
        // GET: CallBack

        public ActionResult SendMessage()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> SendMessage(b2bSwgroup.Models.ModelsForView.CallBackMessage message)
        {
            if(ModelState.IsValid)
            {
                var from = "specification@biz2biz.online";
                var pass = "As12345678";

                // адрес и порт smtp-сервера, с которого мы и будем отправлять письмо
                SmtpClient client = new SmtpClient("smtp.yandex.ru", 25);

                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(from, pass);
                client.EnableSsl = true;

                //// создаем письмо: message.Destination - адрес получателя
                var mail = new MailMessage(from,"kadet635@gmail.com");
                mail.Subject = message.Subject;
                mail.Body = message.Body+" Ответить на "+message.Email;
                await client.SendMailAsync(mail);

                mail = new MailMessage(from, "fedorovi@gmail.com");
                mail.Subject = message.Subject;
                mail.Body = message.Body + " Ответить на " + message.Email;
                await client.SendMailAsync(mail);
                return RedirectToAction("ConfirmSendMessage");
            }
            return View(message);
        }
        public ActionResult ConfirmSendMessage()
        {
            return View();
        }
    }
}