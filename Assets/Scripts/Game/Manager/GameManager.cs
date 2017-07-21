using Assets.Scripts.Game.Utils;
using Game.Core;
using Game.Manager;
using UnityEngine;

namespace Game.Manager
{
    class GameManager : Singleton<GameManager>
    {
        public ResourceManager mResourceManager { get; private set; }

        public void Init(GameObject gameObject)
        {
            mResourceManager = new ResourceManager();
            LuaManager luaManager = gameObject.AddComponent<LuaManager>();

            if (OutputLog.sRegister == false)
            {
                gameObject.AddComponent<OutputLog>();
            }

        }

    }
}
