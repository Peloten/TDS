using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPush : MonoBehaviour
{
    public Vector2 overlapBoxSize = new Vector2(1f, .15f);
    public Color gizmoColor = new Color(1f, 0.5f, 0f, 0.4f); // 주황색 반투명

    void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube((Vector2)transform.position + Vector2.up, overlapBoxSize);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
