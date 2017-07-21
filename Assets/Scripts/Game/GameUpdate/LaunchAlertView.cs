using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Game.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.GameUpdate
{
    class LaunchAlertView:LaunchViewBase
    {
        Text mContent;
        Action mCallBack;
        public LaunchAlertView(GameObject go)
        {
            mGameObject = go;
            mTransform = go.transform;

            mContent = GetComponent<Text>("Text/TextDesc1");
            var okBtn = GetComponent<Button>("Btn/BtnOk");
            okBtn.onClick.AddListener(delegate()
            {
                this.mGameObject.SetActive(false);
                mCallBack();
            });

            AddEventListener<string,Action>(EventConstant.SHOW_ALERT, OnShowAlert);
        }

        public void Dispose()
        {
            RemoveEventListener<string, Action>(EventConstant.SHOW_ALERT, OnShowAlert);
        }

        private void OnShowAlert(string tip,Action callBack)
        {
            EventDispatcher.Instance.Dispatch(EventConstant.LOAD_REMOTE_ERROR);

            this.mGameObject.SetActive(true);
            this.mContent.text = tip;
            mCallBack = callBack;
        }
    }
}
