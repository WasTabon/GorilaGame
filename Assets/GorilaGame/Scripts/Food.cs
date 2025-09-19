using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Food : MonoBehaviour
{
    public FoodType foodType;
    
    [Header("Physics Settings")]
    [SerializeField] private float fallSpeed = 3f;
    [SerializeField] private float mass = 1f;
    [SerializeField] private float drag = 2f;
    [SerializeField] private bool useCustomGravity = true;
    
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetupPhysics();
    }
    
    private void SetupPhysics()
    {
        if (rb == null) return;
        
        rb.mass = mass;
        rb.drag = drag;
        
        if (useCustomGravity)
        {
            rb.useGravity = false;
        }
    }
    
    void FixedUpdate()
    {
        if (useCustomGravity && rb != null)
        {
            ApplyCustomGravity();
        }
    }
    
    private void ApplyCustomGravity()
    {
        Vector3 customGravity = Vector3.down * fallSpeed;
        rb.AddForce(customGravity, ForceMode.Acceleration);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}