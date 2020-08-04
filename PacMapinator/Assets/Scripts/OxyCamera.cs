using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxyCamera : MonoBehaviour
{
    public float moveSpeed = 0.2f;
    public float scrollSensitive = 0.1f;

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float scroll = Input.mouseScrollDelta.y;

        Vector3 move = new Vector3(horizontal, vertical) * moveSpeed;
        move.z += scroll * scrollSensitive;
        this.transform.Translate(move, Space.World);
    }
}
