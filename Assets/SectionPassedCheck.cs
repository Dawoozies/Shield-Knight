using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class SectionPassedCheck : MonoBehaviour
{
    //Player has to be in the trigger for this long for the pass to be valid
    public float passTimeCheck;
    public UnityEvent onSectionPassed;
    float t;
    bool sectionPassed;
    private void Update()
    {
        if(t > passTimeCheck && !sectionPassed)
        {
            onSectionPassed?.Invoke();
            sectionPassed = true;
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            t += Time.fixedDeltaTime;
        }
    }
}
