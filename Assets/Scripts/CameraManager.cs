using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private GameManager gm;

    Vector3 touchStart;
    public float panningMinX = 0f;
    public float panningMaxX = 5f;
    public float panningMinY = 0f;
    public float panningMaxY = 5f;
    public float zoomMin = 3f;
    public float zoomMax = 9f;

    [Range(0.001f, 0.03f)]
    public float zoomSpped = 0.015f;

    private void Awake()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.overPanel.activeInHierarchy && !gm.underPanel.activeInHierarchy)
        {
            PanningAndZoom();
            Zoom(Input.GetAxis("Mouse ScrollWheel") * 3f);                                                   // (PC에서) 마우스 휠 입력이 들어왔을 경우 Zoom 기능을 수행
        }
            
    }

    void PanningAndZoom()
    {
        // 드래그를 통한 카메라 시점 및 배율의 조정 및 범위를 제한하는 메서드
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);                       // MouseButtonDown이 발생했을 시점의 좌표를 저장한다
        }

        if (Input.touchCount == 2)                 
        {
            // 2점 이상의 터치가 인식될 경우 Zoom 기능을 수행
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;                // 두 터치 지점의 첫 좌표값을 저장
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;                   // 두 터치 지점의 PrevPos거리와 현재 거리
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;                                    // 현재 거리에서 첫 거리의 길이값을 비교하여 Zoom의 값을 정한다.

            Zoom(difference * zoomSpped);
        } else if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);   // 드래그를 통한 좌표 이동이 발생했을 때 초기 좌표와의 거리를 저장
            Vector3 modifyDirection = Camera.main.transform.position + direction;                   // Camera의 위치가 될 좌표값

            /* 카메라의 위치가 제한 좌표 밖으로 벗어날 경우 해당 축의 좌표값을 최대 또는 최소값으로 수정 */
            if (modifyDirection.x < panningMinX)
            {
                modifyDirection.x = panningMinX;
            }
            else if (modifyDirection.x > panningMaxX)
            {
                modifyDirection.x = panningMaxX;
            }

            if (modifyDirection.y < panningMinY)
            {
                modifyDirection.y = panningMinY;
            }
            else if (modifyDirection.y > panningMaxY)
            {
                modifyDirection.y = panningMaxY;
            }
            /* ----------------------------------------------------------------------------------------- */

            Camera.main.transform.position = modifyDirection;                                       // 최종적으로 도출된 좌표값을 메인 카메라에 적용
        }
    }


    void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomMin, zoomMax);
    }
}
