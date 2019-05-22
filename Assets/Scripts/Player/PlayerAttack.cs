// ================================================================================================================================
// File:        PlayerAttack.cs
// Description: Allows the player to attack with their melee weapon by clicking the RMB
// ================================================================================================================================

using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator AnimationController;
    public GameObject AttackBox;
    public PlayerResources Resources;
    public PlayerCharacterController Controller;

    void Update()
    {
        //The player must have a positive amount of stamina and not already be attacking to perform a new attack
        AnimatorStateInfo StateInfo = AnimationController.GetCurrentAnimatorStateInfo(0);
        bool CurrentlyAttacking = StateInfo.IsName("Attack 1") || StateInfo.IsName("Attack 2");
        bool CurrentlyRolling = StateInfo.IsName("Rolling");
        //They must also be touching the ground
        bool CanAttack = Resources.CurrentStamina >= 1 && !CurrentlyAttacking && Controller.TouchingGround() && !CurrentlyRolling;

        if (Input.GetMouseButtonDown(1) && CanAttack)
        {
            //Normal Attacks take 20 Stamina, and delay regen by 0.5 seconds
            Resources.CurrentStamina -= 20;
            Resources.StaminaRegenDelay = 0.5f;

            //Perform an attack during the first attacks recovery phase will perform the 2nd attack in the combo
            if (StateInfo.IsName("Attack 1 Recover"))
                AnimationController.SetTrigger("Attack 2");
            else
                AnimationController.SetTrigger("Attack 1");

            //Tell the game server where our attack landed
            if(ConnectionManager.Instance.IsConnected)
                PlayerManagementPacketSender.SendPlayerAttack(AttackBox.transform.position, AttackBox.transform.localScale, AttackBox.transform.rotation);
        }
    }
}
