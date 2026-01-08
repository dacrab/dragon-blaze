using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Core.Input;

[CreateAssetMenu(fileName = "InputReader", menuName = "DragonBlaze/Input/Input Reader")]
public sealed class InputReader : ScriptableObject
{
    [SerializeField] InputActionAsset inputActions;

    public event UnityAction<float> MoveEvent;
    public event UnityAction JumpEvent, JumpCanceledEvent, DashEvent, AttackEvent, InteractEvent, PauseEvent, SubmitEvent;
    public event UnityAction<Vector2> NavigateEvent;

    InputActionMap gameplayMap, uiMap;

    void OnEnable()
    {
        if (inputActions == null) return;
        gameplayMap = inputActions.FindActionMap("Gameplay");
        uiMap = inputActions.FindActionMap("UI");
        
        gameplayMap?.Enable();
        gameplayMap?["Move"].performed += ctx => MoveEvent?.Invoke(ctx.ReadValue<float>());
        gameplayMap?["Move"].canceled += _ => MoveEvent?.Invoke(0);
        gameplayMap?["Jump"].performed += _ => JumpEvent?.Invoke();
        gameplayMap?["Jump"].canceled += _ => JumpCanceledEvent?.Invoke();
        gameplayMap?["Dash"].performed += _ => DashEvent?.Invoke();
        gameplayMap?["Attack"].performed += _ => AttackEvent?.Invoke();
        gameplayMap?["Interact"].performed += _ => InteractEvent?.Invoke();
        gameplayMap?["Pause"].performed += _ => PauseEvent?.Invoke();
        
        uiMap?["Navigate"].performed += ctx => NavigateEvent?.Invoke(ctx.ReadValue<Vector2>());
        uiMap?["Submit"].performed += _ => SubmitEvent?.Invoke();
    }

    void OnDisable() => inputActions?.Disable();

    public void EnableGameplayInput() { uiMap?.Disable(); gameplayMap?.Enable(); }
    public void EnableUIInput() { gameplayMap?.Disable(); uiMap?.Enable(); }
}
