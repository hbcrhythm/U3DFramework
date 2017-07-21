using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Net.Lua;

namespace Net
{
    public class NetSocketParam
    {
        public LuaNetSocketParam mLuaNetSocketParam;
        public int mID;
        public bool mCheckSendError = true;

        public virtual void ShowSendErrorTip()
        {
            //Alert.ShowMessage("保留现场！前端发不出包，需要后端检查连接是否断开，时间为" + DateTime.Now.ToString("mm:ss"));
            Debug.Log("连接断开，时间为：" + DateTime.Now.ToString("dd日HH时mm分ss秒"));
        }

        public virtual bool GetLoginStatus()
        {
            return mLuaNetSocketParam.loginStatus;
        }

        public virtual void LoginIn()
        {
            mLuaNetSocketParam.loginIn.Call();
        }

        public virtual bool ShowTip()
        {
            return true;
        }

        public virtual void MsgSendComplete(int cmdID)
        {

        }

        public virtual void MsgReceiveComplete(int cmdID)
        {

        }
    }
}
