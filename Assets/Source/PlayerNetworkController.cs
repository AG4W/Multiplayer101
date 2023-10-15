using UnityEngine;

using Mirror;

public class PlayerNetworkController : NetworkBehaviour
{
    [SyncVar]Vector3 position = Vector3.zero;
    [SyncVar]Quaternion rotation = Quaternion.identity;

    //important note about syncvars, they are only updated when the value has changed
    //and hooks are only called when the syncvar is updated.
    [SyncVar(hook = nameof(OnColorChanged))]Color color = Color.white;

    [Space, SerializeField]
    GameObject projectile = null;

    void Start()
    {
        //grab camera
        //this should only happen on the local player, hence use of condition
        if(base.isLocalPlayer)
            FindObjectOfType<CameraController>().SetTarget(this.gameObject);
        else
        {
            //remove local-client only functionality that is not needed on remote clients
            //the less stuff on remote clients, the better and more manageable
            Destroy(this.GetComponent<PlayerMovementController>());
        }
    }
    void Update()
    {
        //position and rotation has already been updated on local player, we are effectively doing client-side prediction
        //we only need to send the data to the server
        if (base.isLocalPlayer)
        {
            CmdSendTransformData(this.transform.position, this.transform.rotation);
        }
        //if we are not the local client, we should interpolate towards the synchronized data to get smooth movement on other players
        else
        {
            this.transform.SetPositionAndRotation(Vector3.Lerp(this.transform.position, position, .25f), Quaternion.Lerp(this.transform.rotation, rotation, .25f));
        }
    }

    //it's always a good idea to protect and encapsulate network calls from outside sources.
    public void RandomizeColor()
    {
        //this method might get called both on servers AND clients, so we need to check, since server shouldn't call Cmds
        if (base.isServer)
            ServerRandomizeColor();
        else
            CmdRandomizeColor();
    }
    public void ShootProjectile() => CmdSpawnProjectile();

    [Command]
    void CmdSendTransformData(Vector3 position, Quaternion rotation)
    {
        //this is running on the server, here we could apply anti-cheat measures/check for movement validity etc.
        this.position = position;
        this.rotation = rotation;
    }
    [Command]
    void CmdRandomizeColor()
    {
        //ask the server to give us a random color
        ServerRandomizeColor();
    }
    void ServerRandomizeColor()
    {
        color = new(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
    [Command]
    void CmdSpawnProjectile()
    {
        //create object locally
        GameObject item = Instantiate(projectile, this.transform.position + Vector3.up + Vector3.up + this.transform.forward, this.transform.rotation, null);

        //spawn propagates the instatiated objects on clients, and sets up network identity links.
        NetworkServer.Spawn(item);
    }

    //hooks always have this signature: T old, T new - where T is the type of the syncvar, in this case: Color
    //hooks are the perfect place to spawn models/effects when changing abilities/equipment on characters or similar.
    void OnColorChanged(Color old, Color @new)
    {
        //this method runs on every client, both local and remote whenever this syncvar has changed
        //let's update our player character color
        MeshRenderer[] renderers = this.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = color;
    }
}
