using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float Attackcooldown;
    [SerializeField] private Transform FirePoint;
    [SerializeField] private GameObject[] Fireballs;

    private Animator Animation;
    private PlayerMovement Movement;
    private float Cooldowntimer = Mathf.Infinity;

    private void Awake()
    {
        Animation = GetComponent<Animator>();
        Movement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButton(0) && Cooldowntimer > Attackcooldown && Movement.canAttack())
            Animation.SetTrigger("Attack");
        Cooldowntimer += Time.deltaTime;
    }

    private void Attack()
    {
        Cooldowntimer = 0;

        // Audio Hook: Plays magic fireball spell whoosh sound when launched
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFireballNoise();
        }

        // pools the fireballs
        Fireballs[FindFireball()].transform.position = FirePoint.position;
        Fireballs[FindFireball()].GetComponent<Fireball>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private int FindFireball()
    {
        for (int i = 0; i < Fireballs.Length; i++)
        {
            if (!Fireballs[i].activeInHierarchy)
                return i;
        }

        return 0;
    }
}


