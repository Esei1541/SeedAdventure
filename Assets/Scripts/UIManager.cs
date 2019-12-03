using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    public void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Debug.Log("LoadScene(" + sceneName + ")");
    }

    public void SetObjectActive(GameObject popupWindow)
    {
        // 버튼 터치 게임오브젝트를 매개변수로 받아 active 상태를 전환해준다.
        popupWindow.SetActive(!popupWindow.activeInHierarchy);
    }

    public void SetOffActive(GameObject popupWindow)
    {
        popupWindow.SetActive(false);
    }

    public void DebugMessage(string str)
    {
        Debug.Log(str);
    }

    public object[] TimeConversion(float time)
    {
        // float 타입의 시간을 받아서 분, 초, 밀리초로 변환한 뒤 배열로 반환
        object[] cTime = new object[3];

        cTime[0] = (int)time / 60;
        cTime[1] = (int)time % 60;
        cTime[2] = time * 100;
        cTime[2] = ((float)cTime[2] % 100);

        return cTime;
    }
}
