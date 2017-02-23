using UnityEngine;

public class GIRend : MonoBehaviour
{
    public struct Photon
    {
        public Vector3 position;
        public Vector3 next_dir;
        public Vector3 intensity;
        public bool alive;
    }

    Photon[] _photons;

    public int maxPhotons = 10;

    [Range( 0.95f, 1f )]
    public float _alpha = 0.99f;

    public int _resPerMeter = 256;

    [Range( 0f, 0.2f )]
    public float _blurAmt = 0.05f;

    [Range( 0f, 1f )]
    public float _exposure = 0.5f;

    [Range( 0f, 1f )]
    public float _contrast = 0.5f;

    LightSource[] _lights;
    float _totalLightSurfaceArea = 0f;

    public bool _render = true;

    void Start()
    {
        UpdateLights();
	}
	
    void UpdateLights()
    {
        _lights = FindObjectsOfType<LightSource>();

        _totalLightSurfaceArea = 0f;
        foreach( var light in _lights )
            _totalLightSurfaceArea += light._surfaceArea /** light.intensity*/;
    }

    void Update()
    {
        UpdateLights();

        if( !_render )
            return;

        if( _photons == null )
            _photons = new Photon[maxPhotons];

        // add new photons
        if( _lights.Length > 0 )
        for( int i = 0; i < _photons.Length; i++  )
        {
            if( !_photons[i].alive )
            {
                float u = Random.value * _totalLightSurfaceArea;
                LightSource light = null;
                float cdf = 0f;
                for( int li = 0; li < _lights.Length; li++ )
                {
                    cdf += /*_lights[li].intensity **/ _lights[li]._surfaceArea;

                    if( u <= cdf )
                    {
                        light = _lights[li];
                        break;
                    }
                }

                if( light == null ) light = _lights[_lights.Length - 1];

                light.MakePhoton( ref _photons[i] );

                _photons[i].alive = true;
            }
        }

        // trace photons
        for( int i = 0; i < _photons.Length; i++ )
        {
            //// the should all be alive - no branch needed
            //if( !_photons[i].alive )
            //    continue;

            RaycastHit hitInfo;
            if( Physics.Raycast( _photons[i].position + 0.01f * _photons[i].next_dir, _photons[i].next_dir, out hitInfo ) )
            {
                _photons[i].intensity /= 1f + (hitInfo.point - _photons[i].position).magnitude * 0.1f;
                //_photons[i].intensity /= Mathf.Max( 1f, (hitInfo.point - _photons[i].position).sqrMagnitude );

                //if( Random.value < 0.2f )
                //    Debug.DrawLine( _photons[i].position, hitInfo.point, new Color( 1f, 1f, 1f, _photons[i].intensity.magnitude*0.1f ), 1f );

                Color existingRadiance = Color.black;

                LightFlood lf = hitInfo.collider.GetComponent<LightFlood>();
                if( lf )
                    lf.Hit( ref _photons[i], ref hitInfo, out existingRadiance );

                // cosine rule for energy loss
                _photons[i].intensity *= Vector3.Dot( hitInfo.normal, -_photons[i].next_dir );
                //_photons[i].intensity += 0.1f * new Vector3( existingRadiance.r, existingRadiance.g, existingRadiance.b );
                _photons[i].intensity = new Vector3( _photons[i].intensity.x * lf.color.r, _photons[i].intensity.y * lf.color.g, _photons[i].intensity.z * lf.color.b );
                // add whatever radiance was there before as well

                _photons[i].position = hitInfo.point;

                if( Vector3.Dot( _photons[i].next_dir = Random.onUnitSphere, hitInfo.normal ) < 0f )
                    _photons[i].next_dir = -_photons[i].next_dir;

                if( _photons[i].intensity.sqrMagnitude < 0.01f ) _photons[i].alive = false;
            }
            else
            {
                //Debug.DrawLine( _photons[i].position, _photons[i].position + 100f * _photons[i].next_dir, new Color( 1f, 1f, 1f, _photons[i].intensity * 0.01f ), 1f );

                // photon escaped scene
                _photons[i].alive = false;
            }
        }
    }

    static GIRend _instance = null;
    public static GIRend Instance { get { return _instance != null ? _instance : (_instance = FindObjectOfType<GIRend>()); } }
}
