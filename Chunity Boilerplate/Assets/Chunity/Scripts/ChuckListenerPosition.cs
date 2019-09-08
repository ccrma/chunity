using UnityEngine;
using System.Runtime.InteropServices;

public class ChuckListenerPosition : MonoBehaviour
{

    #if UNITY_WEBGL
    [DllImport( "__Internal" )]
    private static extern bool setListenerTransform( 
        float posX, float posY, float posZ,
        float forwardX, float forwardY, float forwardZ,
        float upX, float upY, float upZ
    );

    void Start()
    {
        UpdateTransform();
    }


    void Update()
    {
        if( transform.hasChanged )
        {
            UpdateTransform();
        }
    }

    void UpdateTransform()
    {
        Vector3 pos = transform.position;
        Vector3 forward = transform.forward;
        Vector3 up = transform.up;
        setListenerTransform( 
            pos.x, pos.y, pos.z,
            forward.x, forward.y, forward.z,
            up.x, up.y, up.z
        );
    }
    #endif
}
