using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobilePaywall.Deamon.Core;
using System.Net.Mail;

namespace MobilePaywall.Deamon.GreenLightMedia
{
  public class Task : MobilePaywall.Deamon.Core.TaskBase
  {
    private string _email = string.Empty;

    public Task(DeamonTaskData taskData) : base(taskData)
    {
      this._email = this.DeamonTask.Storage.GetString("email");
    }

    protected override void Execute()
    {
      SendEmail();
    }

    private void SendEmail()
    {
      SmtpClient client = new SmtpClient();
      client.Port = 587;
      client.Host = "smtp.gmail.com";
      client.EnableSsl = true;
      client.Timeout = 10000;
      client.DeliveryMethod = SmtpDeliveryMethod.Network;
      client.UseDefaultCredentials = false;
      client.Credentials = new System.Net.NetworkCredential("aleksandar.k03@gmail.com", "#093hazzarD");

      MailMessage mm = new MailMessage("donotreply@domain.com", "sendtomyemail@domain.co.uk", "test", "test");
      mm.BodyEncoding = UTF8Encoding.UTF8;
      mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

      client.Send(mm);
    }

  }
}
