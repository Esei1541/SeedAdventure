using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class PanelManager : MonoBehaviour
{
    private Animator animator;
    private Transform playerTransform;
    private Animator playerAnimator;
    private GameManager gm;
    private AudioManager am;
    private int posX, posY;      // 패널의 x, y 좌표를 정의하는 변수

    public int state;       // 패널의 초기 상태를 정의 (0: 흙 / 1: 잔디 / 2: 꽃 / 3: empty)

    private void Awake()
    {
        posX = int.Parse(this.name.Substring(6, 1), System.Globalization.NumberStyles.HexNumber);
        posY = int.Parse(this.name.Substring(7, 1), System.Globalization.NumberStyles.HexNumber);

        am = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerTransform = GameObject.Find("PlayerTransform").GetComponent<Transform>();
        playerAnimator = GameObject.Find("Player").GetComponent<Animator>();
        animator = this.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        animator.SetInteger("state", state);
    }

    // Update is called once per frame
    void Update()
    {
        PanelFocus();
    }

    private void OnMouseDown()
    {
        if (!gm.overPanel.activeInHierarchy && !gm.underPanel.activeInHierarchy)
        {
            Debug.Log(posX.ToString() + posY.ToString() + "Touched");

            if (posX == 0 && posY == 0)
            {
                // 현재 위치와 패널 위치값을 비교하여 이동할 수 있는 좌표인 경우 이동
                if ((GameManager.nowPosX == 99 && GameManager.nowPosY == 99) || (GameManager.nowPosX == 1 && GameManager.nowPosY == 0) || (GameManager.nowPosX == 0 && GameManager.nowPosY == 1))
                {
                    StateIncrease();
                }
                else
                {
                    am.PlaySE(3);
                    Debug.Log("position Error");
                }
            }
            else
            {
                if (posY == GameManager.maxSize[1])
                {
                    // 패널이 골 패널인 경우 
                    // 골 패널은 가장 오른쪽 아래 패널보다 y 좌표값이 1 높은 경우 골 패널로 취급(GameManager.cs의 maxsize[1] 값과 동일)
                    if (gm.ClearCheck())
                    {
                        if ((GameManager.nowPosX == posX && (GameManager.nowPosY == posY - 1 || GameManager.nowPosY == posY + 1)) || (GameManager.nowPosY == posY && (GameManager.nowPosX == posX - 1 || GameManager.nowPosX == posX + 1)))
                        {
                            StateIncrease();
                            gm.Clear();
                        }
                    } else
                    {
                        am.PlaySE(3);
                        Debug.Log("Clear Check Failed");
                    }
                } else
                {
                    if ((GameManager.nowPosX == posX && (GameManager.nowPosY == posY - 1 || GameManager.nowPosY == posY + 1)) || (GameManager.nowPosY == posY && (GameManager.nowPosX == posX - 1 || GameManager.nowPosX == posX + 1)))
                    {
                        StateIncrease();
                    }
                    else
                    {
                        am.PlaySE(3);
                        Debug.Log("position Error");
                    }
                }
            }
        }

    }

    private void PanelFocus()
    {
        // 이동할 수 있는 Panel을 체크하여 밝게 표시해주는 메서드
        if (GameManager.nowPosX == 99 && GameManager.nowPosY == 99)
        {
            if (!(posX == 0 && posY == 0))
            {
                this.gameObject.GetComponent<SpriteRenderer>().color = new Color32(150, 150, 150, 255);
            } else
            {
                this.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            }
        } else
        {
            if (posY == GameManager.maxSize[1])
            {
                // Goal 패널일 경우
                if ((GameManager.nowPosX == GameManager.maxSize[0] - 1 && GameManager.nowPosY == GameManager.maxSize[1] - 1) && gm.ClearCheck())
                {
                    this.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
                } else
                {
                    this.gameObject.GetComponent<SpriteRenderer>().color = new Color32(150, 150, 150, 255);
                }
            } else
            {
                if (((posX == GameManager.nowPosX && (posY == GameManager.nowPosY - 1 || posY == GameManager.nowPosY + 1))
                || (posY == GameManager.nowPosY && (posX == GameManager.nowPosX - 1 || posX == GameManager.nowPosX + 1))) && state < 2)
                {
                    // 현재 좌표의 상하좌우 패널이면서 state가 2나 3이 아닐 경우
                    this.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
                } else if (posX == GameManager.nowPosX && posY == GameManager.nowPosY)
                {
                    // 현재 좌표와 일치하는 패널일 경우
                    this.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
                } else
                {
                    this.gameObject.GetComponent<SpriteRenderer>().color = new Color32(150, 150, 150, 255);
                }
            }
        }
    }

    public void SetState(int state)
    {
        this.state = state;
        animator.SetInteger("state", this.state);
    }

    private void StateIncrease()
    {
        if (state < 2)      // 패널이 꽃 상태가 아닐 경우 패널의 상태를 한 단계 증가
        {
            animator.SetInteger("state", ++state);
            GameManager.nowPosX = posX;
            GameManager.nowPosY = posY;

            playerAnimator.SetTrigger("trans");
            playerTransform.position = new Vector2(this.gameObject.transform.position.x - 0.02f, this.gameObject.transform.position.y + 0.4f);
            am.PlaySE(0);

            /*
            if ((GameManager.nowPosX == (GameManager.maxSize[0] - 1) && GameManager.nowPosY == (GameManager.maxSize[1]) - 1) && this.state == 2)
            {
                gm.ClearCheck();
            }
            */
        }
        else
        {
            am.PlaySE(3);
            Debug.Log("state value is max");
        }
    }

    
}
