using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobilePaywall.Deamon.Core
{

  public interface ITask
  {
    Data.DeamonTask DeamonTask { get; }
    Data.DeamonTaskResult DeamonResult { get; }
    string TextData { get; }
    bool IsProcessSuccessful { get; }
    
  }

  public abstract class TaskBase
  {
    private Data.DeamonTask _deamonTask = null;
    private Data.DeamonTaskResult _deamonResult = null;
    private string _textData = string.Empty;
    private bool _isProcessSuccessful = true;
    private bool _isOnDemand = true;

    public Data.DeamonTask DeamonTask { get { return this._deamonTask; } }
    public Data.DeamonTaskResult DeamonResult { get { return this._deamonResult; } }
    public string TextData { get { return this._textData; } protected set { this._textData = value; } }
    public bool IsProcessSuccessful { get { return this._isProcessSuccessful; } protected set { this._isProcessSuccessful = value; } }


    public TaskBase(DeamonTaskData taskData)
    {
      this._isOnDemand = taskData.IsOnDemand;
      this._deamonTask = Data.DeamonTask.CreateManager().Load(taskData.ID);
      if (this._deamonTask == null)
        return;

      this._deamonTask.IsRunning = true;
      this._deamonTask.Update();

      this._deamonResult = new Data.DeamonTaskResult(-1, this._deamonTask, string.Empty, DateTime.Now, null, false, false, DateTime.Now, DateTime.Now);
      this._deamonResult.Insert();
      
      try
      {
        this.Execute();

        this._deamonResult.Data = this._textData;
        this._deamonResult.IsSuccessful = this._isProcessSuccessful;
      }
      catch(Exception e)
      {
        this.OnException();

        this._deamonResult.IsSuccessful = false;
        this._deamonResult.Data = "EXCEPTION: " + e.Message.ToString();
        this._deamonResult.HasFatal = true;

      }
      finally
      {

        this._deamonResult.Ended = DateTime.Now;
        this._deamonResult.Update();

        this._deamonTask.IsRunning = false;

        if(!this._isOnDemand)
          this._deamonTask.LastExecution = DateTime.Now;
        this._deamonTask.Update();

      }

    }

    protected abstract void Execute();

    protected virtual void OnException() { }
  }
}
