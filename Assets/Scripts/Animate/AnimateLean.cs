using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateLean : MonoBehaviour
{
    public float lerpSpeed = 0.125f;

    private Vector2 lean;
    private Vector2 actualLean;
    private Animator animator;
    private static readonly int X = Animator.StringToHash("x");
    private static readonly int Y = Animator.StringToHash("y");

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Mathf.Abs(lean.x - actualLean.x) > 0.02f)
        {
            actualLean.x = Mathf.Lerp(actualLean.x, lean.x, lerpSpeed);
        }

        if (Mathf.Abs(lean.y - actualLean.y) > 0.02f)
        {
            actualLean.y = Mathf.Lerp(actualLean.y, lean.y, lerpSpeed);
        }

        animator.SetFloat(X, actualLean.x);
        animator.SetFloat(Y, actualLean.y);
    }

    public void SetLean(Vector2 set)
    {
        lean = set;
    }
}