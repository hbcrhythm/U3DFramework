using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Game.Utils
{
    public class IniFile
    {
        private string mDefaultSection;
        private Dictionary<string, Dictionary<string, string>> mDicIni = new Dictionary<string, Dictionary<string, string>>();

        public IniFile(string content)
        {
            Init(content);
        }

        public void Init(string content)
        {
            if (string.IsNullOrEmpty(content))
                return;

            string[] list = content.Split('\n');

            for (int i = 0; i < list.Length; i++)
            {
                string str = list[i].Trim();
                if (string.IsNullOrEmpty(str) == false && str.Length > 3)
                {
                    //section
                    if (str[0] == '[')
                    {
                        int endSectionFlag = str.IndexOf(']');
                        if (endSectionFlag == -1)
                        {
                            Debug.LogError("section error:" + str);
                            return;
                        }

                        mDefaultSection = str.Substring(1, endSectionFlag - 1);
                        mDicIni[mDefaultSection] = new Dictionary<string, string>();
                    }
                    else
                    {
                        int keyFlagIndex = str.IndexOf('=');

                        // key value
                        if (keyFlagIndex != -1)
                        {
                            string key = str.Substring(0, keyFlagIndex);
                            string value = str.Substring(keyFlagIndex + 1);

                            mDicIni[mDefaultSection][key] = value;
                        }
                    }
                }
            }
        }

        public string GetValue(string section, string key)
        {
            Dictionary<string, string> dicSection;
            if (mDicIni.TryGetValue(section, out dicSection))
            {
                string value;
                if (dicSection.TryGetValue(key, out value))
                {
                    return value;
                }
            }
            return "";
        }
    }
}
