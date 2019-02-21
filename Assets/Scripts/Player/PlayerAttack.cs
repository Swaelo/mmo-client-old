using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float AttackCooldown = 0.25f;
    public float AttackCooldownRemaining = 0.25f;
    [SerializeField] private Animator AnimationController;
    [SerializeField] private GameObject AttackBox;

    void Update()
    {
        AttackCooldownRemaining -= Time.deltaTime;
        if(AttackCooldownRemaining <= 0.0f && Input.GetMouseButtonDown(1))
        {
            //Start the attack cooldown, play the attack animation and tell the server where our attack landed
            AttackCooldownRemaining = AttackCooldown;
            AnimationController.SetTrigger("Attack");
            PacketSender.Instance.SendPlayerAttack(AttackBox.transform.position, AttackBox.transform.localScale, AttackBox.transform.rotation);
        }
    }
}
