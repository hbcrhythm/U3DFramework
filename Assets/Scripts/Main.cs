using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.Manager;
using Game.Manager;
using Game.Utils.Timers;
using Net;
using Assets.Scripts.Game.Utils;
public class Main : MonoBehaviour
{
    private GameTimerManager mGameTimerManager = GameTimerManager.Instance;
    void Start()
    {
        GameDebugConfig.openLog = GameDebugConfig.openLog && Application.isEditor;
        GameDebugConfig.debugTimer = GameDebugConfig.debugTimer && Application.isEditor;

        DontDestroyOnLoad(this.gameObject);
        
        if (Application.isEditor)
        {
            var dh = this.gameObject.GetComponent<DebugHelper>();
            if (dh == null)
            {
                this.gameObject.AddComponent<DebugHelper>();
            }
        }
  
        var fileUtil = new GameLuaFileUtils();
        fileUtil.Init(delegate()
        {
            GameManager.Instance.Init(gameObject);
        });
    }

    private ResourceManager mResourceManager = ResourceManager.Instance;
    void Update()
    {
#if UNITY_EDITOR
        mResourceManager.Update();
#endif

        NetInterface.Update();
        mGameTimerManager.Execute();
    }

    void OnDisable()
    {
        print("main OnDisable");

        NetInterface.CloseConnect();
    }
}
