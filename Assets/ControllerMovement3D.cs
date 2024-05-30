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

    [Header("Jumping")]
    [SerializeField] private float _gravity = -20f;
    [SerializeField] private float _jumpHeight = 2.5f;
    private Vector3 _velocity;
    

    [Header("Grounding")]
    [SerializeField] private float _groundCheckOffset = 0f;
    [SerializeField] private float _groundCheckDistance = 0.4f;
    [SerializeField] private float _groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask _groundMask;
    private bool _isGrounded;
    private Vector3 _groundNormal;

    private CharacterController _characterController;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    // This method is used to set the movement input for the character.
    public void SetMoveInput(Vector3 input)
    {
        // Check if the player has pressed any keys. If the input magnitude is greater than 0.1,
        // set '_hasMoveInput' to true to avoid stick drag or floating-point error. Otherwise, set it to zero.
        _hasMoveInput = input.magnitude > 0.1f;
        _moveInput = _hasMoveInput ? input : Vector3.zero;
    }

    // This method is used to set the look direction for the character.
    public void SetLookDirection(Vector3 direction)
    {
        // We only get axis x and z because the camera only moves horizontally.
        // Rotate the player.
        _lookDirection = new Vector3(direction.x, 0f, direction.z).normalized;
    }

    // This method is called when the player wants to jump.
    public void Jump()
    {
        // Check if the player is grounded before allowing the jump.
        if (!_isGrounded) return;

        // Calculate jump velocity from jump height and gravity.
        float jumpVelocity = Mathf.Sqrt(2f * -_gravity * _jumpHeight);
        _velocity = new Vector3(0, jumpVelocity, 0);
    }
    private void FixedUpdate() 
    {
        // Check if the player is grounded.
        _isGrounded = CheckGround();

        // Apply gravity over time.
        _velocity.y += _gravity * Time.fixedDeltaTime;
        _characterController.Move(_velocity * Time.fixedDeltaTime); // Use calculated in Jump()

        //_speed = 0; // Reset speed to 0 at the beginning of each FixedUpdate.

        // If player is not moving, set movement to zero.
        if (_moveInput.magnitude < 0.1f)
        {
            _moveInput = Vector3.zero; // make movement to zero if magnitude is too small
            return; // Having this line, we may not need to Reset speed to 0
        }

        // Move character.
        if (_moveInput != Vector3.zero)
        {
            _speed = _moveSpeed; // If player is moving
        }
        
        float targetRotation = Quaternion.LookRotation(_lookDirection).eulerAngles.y + _mainCamera.transform.rotation.eulerAngles.y;
        //UnityEngine.Quaternion rotation = UnityEngine.Quaternion.Euler(0, targetRotation, 0);
        //transform.rotation = UnityEngine.Quaternion.Slerp(transform.rotation, rotation, _turnSpeed * Time.fixedDeltaTime); // Smooth the rotation

        Quaternion rotation = Quaternion.Euler(0, targetRotation, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _turnSpeed * Time.fixedDeltaTime); // Smooth rotation.

        _moveInput = rotation * Vector3.forward;
        _characterController.Move(_moveInput * _speed * Time.fixedDeltaTime); // Let CharacterController move the character.
    }

    // Check if the player is grounded using a sphere cast.
    private bool CheckGround()
    {
        // Start position for the sphere cast.
        Vector3 start = transform.position + Vector3.up * _groundCheckOffset;

        // Perform sphere cast.
        if (Physics.SphereCast(start, _groundCheckRadius, Vector3.down, out RaycastHit hit, _groundCheckDistance, _groundMask))
        {
            // If the player is grounded, save the ground normal and return true.
            _groundNormal = hit.normal;
            return true;
        }
        _groundNormal = Vector3.up;
        return false;
    }

    // Draw debug spheres for ground checking.
    private void OnDrawGizmosSelected() 
    {
        // Set gizmos color.
        //Gizmos.color = Color.red;
        //if (_isGrounded) Gizmos.color = Color.green;
        Gizmos.color = _isGrounded ? Color.green : Color.red;

        // Find start/end positions of sphere cast.
        Vector3 start = transform.position + Vector3.up * _groundCheckOffset;
        Vector3 end = start + Vector3.down * _groundCheckDistance;

        // Draw wire spheres.
        Gizmos.DrawWireSphere(start, _groundCheckRadius);
        Gizmos.DrawWireSphere(end, _groundCheckRadius);
    }
}
