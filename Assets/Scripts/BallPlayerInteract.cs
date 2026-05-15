using UnityEngine;

public class BallPlayerInteract : MonoBehaviour
{
    public float powerHit = 5f;
    public float upForce = 0.5f;
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            Rigidbody rbMinge = GetComponent<Rigidbody>();

            if (rbMinge != null)
            {
                Vector3 directie = transform.position - collision.gameObject.transform.position;

                directie.y = 0;
                directie = directie.normalized + (Vector3.up * upForce);

                rbMinge.AddForce(directie * powerHit, ForceMode.Impulse);

                collision.gameObject.GetComponent<SoccerAgent>()?.OnBallHit();
            }
        }
    }
}