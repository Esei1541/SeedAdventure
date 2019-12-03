using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class MenuManager : UIManager
{
    [Serializable]
    public struct stage
    {
        public int stageNumber;             // 스테이지 No.
        public string stageTitle;           // 스테이지명
    }

    public GameObject bgPanel;
    public GameObject frame;
    public GameObject infoSet;
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI recordTimeText;

    public stage[] stageInfo;

    private void Start()
    {
    }

    public void StageButton(int level)
    {
        bgPanel.SetActive(true);
        frame.SetActive(true);
        infoSet.SetActive(true);

        stageText.SetText("Stage" + stageInfo[level - 1].stageNumber);
        titleText.SetText("[" + stageInfo[level - 1].stageTitle + "]");

        if (PlayerPrefs.GetFloat("record" + (level - 1)) != 0)
        {
            object[] rTime = TimeConversion(PlayerPrefs.GetFloat("record" + (level - 1)));
            string recordTimeStr = string.Format("{0:00}:{1:00}:{2:00}", rTime[0], rTime[1], rTime[2]);

            recordTimeText.SetText(recordTimeStr);
        } else
        {
            recordTimeText.SetText("-");
        }

        DataManager.instance.selectLevel = level - 1;
        Debug.Log("Level Num: " + level);
        Debug.Log("Record Time: " + PlayerPrefs.GetFloat("record" + (level - 1)));
    }
}
