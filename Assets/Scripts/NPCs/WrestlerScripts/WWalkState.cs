using UnityEngine;

public class WWalkState : IWrestlerState
{
    Wrestler wrestler;

    public WWalkState(Wrestler thisWrestler)
    {
        this.wrestler = thisWrestler;
    }

    public void WUpdateState()
    {
        float distanceToWaypoint = Vector3.Distance(wrestler.transform.position, wrestler.waypoints[wrestler.waypointIndex].transform.position);
        if(distanceToWaypoint < 5)
        {
            ToWIdleState();
        }
    }

    public void ToWIdleState()
    {
        wrestler.anim.SetBool("Walk", false);
        wrestler.idleState.idleStartTime = Time.time;

        int randomChoice = Random.Range(0, 2);
        if(randomChoice == 0)
            SelectRandomIdleState();
        else
        {
            wrestler.anim.SetBool("RumbaDancing", true);
            wrestler.idleState.maxIdleTime = 6;
        }

            wrestler.currentState = wrestler.idleState;
    }

    public void ToWWalkState()
    {

    }

    public void SelectRandomIdleState()
    {
        int randomIndex = Random.Range(0, wrestler.idleState.animations.Length);
        string currentAnim = wrestler.idleState.animations[randomIndex];
        wrestler.idleState.currentAnim = currentAnim;

        RuntimeAnimatorController rac = wrestler.anim.runtimeAnimatorController;

        foreach(AnimationClip clip in rac.animationClips)
        {
            if(clip.name == currentAnim)
            {
                wrestler.idleState.maxIdleTime = clip.length;
                wrestler.anim.SetTrigger(wrestler.idleState.currentAnim);
                //Debug.Log("Clip length: " + clip.length);
                //Debug.Log("maxIdleTime: " + wrestler.idleState.maxIdleTime);
                break;
            }
        }

    }
}