using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Service;

public partial class DeviceOrientationService
{
    public QueueBuffer SerialBuffer = new();

    public partial void OpenPort();
    public partial void ClosePort();
  
    public sealed partial class QueueBuffer : Queue
    {
        public event EventHandler? Changed;
        public override void Enqueue(object? obj)
        { 
            base.Enqueue(obj);
            Changed?.Invoke(this,EventArgs.Empty);
        }    
    }
}
