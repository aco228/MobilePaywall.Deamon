using MobilePaywall.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobilePaywall.Deamon.Core
{
  public class DeamonTaskData
  {
    private int _deamonTaskID = -1;
    private int _deamonTaskTypeID = -1;
    private int _day = -1;
    private int _hour = -1;
    private int _minute = -1;
    private string _namespace = string.Empty;
    private DateTime _lastExecutionDate;
    private bool _isOnDemand = false;

    public int ID { get { return this._deamonTaskID; } set { this._deamonTaskID = value; } }
    public int TypeID { get { return this._deamonTaskTypeID; } }
    public int Day { get { return this._day; } }
    public int Hour { get { return this._hour; } set { this._hour = value; } }
    public int Minute { get { return this._minute; } set { this._minute = value; } }
    public string Namespace { get { return this._namespace; } }
    public DateTime LastExecutionDate { get { return this._lastExecutionDate; } set { this._lastExecutionDate = value; } }
    public bool IsOnDemand { get { return this._isOnDemand; } set { this._isOnDemand = true; } }

    // SUMMARY: Bool should this task be executed based on last exectution date
    public bool ShouldThisTaskBeExecuted
    {
      get
      {
        DateTime timeOfNewExecution = this._lastExecutionDate;
        timeOfNewExecution = timeOfNewExecution.AddDays(this._day).AddHours(this._hour).AddMinutes(this._minute);
        return DateTime.Now > timeOfNewExecution;
      }
    }
    
    
    // SUMMARY: Constructor with all setups
    public DeamonTaskData(DirectContainerRow row)
    {
      int? deamonTaskID = row.GetInt("DeamonTaskID");
      if (deamonTaskID.HasValue)
        this._deamonTaskID = deamonTaskID.Value;

      int? hour = row.GetInt("Hour");
      if (hour.HasValue)
        this._hour = hour.Value;

      int? day = row.GetInt("Day");
      if (day.HasValue)
        this._day = day.Value;

      int? minute = row.GetInt("Minute");
      if (minute.HasValue)
        this._minute = minute.Value;

      int? deamonTaskTypeID = row.GetInt("DeamonTaskTypeID");
      if (deamonTaskTypeID.HasValue)
        this._deamonTaskTypeID = deamonTaskTypeID.Value;

      this._namespace = row.GetString("Namespace");

      DateTime? lastExecution = row.GetDate("LastExecution");
      if (lastExecution.HasValue)
        this._lastExecutionDate = lastExecution.Value;
    }

    // SUMMARY: Get all task data
    public static IEnumerable<DeamonTaskData> GetCurrentData()
    {
      DirectContainer container = DeamonManager.Database.LoadContainer(string.Format(@"
        SELECT DeamonTaskID, Namespace, Day, Hour, Minute, LastExecution 
        FROM MobilePaywall.core.DeamonTask 
        WHERE IsActive=1 AND IsRunning=0 AND ExecuteOnlyOnDemand=0;"));

      foreach (DirectContainerRow row in container.Rows)
        yield return new DeamonTaskData(row);
    }

    public static DeamonTaskData GetTaskByKey(string key)
    {
      DirectContainer container = DeamonManager.Database.LoadContainer(string.Format(@"
        SELECT DeamonTaskID, Namespace, Day, Hour, Minute, LastExecution 
        FROM MobilePaywall.core.DeamonTask 
        WHERE IsActive=1 AND IsRunning=0 AND DemandKey='{0}';", key));

      if (!container.HasValue || container.RowsCount == 0)
        return null;

      return new DeamonTaskData(container[0]);
    }

  }
}
