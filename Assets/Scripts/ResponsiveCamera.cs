using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponsiveCamera : MonoBehaviour
{
    [SerializeField]private float defaultWidth = 10f;
    [SerializeField] private bool maintainWidth;

    void Update()
    {
        Camera.main.orthographicSize = defaultWidth / Camera.main.aspect;
    }
}
