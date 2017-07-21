using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Game.Platform
{
    public class PlatformVersion
    {
        string mVersion;
        public int mMajorVerison;
        public int mMinorVerison;
        public int mReleaseVerison;

        public PlatformVersion(string version)
        {
            mVersion = version;

            string[] versionList = mVersion.Split('.');
            if (versionList.Length == 3)
            {
                mMajorVerison = int.Parse(versionList[0].Trim());
                mMinorVerison = int.Parse(versionList[1].Trim());
                mReleaseVerison = int.Parse(versionList[2].Trim());
            }
        }

        public int version
        {
            get
            {
                return mMajorVerison * 10000 + mMinorVerison * 100 + mReleaseVerison;
            }
        }

        public static int CompareVersion(string v1, string v2)
        {
            int version1 = new PlatformVersion(v1).version;
            int version2 = new PlatformVersion(v2).version;

            return version1.CompareTo(version2);
        }

        public static bool CheckUpgradeGame(string v1, string v2)
        {
            return new PlatformVersion(v1).mMajorVerison - new PlatformVersion(v2).mMajorVerison > 0;
        }
    }
}
