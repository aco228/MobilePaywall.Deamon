using log4net;
using MobilePaywall.Deamon.Core;
using MobilePaywall.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobilePaywall.Deamon
{
  class Program
  {

    #region #logging#
    private static ILog _log = null;

    protected static ILog Log
    {
      get
      {
        if (Program._log == null)
          Program._log = LogManager.GetLogger(typeof(Program));
        return Program._log;
      }
    }
    #endregion
    
    static void Main(string[] args)
    {
      MobilePaywall.Data.Sql.Database db = null;
      Senti.Data.DataLayerRuntime runtime = new Senti.Data.DataLayerRuntime();
      //log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("~/log4net.config"));
      new SignalRConnector();

      Log.Debug("Task.Deamon started");
      DeamonManager.Start();

      Console.ReadKey();
    }
  }
}
