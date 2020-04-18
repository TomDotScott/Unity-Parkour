using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 GetInput()
    {
        Vector2 i = Vector2.zero;
        i.x = UnityEngine.Input.GetAxis("Horizontal");
        i.y = UnityEngine.Input.GetAxis("Vertical");
        i *= (i.x != 0.0f && i.y != 0.0f) ? .7071f : 1.0f;
        return i;
    }

    public Vector2 GetDown() { return _down; }

    public Vector2 GetRaw()
    {
        Vector2 i = Vector2.zero;
        i.x = UnityEngine.Input.GetAxisRaw("Horizontal");
        i.y = UnityEngine.Input.GetAxisRaw("Vertical");
        i *= (i.x != 0.0f && i.y != 0.0f) ? .7071f : 1.0f;
        return i;
    }

    public bool GetRun() { return UnityEngine.Input.GetKey(KeyCode.LeftShift); }

    public bool GetCrouch() { return UnityEngine.Input.GetKeyDown(KeyCode.C); }

    public bool GetCrouching() { return UnityEngine.Input.GetKey(KeyCode.C); }

    public bool GetLeftClick() { return UnityEngine.Input.GetMouseButtonDown(0); }

    public bool GetGrowButton() { return UnityEngine.Input.GetKeyDown(KeyCode.E); }

    public bool GetShrinkButton() { return UnityEngine.Input.GetKeyDown(KeyCode.Q); }

    private Vector2 previous;
    private Vector2 _down;

    private int jumpTimer;
    private bool jump;

    void Start()
    {
        jumpTimer = -1;
    }

    void Update()
    {
        _down = Vector2.zero;
        if (GetRaw().x != previous.x)
        {
            previous.x = GetRaw().x;
            if (previous.x != 0)
            {
                _down.x = previous.x;
            }
        }
        if (GetRaw().y != previous.y)
        {
            previous.y = GetRaw().y;
            if (previous.y != 0)
            {
                _down.y = previous.y;
            }
        }
    }

    public void FixedUpdate()
    {
        if (!UnityEngine.Input.GetKey(KeyCode.Space))
        {
            jump = false;
            jumpTimer++;
        }
        else if (jumpTimer > 0)
        {
            jump = true;
        }
    }

    public bool Jump()
    {
        return jump;
    }

    public void ResetJump()
    {
        jumpTimer = -1;
    }
}
