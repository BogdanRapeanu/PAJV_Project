using UnityEngine;

public class BallPlayerInteract : MonoBehaviour
{
    public float powerHit = 5f;
    public float upForce = 0.5f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            Rigidbody rbMinge = collision.gameObject.GetComponent<Rigidbody>();

            if (rbMinge != null)
            {
                Vector3 directieLovitura = collision.gameObject.transform.position - transform.position;

                directieLovitura.y = 0;
                directieLovitura = directieLovitura.normalized + (Vector3.up * upForce);

                rbMinge.AddForce(directieLovitura * powerHit, ForceMode.Impulse);

                Debug.Log("Ball hit!");
            }
        }
    }
}