using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Vector2 mouseAbsolute;
    Vector2 smoothMouse;

    public GameObject characterBody;

    [SerializeField]
    private Vector2 clampInDegrees = new Vector2(360, 180);
    [SerializeField]
    private Vector2 sensitivity = new Vector2(2, 2);
    [SerializeField]
    private Vector2 smoothing = new Vector2(3, 3);
    [SerializeField]
    private Vector2 targetDirection;
    [SerializeField]
    private Vector2 targetCharacterDirection;

    void Start()
    {
        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;

        // Set target direction for the character body to its inital state.
        if (characterBody)
        {
            targetCharacterDirection = characterBody.transform.localRotation.eulerAngles;
        }
    }

    void Update()
    {
        if (!GameManager.Instance.IsPaused && !GameManager.Instance.GameOver)
        {
            // Ensure the cursor is always locked when set
            Cursor.lockState = CursorLockMode.Locked;

            // Allow the script to clamp based on a desired target value.
            var targetOrientation = Quaternion.Euler(targetDirection);
            var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

            // Get raw mouse input for a cleaner reading on more sensitive mice.
            var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            // Scale input against the sensitivity setting and multiply that against the smoothing value.
            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

            // Interpolate mouse movement over time to apply smoothing delta.
            smoothMouse.x = Mathf.Lerp(smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
            smoothMouse.y = Mathf.Lerp(smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

            // Find the absolute mouse movement value from point zero.
            mouseAbsolute += smoothMouse;

            // Clamp and apply the local x value first, so as not to be affected by world transforms.
            if (clampInDegrees.x < 360)
            {
                mouseAbsolute.x = Mathf.Clamp(mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);
            }

            // Then clamp and apply the global y value.
            if (clampInDegrees.y < 360)
            {
                mouseAbsolute.y = Mathf.Clamp(mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);
            }

            transform.localRotation = Quaternion.AngleAxis(-mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;


            characterBody.transform.localRotation = Quaternion.AngleAxis(mouseAbsolute.x, Vector3.up)
                                                    * targetCharacterOrientation;

        }
    }
}
