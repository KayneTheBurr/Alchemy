using KE;
using UnityEngine;

public class CharacterMovementManager : MonoBehaviour
{
    public CharacterManager character;

    [Header("Movement Flags")]
    public ObservableVariable<bool> isMoving = new ObservableVariable<bool>(false);
    public bool isSprinting = false;
    public bool isDodging = false;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void OnEnable()
    {
        isMoving.OnValueChanged += character.OnIsMovingChanged;
    }
    protected virtual void OnDisable()
    {
        isMoving.OnValueChanged -= character.OnIsMovingChanged;
    }
    protected virtual void Update()
    {

    }
}
