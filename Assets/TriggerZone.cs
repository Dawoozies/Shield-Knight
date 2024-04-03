using System.Linq;
using UnityEngine;
using UnityEngine.Events;
public class TriggerZone : MonoBehaviour
{
    public string[] triggerTags;
    public UnityEvent onEnter;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (triggerTags.Contains(col.tag))
        {
            onEnter?.Invoke();
        }
    }
}
