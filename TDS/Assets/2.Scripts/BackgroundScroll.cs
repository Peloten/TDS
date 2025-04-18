using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public float scrollSpeed;
    public float imageWidth; 

    private Transform[] parts;

    void Start()
    {
        parts = new Transform[transform.childCount];
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = transform.GetChild(i);
        }
    }

    void Update()
    {
        foreach (Transform part in parts)
        {
            part.position += Vector3.left * scrollSpeed * Time.deltaTime;
        }

        foreach (Transform part in parts)
        {
            if (part.position.x <= -imageWidth)
            {
                float maxX = GetMaxX();
                part.position = new Vector3(maxX + imageWidth, part.position.y, part.position.z);
            }
        }

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
