using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField]
    float movementSpeed = 5f;

    [Space, SerializeField]
    float rotationSpeed = 100f;

    Vector3 targetPosition;
    float targetRotation;

    Vector3 movementInput = Vector3.zero;
    float rotationInput = 0f;

    PlayerNetworkController playerNetworkController;

    void Start()
    {
        //lock cursor so rotation doesn't feel like shit
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerNetworkController = GetComponent<PlayerNetworkController>();
    }
    void FixedUpdate()
    {
        UpdatePositionAndRotation();
    }
    void Update()
    {
        GatherInput();
        CalculateMovement();
    }

    void GatherInput()
    {
        movementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        rotationInput = Input.GetAxis("Mouse X");

        if (Input.GetKeyDown(KeyCode.Space))
            GetComponent<PlayerNetworkController>().ShootProjectile();
    }
    void CalculateMovement()
    {
        //transform direction from world to local space so we move forward along player axis, and not world axis
        targetPosition += this.transform.TransformDirection(movementSpeed * Time.deltaTime * movementInput);
        targetRotation += rotationSpeed * Time.deltaTime * rotationInput;
    }

    void UpdatePositionAndRotation()
    {
        //add some interpolation to smooth out eventual stutters/jitters
        this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, .25f);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(Vector3.up * targetRotation), .25f);
    }
}
