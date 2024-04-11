using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionManager : MonoBehaviour
{
    public static SectionManager ins;
    private void Awake()
    {
        ins = this;
    }
    public Transform playerSpawn;
    public Transform[] sectionCheckpoints;
    int section = 0;
    SectionTransition sectionTransition;
    Player player => GameManager.GetActivePlayer();
    
    void Start()
    {
        sectionTransition = GetComponent<SectionTransition>();
        sectionTransition.RegisterTransitionMidpointCallback(MovePlayerToNextSection);
    }
    public void StartTransitionToNextSection(Action a)
    {
        sectionTransition.StartTransition();
        transitionCompleteCallback = a;
    }
    Action transitionCompleteCallback;
    public void MovePlayerToNextSection()
    {
        section++;
        player.transform.position = sectionCheckpoints[(section-1)%sectionCheckpoints.Length].position;
        transitionCompleteCallback();
    }
}
