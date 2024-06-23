using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class EventHandler<T>
    {
        private  event Action<T> _event;

        public event Action<T> Event
        {
            add 
            {
                _event += value;
            }
            remove
            {
                _event -= value;
            }
        }
        public void Raise(T args)
        { 
            _event?.Invoke(args);
        }
    }
}
