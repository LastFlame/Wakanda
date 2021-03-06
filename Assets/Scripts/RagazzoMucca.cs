﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagazzoMucca : MonoBehaviour {

    public TurnManager turnManager;
    private PlayerActions player; //da mettere se si vuole gestire la morte del player tramite un metodo
    private FieldOfView fieldOfView;

    public bool isMyTurn;
    public bool isSleeping;
    public bool hasSeenPlayer;
    private bool imDead;
    private float waitTimer = 1f;
    private float sleepingView;
    private float originalViewAngle;
    public Transform enemyRear;
    public Transform spriteTransform;
    public Animator anim;
    private SpriteRenderer sprite;
    private AudioSource ragazzoMuccaSoundPlayer;
    public AudioClip ragazzoMuccaAttackSound;
    public Transform soundVisualization;
    private bool soundTrigger = true;
    private bool canKillPlayer = true;

    public GameObject thunder;

    private void Start()
    {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        player = GameObject.Find("Player").GetComponent<PlayerActions>();
        fieldOfView = GetComponent<FieldOfView>();
        originalViewAngle = fieldOfView.viewAngle;
        ragazzoMuccaSoundPlayer = GetComponent<AudioSource>();
        sprite = anim.GetComponent<SpriteRenderer>();
    }

    float AngleToPositive(float angle)
    {
        if (angle > 359)
        {
            return angle - 360;
        }
        else if (angle < 0)
        {
            return 360 - angle;
        }
        else return angle;
    }

    void Update ()
    {
        if (AngleToPositive(transform.rotation.eulerAngles.z) > 45 && AngleToPositive(transform.rotation.eulerAngles.z) < 225)
        {
            sprite.flipX = true;
        }
        else
        {
            sprite.flipX = false;
        }
        anim.SetBool("isSleeping", isSleeping);
        anim.SetFloat("angle", transform.rotation.eulerAngles.z); 
        hasSeenPlayer = fieldOfView.FindVisibleTarget();

	    if (player.isMyTurn && !imDead)
        {
             if (!isSleeping && hasSeenPlayer)
             {
                KillThePlayer();
             }
        }
        if ( isSleeping)
        {
            fieldOfView.viewAngle = 0;
        }
        else
        {
            fieldOfView.viewAngle = originalViewAngle;
        }
	}

    IEnumerator ChangeTurnDelay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        turnManager.changeTurn();
    }
	
	public void StartTurn()
    {
        Debug.Log("i'm in");
        if (imDead)
        {
            StartCoroutine("ChangeTurnDelay", waitTimer);
            return;
        }

       // isMyTurn = true;
        SleepingManager();
        

        if (isSleeping)
        {
            // feeback snore
           isMyTurn = false;
           StartCoroutine("ChangeTurnDelay", waitTimer);
            
        }
        else
        {
            // remove snore feedback
            if (hasSeenPlayer)
            {
                KillThePlayer();
                
            }
            else
            {
                isMyTurn = false;
                StartCoroutine("ChangeTurnDelay", waitTimer);
                

            }
        }
    }

    public void KillThePlayer()
    {
        if (canKillPlayer)
        {
            Quaternion rot = new Quaternion();
            rot.eulerAngles = new Vector3(-35, -45, 60);
            GameObject clone = Instantiate(thunder, player.transform.position, rot);
            Destroy(clone, 1);
            // animazione sparo
            anim.SetTrigger("Attack"); 
            // bool player morto
            if (soundTrigger)
            {
                ragazzoMuccaSoundPlayer.clip = ragazzoMuccaAttackSound;
                ragazzoMuccaSoundPlayer.Play();
                soundTrigger = false;
            }
            player.Die();
            Debug.Log("Pew pew pew. Git Gud sei morto casual");
            canKillPlayer = false;
        }       
    }

    private void SleepingManager()
    {
        if (isSleeping)
        {
            isSleeping = false;
        }
        else
        {
            isSleeping = true;
        }
    }

    public void Die()
    {
        anim.SetTrigger("Die");
        sprite.transform.localScale *= 4;
        imDead = true;
        StartCoroutine(DieDelay());
    }

    IEnumerator DieDelay()
    {
        yield return new WaitForSeconds(0.5f);
        this.transform.position = new Vector3(100, 100, 100);
    }

    private void LateUpdate()
    {
        enemyRear.position = transform.position - transform.up;
        spriteTransform.position = transform.position;
        soundVisualization.rotation = transform.rotation;
    }
}
