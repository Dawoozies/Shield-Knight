using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class BossFinalSystem : MonoBehaviour
{
    public int lastValidStateHash;
    Animator animator;
    Player player;
    public TextMeshProUGUI endGameText;
    public float timeAfterBothDead;
    public bool inEndGame;
    public Eyeball redEye;
    public Eyeball blueEye;
    Camera mainCamera;
    float t;
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameManager.GetActivePlayer();
        player.RegisterOnDeathCompleteCallback(OnDeathCompleteHandler);
        mainCamera = Camera.main;
    }
    void OnDeathCompleteHandler()
    {
        animator.Play(lastValidStateHash, 0, 0f);
    }
    private void Update()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("ValidCheckpoint"))
        {
            lastValidStateHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        }
        if(redEye.isDying && blueEye.isDying)
        {
            endGameText.gameObject.SetActive(false);
            t += Time.deltaTime;
            if(t > timeAfterBothDead)
            {
                PlayerPrefs.SetInt("END", 1);
                SceneManager.LoadScene("Credits");
            }
        }
    }
    public void EnteredEndGame()
    {
        animator.enabled = false;
        endGameText.gameObject.SetActive(true);
        inEndGame = true;
    }
}
