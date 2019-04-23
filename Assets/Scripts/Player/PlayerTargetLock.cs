// ================================================================================================================================
// File:        PlayerTargetLock.cs
// Description: Allows player to lock onto their target enemy for better controls during combat
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTargetLock : MonoBehaviour
{
    public float MaxTargetDistance;
    public GameObject CurrentTarget = null;
    private Image TargettingReticle;
    private RectTransform ReticleRect;
    private RectTransform CanvasRect;
    private Camera PlayerCam;

    private void Awake()
    {
        PlayerCam = GetComponent<PlayerCharacterController>().CameraTransform.gameObject.GetComponent<Camera>();

        GameObject ReticleObject = GameObject.Find("Reticle");
        if(ReticleObject != null)
            TargettingReticle = ReticleObject.GetComponent<Image>();


        //Make sure the targetting reticle is hidden to start off with
        if (TargettingReticle)
        {
            TargettingReticle.enabled = false;
            ReticleRect = TargettingReticle.rectTransform;
            CanvasRect = TargettingReticle.canvas.GetComponent<RectTransform>();
        }
    }

    private void Update()
    {
        //Clicking the middle mouse button targets the nearest enemy, or drops the current target
        if(Input.GetKeyDown(KeyCode.Q))
        {
            if (CurrentTarget == null)
                AcquireNewTarget();
            else
                DropCurrentTarget();
        }

        //Keep the reticle in position when we have a target
        if (CurrentTarget != null)
            UpdateReticle();
    }

    private void AcquireNewTarget()
    {
        l.og("searching for new target");

        //Find all the enemies in the scene that are visible to the player and close enough to be targetted
        GameObject[] AllEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        l.og(AllEnemies.Length + " enemies in game");
        List<GameObject> NearbyEnemies = new List<GameObject>();
        
        for (int EnemyIter = 0; EnemyIter < AllEnemies.Length; EnemyIter++)
        {
            //Add any of the enemies that are in range to the nearby list
            float EnemyDistance = Vector3.Distance(transform.position, AllEnemies[EnemyIter].transform.position);
            if (EnemyDistance <= MaxTargetDistance)
                NearbyEnemies.Add(AllEnemies[EnemyIter]);
        }
        l.og(NearbyEnemies.Count + " are in range");

        //The nearby enemies that are visible to the player are those that can be targetted
        List<GameObject> VisibleEnemies = new List<GameObject>();
        foreach(GameObject Target in NearbyEnemies)
        {
            Vector3 ViewPosition = PlayerCam.WorldToViewportPoint(Target.transform.position);
            //Find which targets are in position of the camera view frustrum
            if(ViewPosition.x >= 0f && ViewPosition.x <= 1f)
            {
                if(ViewPosition.y >= 0f && ViewPosition.y <= 1f)
                {
                    if(ViewPosition.z >= 0f)
                    {
                        //Now we know this is inside the camera view frustrum, but it still may be behind a wall
                        //or something, so raycast to make sure theres nothing in the way
                        RaycastHit RayHit;
                        //if(Physics.Raycast(PlayerCam.transform.position, Target.transform.position - PlayerCam.transform.position, out RayHit))
                        if (Physics.Raycast(PlayerCam.transform.position, Target.transform.position - PlayerCam.transform.position, out RayHit, 1 << 9))
                        {
                            l.og("ray hit " + RayHit.transform.name);
                            //If we hit the enemy add them to the visible targets list
                            if (RayHit.transform.tag == "Enemy")
                                VisibleEnemies.Add(Target);
                        }
                    }
                }
            }
        }
        l.og(VisibleEnemies.Count + " of those are visible");

        //Figure out which one of these enemies is kind of closest to where the camera is pointing as the new target
        GameObject NewTarget = null;
        float TargetDot = -2f;
        foreach(GameObject Enemy in VisibleEnemies)
        {
            Vector3 LocalPoint = PlayerCam.transform.InverseTransformPoint(Enemy.transform.position).normalized;
            float CompareDot = Vector3.Dot(LocalPoint, Vector3.forward);
            if(CompareDot > TargetDot)
            {
                NewTarget = Enemy;
                TargetDot = CompareDot;
            }
        }

        //Now finally, if we were able to find a target out of all this, assign it as such and set the reticle
        if(NewTarget != null)
        {
            l.og(NewTarget.transform.name + " acquired as new target");
            CurrentTarget = NewTarget;
            TargettingReticle.enabled = true;
        }
    }

    private void DropCurrentTarget()
    {
        l.og("dropping target");
        CurrentTarget = null;
        TargettingReticle.enabled = false;
    }

    //Have the targetting reticle constantly aiming at the current player target lock
    private void UpdateReticle()
    {
        Vector2 ViewportPosition = PlayerCam.WorldToViewportPoint(CurrentTarget.transform.position);
        ReticleRect.anchoredPosition = new Vector2(
            ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));
    }
}
