using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalPlatform : MonoBehaviour
{
    Animator animator;
    public void FinalSectionActivate()
    {
        animator = GetComponent<Animator>();
        animator.Play("MovePlatformUp");
    }
}
