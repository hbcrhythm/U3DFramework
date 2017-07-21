using UnityEngine;

public class GameDebugConfig : MonoBehaviour
{
    public static bool loadLuaBytes = false;
    public static bool loadUIAssetBundle = false;
    public static bool openLog = true;
    public static bool debugTimer = false;

    public bool _loadLuaBytes;
    public bool _loadUIAssetBundle;
    public bool _openLog = true;
    public bool _openZbsDebugger;
    public bool _debugTimer;

    void Awake()
    {
        loadLuaBytes = _loadLuaBytes;
        loadUIAssetBundle = _loadUIAssetBundle;
        openLog = _openLog;
        LuaConst.openLuaDebugger = _openZbsDebugger;
        debugTimer = _debugTimer;
    }
}
