using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.ObjectPool
{
    public interface IPool
    {
        void Release();
    }
}