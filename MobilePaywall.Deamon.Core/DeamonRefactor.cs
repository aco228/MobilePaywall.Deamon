using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobilePaywall.Deamon.Core
{
  public class DeamonRefactor
  {

    private string _namespace = string.Empty;
    private Type _type = null;

    public string Namespace { get { return this._namespace; } }
    public bool Exists { get { return this._type != null; } }

    public DeamonRefactor(string namespaceName)
    {
      this._namespace = namespaceName;
      this._type = Type.GetType(this._namespace, false);
    }

    public ITask Instantiate(params object[] args)
    {
      if(!this.Exists)
        return null;

      return Activator.CreateInstance(this._type, args) as ITask;
    }

  }
}
