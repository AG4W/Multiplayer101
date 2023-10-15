using UnityEngine;

using Mirror;

public class ProjectileController : NetworkBehaviour
{
    [SyncVar]
    Vector3 position = Vector3.zero;

    [SerializeField]
    float speed = 100f;

    float lifetime = 0f;

    void Start()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();

        if (base.isServer)
        {
            rigidbody.AddForce(this.transform.forward * speed, ForceMode.Impulse);
        }
        else
        {
            Destroy(rigidbody);
        }
    }
    void FixedUpdate()
    {
        //as this runs on the server, we don't need to use a command and can instead update the syncvar directly
        if (base.isServer)
        {
            position = this.transform.position;

            lifetime += Time.fixedDeltaTime;

            //cleanup old projectiles
            if (lifetime >= 30f)
                NetworkServer.Destroy(this.gameObject);
        }
        else
            this.transform.position = Vector3.Lerp(this.transform.position, position, .25f);
    }

    void OnCollisionEnter(Collision collision)
    {
        //only server checks for damage
        if (!base.isServer)
            return;

        //if the hit object is a player
        if(collision.transform.root.TryGetComponent<PlayerNetworkController>(out PlayerNetworkController controller))
            controller.RandomizeColor();
    }
}
