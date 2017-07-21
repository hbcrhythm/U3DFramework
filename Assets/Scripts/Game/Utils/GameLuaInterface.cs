using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;

namespace Game.Utils
{
    public class GameLuaInterface
    {
        public static void DOLocalMoveX(Transform target, float endValue, float duration, TweenCallback action)
        {
            OnComplete(ShortcutExtensions.DOLocalMoveX(target, endValue, duration), action);
        }

        public static void DOLocalMoveY(Transform target, float endValue, float duration, TweenCallback action)
        {
            OnComplete(ShortcutExtensions.DOLocalMoveY(target, endValue, duration), action);
        }

        public static void DOScale(Transform target, Vector3 endValue, float duration, TweenCallback action)
        {
            OnComplete(ShortcutExtensions.DOScale(target, endValue, duration), action);
        }

        public static void SetEase(Tweener t,Ease ease)
        {
            TweenSettingsExtensions.SetEase<Tweener>(t, ease);
        }

        public static void OnComplete(Tweener t, TweenCallback action)
        {
            TweenSettingsExtensions.OnComplete<Tweener>(t, action);
        }
    }
}
