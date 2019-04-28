using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteInLine : MonoBehaviour
{
    public int index;
    public float y;
    public bool isDestroy = false;
    void Update()
    {
        y = transform.position.y;
    }
}
