using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    private bool isPause;
    private bool isClear;
    private UIManager um;
    private Animator playerAnimator;
    private int[,] defaultState;
    private TextMeshProUGUI recordTimeText;
    private TextMeshProUGUI playTimeText;
    private Transform goalText;
    private CameraManager cm;
    private int level;
    private float timeCount;
    private AudioManager am;

    [Serializable]
    public struct stageData
    {
        public int maxX;
        public int maxY;
        public float maxPanningHorizon;
        public float maxPanningVertical;
        public GameObject panelSet;
    }

    public static int[] maxSize;
    public static int nowPosX;
    public static int nowPosY;
    
    public stageData[] stageSetting;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI resultTimeText;
    public TextMeshProUGUI resultRecordTimeText;
    public GameObject overPanel;
    public GameObject underPanel;
    public GameObject clearWindow;
    public int testLevelSetting;
    

    private void Awake()
    {
        am = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        cm = GameObject.Find("Main Camera").GetComponent<CameraManager>();
        goalText = GameObject.Find("GoalText_Canvas").GetComponent<Transform>();
        recordTimeText = GameObject.Find("Text_STime").GetComponent<TextMeshProUGUI>();
        maxSize = new int[2];
        playerAnimator = GameObject.Find("Player").GetComponent<Animator>();
        um = GameObject.Find("GameManager").GetComponent<UIManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(PlayerPrefs.GetFloat("record" + level));
        
        isClear = false;
        timeCount = 0f;
        timeText.SetText("00:00:00");

        nowPosX = 99;
        nowPosY = 99;

        SetLevel();
        SetDefaultState();
        SetGoalText();
    }

    private void Update()
    {
        if (!isClear)
        {
            Timer();
        }
    }

    private void SetLevel()
    {
        if (testLevelSetting != 0)
        {
            level = testLevelSetting - 1;
        }
        else
        {
            level = DataManager.instance.selectLevel;
        }

        maxSize[0] = stageSetting[level].maxX;
        maxSize[1] = stageSetting[level].maxY;
        cm.panningMaxX = stageSetting[level].maxPanningHorizon;
        cm.panningMinY = stageSetting[level].maxPanningVertical * -1.0f;

        if (PlayerPrefs.GetFloat("record" + level) != 0)
        {
            object[] rTime = um.TimeConversion(PlayerPrefs.GetFloat("record" + level));
            string recordTimeStr = string.Format("{0:00}:{1:00}:{2:00}", rTime[0], rTime[1], rTime[2]);

            recordTimeText.SetText(recordTimeStr);
        }
        else
        {
            recordTimeText.SetText("-");
        }
        
        for (int i = 0; i < stageSetting.Length; i++)
        {
            // 플레이하는 레벨이 아닌 레벨의 스테이지 오브젝틀를 날려버린다(최적화)
            if (i != level)
            {
                Destroy(stageSetting[i].panelSet);
            }
        }

        stageSetting[level].panelSet.SetActive(true);
    }
    private void SetGoalText()
    {
        // GoalText의 위치를 초기화

        Transform goalPanel = GameObject.Find("Panel_" + Convert.ToString((maxSize[0] - 1), 16) + Convert.ToString(maxSize[1], 16)).GetComponent<Transform>();

        goalText.position = new Vector2(goalPanel.position.x, goalPanel.position.y + 0.6f);
    }

    public void Timer()
    {
        if (!isClear)
        {
            timeCount += Time.deltaTime;

            object[] cTime = new object[3];
            cTime = um.TimeConversion(timeCount);

            string currentTime = string.Format("{0:00}:{1:00}:{2:00}", cTime[0], cTime[1], cTime[2]);

            timeText.SetText(currentTime);
        }
    }

    public void SetDefaultState()
    {
        defaultState = new int[maxSize[0], maxSize[1]];

        for (int x = 0; x < maxSize[0]; x++)                                                        
        {
            for (int y = 0; y < maxSize[1]; y++)
            {
                int state = GameObject.Find("Panel_" + Convert.ToString(x, 16) + Convert.ToString(y, 16)).GetComponent<PanelManager>().state;
                defaultState[x, y] = state;

                Debug.Log("Set default state : Panel_" + Convert.ToString(x, 16) + Convert.ToString(y, 16) + " is " + state);
            }
        }
    }

    public void Pause()
    {
        if (isPause)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
        isPause = !isPause;
    }

    public void ResetGame()
    {
        for (int x = 0; x < maxSize[0]; x++)
        {
            for (int y = 0; y < maxSize[1]; y++)
            {
                PanelManager panel = GameObject.Find("Panel_" + Convert.ToString(x, 16) + Convert.ToString(y, 16)).GetComponent<PanelManager>();
                panel.SetState(defaultState[x, y]);
            }
        }

        timeCount = 0;

        nowPosX = 99;
        nowPosY = 99;

        Transform playerTransform = GameObject.Find("PlayerTransform").GetComponent<Transform>();

        playerTransform.position = new Vector2(-1.86f, 2.2f);
    }

    public bool ClearCheck()
    {
        /* 골 패널에 들어갈 수 있는지를 체크하는 메서드 */
        for (int x = 0; x < maxSize[0]; x++)                                                        // Panel_xy 의 state 상태를 체크하여 모든 패널의 상태가 2나 3이 미만일 경우 return
        {
            for (int y = 0; y < maxSize[1]; y++)
            {
                if (GameObject.Find("Panel_" + Convert.ToString(x, 16) + Convert.ToString(y, 16)).GetComponent<PanelManager>().state < 2)
                {
                    Debug.Log("Panel_" + Convert.ToString(x, 16) + Convert.ToString(y, 16) + " is Not Clear.");
                    return false;
                }
                Debug.Log("Panel_" + Convert.ToString(x, 16) + Convert.ToString(y, 16) + " is Clear.");
            }
        }

        return true;
    }

    public void Clear()
    {
        isClear = true;
        RecordChange();

        object[] cTime = um.TimeConversion(timeCount);
        string clearTimeStr = string.Format("{0:00}:{1:00}:{2:00}", cTime[0], cTime[1], cTime[2]);
        object[] rTime = um.TimeConversion(PlayerPrefs.GetFloat("record" + level));
        string recordTimeStr = string.Format("{0:00}:{1:00}:{2:00}", rTime[0], rTime[1], rTime[2]);

        resultRecordTimeText.SetText(recordTimeStr);
        resultTimeText.SetText(clearTimeStr);
        playerAnimator.SetTrigger("jump");
        overPanel.SetActive(true);
        um.SetObjectActive(clearWindow);
        Debug.Log("Stage Clear");
        am.PlaySE(4);
    }

    public void RecordChange()
    { 
        if (PlayerPrefs.GetFloat("record" + level) != 0)
        {
            if (timeCount < PlayerPrefs.GetFloat("record" + level))
            {
                PlayerPrefs.SetFloat("record" + level, timeCount);
            }
        }
        else
        {
            PlayerPrefs.SetFloat("record" + level, timeCount);
        }
    }
}
