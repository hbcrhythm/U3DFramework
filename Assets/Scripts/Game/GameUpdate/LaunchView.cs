using Assets.Scripts.Game.Events;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Assets.Scripts.Game.GameUpdate
{
    class LaunchView : LaunchViewBase
    {
        Text mTip;
        Slider mSlider;
        Text mProgressTip;
        GameObject mSliderGameObject;
        LaunchAlertView mLaunchAlertView;

        public LaunchView(GameObject go)
        {
            mGameObject = go;
            mTransform = go.transform;
            mTip = GetComponent<Text>("Canvas/tip");

            mSliderGameObject = Find("Canvas/Slider").gameObject;
            mSlider = mSliderGameObject.GetComponent<Slider>();
            mProgressTip = GetComponent<Text>("Canvas/Slider/tip");

            mLaunchAlertView = new LaunchAlertView(Find("Canvas/alert_view").gameObject);

            AddEventListener<string>(EventConstant.UPDATE_TIP, OnUpdateTip);
            AddEventListener<float, float>(EventConstant.UPDATE_PROGRESS, OnUpdateProgress);
            AddEventListener(EventConstant.OPEN_LOGIN_VIEW, Dispose);
            AddEventListener(EventConstant.LOAD_REMOTE_ERROR, OnLoadRemoteError);
        }

        private void Dispose()
        {
            RemoveEventListener(EventConstant.OPEN_LOGIN_VIEW, Dispose);
            RemoveEventListener<string>(EventConstant.UPDATE_TIP, OnUpdateTip);
            RemoveEventListener<float, float>(EventConstant.UPDATE_PROGRESS, OnUpdateProgress);
            RemoveEventListener(EventConstant.LOAD_REMOTE_ERROR, OnLoadRemoteError);

            mLaunchAlertView.Dispose();
            GameObject.Destroy(mGameObject);
        }

        private void OnLoadRemoteError()
        {
            mSliderGameObject.SetActive(false);
            mProgressTip.text = "";
            mSlider.value = 0f;
        }

        private void OnUpdateTip(string str)
        {
            mTip.text = str;
        }

        private void OnUpdateProgress(float curLoaded, float totalSize)
        {
            if (totalSize <= 0)
            {
                return;
            }

            var v = curLoaded / totalSize;
            mSliderGameObject.SetActive(v < 1);
            mSlider.value = v;
            mProgressTip.text = string.Format("正在更新:{0}/{1}", Launch.GetSizeStr(curLoaded), Launch.GetSizeStr(totalSize));
        }

    }
}
