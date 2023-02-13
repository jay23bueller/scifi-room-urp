using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController _characterController;
    [Header("Movement Settings")]
    [SerializeField]
    private float _moveSpeed = 3.5f;
    [SerializeField]
    private float _sprintMultiplier = 2f;
    private float _defaultMultiplier = 1f;
    private float _currentSpeedMultiplier;
    private Vector3 _movementInput;
    private float _gravity = -9.8f;

    [Header("Mouse Settings")]
    [SerializeField]
    private float _mouseXSensitivity = 1f;
    [SerializeField]
    private float _mouseYSensitivity = 1f;
    private float _currentCameraXRotation;
    private Transform _cameraTransform;
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        foreach(Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.gameObject.name != "Player") { _cameraTransform = t; break; }
        }
        _movementInput = Vector3.zero;
        _currentSpeedMultiplier = _defaultMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        Rotation();
        CheckForSprinting();
        ReadMovementInput();
        ApplyGravity();
        MovePlayer();
    }

    private void Rotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * _mouseXSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * _mouseYSensitivity;
        
        _currentCameraXRotation = Mathf.Clamp(_currentCameraXRotation - mouseY, -90f, 90f);

        //Don't need to limit rotation since we can do a full 360 degree rotation around the y-axis
        transform.Rotate(0f, mouseX, 0f);

        //Need to use localRotation since we are only rotating it relative to the Player GameObject
        _cameraTransform.localRotation = Quaternion.Euler(_currentCameraXRotation, 0f, 0f);


    }

    private void CheckForSprinting()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            _currentSpeedMultiplier = _sprintMultiplier;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            _currentSpeedMultiplier = _defaultMultiplier;
    }

    private void MovePlayer()
    {
        _characterController.Move(_movementInput);
    }

    private void ApplyGravity()
    {
        if(!_characterController.isGrounded)
        {
            _movementInput.y += _gravity * Time.deltaTime;
        }
        
    }

    private void ReadMovementInput()
    {
        float forwardInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        //Vector3 directionRelativeToCamera = Quaternion.AngleAxis(_cameraTransform.transform.rotation.eulerAngles.y, Vector3.up) * new Vector3(horizontalInput, 0f, verticalInput).normalized;
        //_movementInput = (directionRelativeToCamera * _moveSpeed * _currentSpeedMultiplier * Time.deltaTime);
        
        //Since the y rotation of the Camera will be the same as the player, we can just use 
        //the player's transform's forward and right vectors
        float deltaSpeed = _moveSpeed * _currentSpeedMultiplier * Time.deltaTime;
        _movementInput = ((transform.forward*forwardInput + transform.right*horizontalInput)*deltaSpeed);
    }
}
