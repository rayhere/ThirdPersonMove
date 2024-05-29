using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ControllerMovement3D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _turnSpeed = 10f;
    [SerializeField] private GameObject _mainCamera;
    // Used to synchronize the rotation of the main camera with the character's position.

    private float _speed = 0f;
    private bool _hasMoveInput;
    private Vector3 _moveInput;
    private Vector3 _lookDirection;

    private CharacterController _characterController;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    // This method is used to set the movement input for the character.
    public void SetMoveInput(Vector3 input)
    {
        // Check if the player presses the key or not.
        // It checks if the player has pressed any keys. If the input magnitude is greater than 0.1,
        // it sets '_hasMoveInput' to true to avoid stick drag or floating-point error. Otherwise, it sets it to zero.
        _hasMoveInput = input.magnitude > 0.1f;
        _moveInput = _hasMoveInput ? input : Vector3.zero;
    }

    // to make the character actually rotate
    public void SetLookDirection(Vector3 direction)
    {
        // We only get axis x and z because the camera only moves horizontally.
        // Rotate the player.
        _lookDirection = new Vector3(direction.x, 0f, direction.z).normalized;
    }

    private void FixedUpdate() {
        _speed = 0;

        // If player not moving
        float targetRotation = 0f;

        if (_moveInput.magnitude < 0.1f)
        {
            _moveInput = Vector3.zero; // make movement to zero if magnitude is too small
            return;
        }

        // Move character
        if (_moveInput != Vector3.zero)
        {
            _speed = _moveSpeed; // If player is moving
        }

        targetRotation = Quaternion.LookRotation(_lookDirection).eulerAngles.y + _mainCamera.transform.rotation.eulerAngles.y;
        UnityEngine.Quaternion rotation = UnityEngine.Quaternion.Euler(0, targetRotation, 0);
        transform.rotation = UnityEngine.Quaternion.Slerp(transform.rotation, rotation, _turnSpeed * Time.fixedDeltaTime); // Smooth the rotation

        _moveInput = rotation * Vector3.forward;
        _characterController.Move(_moveInput * _speed * Time.fixedDeltaTime); // Let CharacterController move the character
    }
}
