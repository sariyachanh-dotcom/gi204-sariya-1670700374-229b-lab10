using UnityEngine;
using UnityEngine.InputSystem;

public class AirplaneFlightPhysicsSimulation : MonoBehaviour
{
    Rigidbody rb;
    bool engineOn;
    [Header("Engine")]
    [SerializeField] float thrust = 50f;
    [Header("Lift")]
    [SerializeField] float liftCoefficient = 0.1f;
    [SerializeField] float stallAngle = 30f;
    [SerializeField] float stallLiftMultiplier = 0.3f;
    [Header("Drag")]
    [SerializeField] float dragCoefficient = 0.02f;
    [SerializeField] float sideDrag = 2f;
    [Header("Control")]
    [SerializeField] float pitchPower = 5f;
    [SerializeField] float yawPower = 3f;
    [SerializeField] float rollPower = 5f;
    [Header("Turn")]
    [SerializeField] float turnStrength = 0.5f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // ทำให้เครื่องบินเสถียร
        rb.centerOfMass = new Vector3(0, -0.6f, -0.2f);
    }
    void FixedUpdate()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;
        // ===== THRUST =====
        if (kb.spaceKey.isPressed)
        {
            engineOn = true;
            rb.AddRelativeForce(Vector3.forward * thrust, ForceMode.Acceleration);
        }
        // ===== SPEED =====
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        // ===== LIFT =====
        if (engineOn && forwardSpeed > 5f)
        {
            float lift = forwardSpeed * forwardSpeed * liftCoefficient;
            float pitchAngle = Vector3.Angle(
                transform.forward,
                Vector3.ProjectOnPlane(transform.forward, Vector3.up)
            );
            if (pitchAngle > stallAngle)
            {
                lift *= stallLiftMultiplier;
            }
            rb.AddForce(transform.up * lift, ForceMode.Acceleration);
            Debug.DrawRay(transform.position, transform.up * 5f, Color.green);
        }
        // ===== DRAG =====
        Vector3 drag = -rb.linearVelocity * dragCoefficient;
        rb.AddForce(drag);
        // ===== SIDE DRAG =====
        Vector3 sideVel = Vector3.Dot(rb.linearVelocity, transform.right) * transform.right;
        rb.AddForce(-sideVel * sideDrag);
        // ===== CONTROL INPUT =====
        float pitch = 0;
        float roll = 0;
        float yaw = 0;
        if (kb.sKey.isPressed) pitch = 1;
        if (kb.wKey.isPressed) pitch = -1;
        if (kb.aKey.isPressed) roll = 1;
        if (kb.dKey.isPressed) roll = -1;
        if (kb.qKey.isPressed) yaw = -1;
        if (kb.eKey.isPressed) yaw = 1;
        // ===== TORQUE CONTROL =====
        rb.AddRelativeTorque(new Vector3(
            pitch * pitchPower,
            yaw * yawPower,
            -roll * rollPower
        ));
        // ===== BANKED TURN =====
        float bankAmount = Vector3.Dot(transform.right, Vector3.up);
        rb.AddForce(transform.right * bankAmount * forwardSpeed * turnStrength);
        // แสดงทิศทาง
        Debug.DrawRay(transform.position, transform.forward * 5f, Color.blue);
    }
}
