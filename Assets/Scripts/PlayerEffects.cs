using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    [SerializeField] private Animator effectsAnimator;
    [SerializeField] private SpriteRenderer effectsRenderer;

    private void OnEnable()
    {
        PlayerMovementStates.OnPlayerMoveStateChanged += HandleStateChanged;
    }
    private void OnDisable()
    {
        PlayerMovementStates.OnPlayerMoveStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(PlayerMovementStates.MoveState state)
    {
        switch (state)
        {
            case PlayerMovementStates.MoveState.idle:
                PlayEffect("NOTHING");
                break;
            case PlayerMovementStates.MoveState.run:
                PlayEffect("RunningEffect");
                break;
            case PlayerMovementStates.MoveState.dashing:
                PlayEffect("DashingEffect");
                break;
        }
    }

    private void PlayEffect(string stateName)
    {
        effectsRenderer.enabled = true;
        effectsAnimator.Play(stateName);
    }

    public void HideEffect() 
    {
        effectsRenderer.enabled = false;
    }
}
