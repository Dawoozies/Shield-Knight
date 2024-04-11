using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionFinishDoor : MonoBehaviour
{
    bool entered;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (entered) { return; }
        if(col.CompareTag("Player"))
        {
            SectionManager.ins.StartTransitionToNextSection(() => entered = false);
            entered = true;
        }
    }
}
