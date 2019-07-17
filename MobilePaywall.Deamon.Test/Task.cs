using MobilePaywall.Deamon.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobilePaywall.Deamon.Test
{
  public class Task : TaskBase
  {

    public Task(DeamonTaskData data)
      :base(data)
    { }

    protected override void Execute()
    {
      this.TextData = "Uspjesno";
      Console.WriteLine("radi");
    }
    
  }
}
