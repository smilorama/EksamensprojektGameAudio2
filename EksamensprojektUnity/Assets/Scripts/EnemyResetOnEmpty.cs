using UnityEngine;

public class EnemyResetOnEmpty : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Enemy enemy = animator.GetComponentInParent<Enemy>();
        if (enemy != null)
            enemy.OnActionComplete();
    }
}
