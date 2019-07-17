using MobilePaywall.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobilePaywall.Deamon.Core
{
  public class DeamonManager
  {

    public static MobilePaywallDirect Database = MobilePaywallDirect.Instance;

    // SUMMARY: Find all suitable tasks and run them
    public static void Start()
    {
      MobilePaywallDirect database = MobilePaywallDirect.Instance;
      for (;;)
      {
        System.Threading.Thread.Sleep(1500);
        
        foreach(DeamonTaskData task in DeamonTaskData.GetCurrentData())
        {
          if (!task.ShouldThisTaskBeExecuted)
            continue;

          StartTask(task);
        }
      }
    }

    // SUMMARY: Run task from demand (from signalR)
    public static void OnDemand(string key)
    {
      new System.Threading.Thread(() =>
      {
        DeamonTaskData task = DeamonTaskData.GetTaskByKey(key);
        if (task == null)
        {
          SignalRConnector.SendMessage(string.Format("OnDemand:: Could not find task with key '{0}'", key));
          return;
        }

        task.IsOnDemand = true;
        StartTask(task);

      }).Start();
    }
    
    // SUMMARY: Try to run task
    public static void StartTask(DeamonTaskData task)
    {
      string taskNamespace = task.Namespace;
      string className = string.Format("{0}.Task, {0}", taskNamespace);
      DeamonRefactor refactor = new DeamonRefactor(className);
      if (!refactor.Exists)
      {
        Console.WriteLine(DateTime.Now.ToString() + "Task with namespace DOES NOT EXISTS: " + taskNamespace);
        return;
      }

      new System.Threading.Thread(() =>
      {
        SignalRConnector.SendMessage(DateTime.Now.ToString() + " Task started: " + taskNamespace);
        refactor.Instantiate(task);
      }).Start();
    }


  }
  

}
