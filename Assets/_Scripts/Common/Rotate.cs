using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
    }
}
