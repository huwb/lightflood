using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightSourceCone : LightSource
{
    [Range( 0f, 90f )]
    public float _coneAngleDeg = 20f;

    public override void MakePhoton( ref GIRend.Photon p )
    {
        p.position = transform.position;

        // construct a random direction within cone angle
        float azimuth = Random.value * Mathf.PI * 2f;
        float altitude = Random.value * _coneAngleDeg * Mathf.Deg2Rad;

        Vector3 local;
        local.x = Mathf.Cos( azimuth ) * Mathf.Sin( altitude );
        local.y = Mathf.Sin( azimuth ) * Mathf.Sin( altitude );
        local.z = Mathf.Sqrt( 1f - local.x * local.x - local.y * local.y );

        p.next_dir = local.x * transform.right + local.y * transform.up + local.z * transform.forward;


        float dotmin = Mathf.Cos( 0.5f * _light.spotAngle * Mathf.Deg2Rad );
        do { p.next_dir = Random.onUnitSphere; }
        while( Vector3.Dot( transform.forward, p.next_dir ) <= dotmin );

        Color col = intensity * _light.color;
        p.intensity = new Vector3( col.r, col.g, col.b );
    }
}
