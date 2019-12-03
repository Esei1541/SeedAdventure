using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance = null;
    [HideInInspector] public int selectLevel;

    private void Awake()
    {
        /* ---------- 싱글톤 구현 ---------- */
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        /* --------------------------------- */
    }
}
