using UnityEngine;

public abstract class LightSource : MonoBehaviour
{
    public const float LIGHT_INTEN_DIV = 4f;

    public float _surfaceArea = 1f;

    public abstract void MakePhoton( ref GIRend.Photon p );

    protected Light _light;
    public float intensity { get { return _light.intensity / LIGHT_INTEN_DIV; } } // division is a hack - low intensities look best with log tonemap

    protected virtual void Awake()
    {
        _light = GetComponent<Light>();
    }
}
