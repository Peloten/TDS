using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    // 배경 속도
    public float scrollSpeed;
    // 배경 세로 사이즈
    public float imageWidth; 

    // 배경 자식 갯수
    private Transform[] parts;

    void Start()
    {
        // 자식들의 위치 가져옴
        parts = new Transform[transform.childCount];
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = transform.GetChild(i);
        }
    }

    void Update()
    {
        // 왼쪽으로 이동
        foreach (Transform part in parts)
        {
            part.position += Vector3.left * scrollSpeed * Time.deltaTime;
        }

        // x죄표가 왼쪽범위를 넘어가면 오른쪽으로 이동
        foreach (Transform part in parts)
        {
            if (part.position.x <= -imageWidth)
            {
                float maxX = GetMaxX();
                part.position = new Vector3(maxX + imageWidth, part.position.y, part.position.z);
            }
        }

        // 닿은 몬스터의 갯수를 가져와서 적용
        if(TruckCtrl.instance.monsterCount > 0 && TruckCtrl.instance.monsterCount < 2)
        {
            scrollSpeed = 1;
        }
        else if(TruckCtrl.instance.monsterCount > 1)
        {
            scrollSpeed = 0;
        }
        else
        {
            scrollSpeed = 2;
        }
    }

    float GetMaxX()
    {
        float maxX = parts[0].position.x;
        for (int i = 1; i < parts.Length; i++)
        {
            if (parts[i].position.x > maxX)
                maxX = parts[i].position.x;
        }
        return maxX;
    }
}
