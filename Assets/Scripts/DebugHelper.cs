using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper : MonoBehaviour {

	// Use this for initialization
    public static DebugHelper Instance;
    public static float sSkillMinHurt { get { return Instance ? Instance.MinHurt : 0; } }
    public static float sSkillMaxHurt { get { return Instance ? Instance.MaxHurt : 100000; } }
    public static bool sPlayerControlMonster { get { return Instance ? Instance.ControlMonster : false; } }
    public static bool sTestFollower { get { return Instance ? Instance.TestFollower : false; } }
    public static bool sDisableEffect { get { return Instance ? Instance.DisableEffect : false; } }
    public static bool sDisableRole { get { return Instance ? Instance.DisableRole : false; } }
    public static bool sTestShake { get { return Instance ? Instance.TestShake : false; } }
    public static bool sTestFashion { get { return Instance ? Instance.TestFashion : false; } }
    public static bool sOpenGuide { get { return Instance ? Instance.OpenGuide : false; } }
    public static int sAddAffectProbability { get { return Instance ? Instance.Probability : 0; } }
    public static List<int> sLeftFollowers { get { return Instance ? Instance.LeftFollowers : null; } }
    public static List<int> sRightFollowers { get { return Instance ? Instance.RightFollowers : null; } }

    [UnityEngine.SerializeField]
    private int targetFrame = 30;
    [UnityEngine.SerializeField][Range(0, 100)]
    private int Probability = 0;
    [UnityEngine.SerializeField]
    [Range(0, 10000)]
    private float MinHurt = 0;
    [UnityEngine.SerializeField]
    [Range(0, 10000)]
    private float MaxHurt = 100000;
    [UnityEngine.SerializeField]
    private bool ControlMonster = false;
    [UnityEngine.SerializeField]
    private bool DisableEffect = false;
    [UnityEngine.SerializeField]
    private bool DisableRole = false;
    [UnityEngine.SerializeField]
    private bool TestShake = false;
    [UnityEngine.SerializeField]
    private bool OpenGuide = false;
    [UnityEngine.SerializeField]
    private bool TestFashion = false;
    [UnityEngine.SerializeField]
    private bool TestFollower = false;
    [UnityEngine.SerializeField]
    private List<int> LeftFollowers = new List<int>();
    [UnityEngine.SerializeField]
    private List<int> RightFollowers = new List<int>();

    void Start()
    {
        Instance = this;
    }
	void Update()
	{
		if (Application.targetFrameRate != this.targetFrame) {
			Application.targetFrameRate = this.targetFrame;
		}

	}
}
