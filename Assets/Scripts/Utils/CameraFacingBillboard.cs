using UnityEngine;

namespace DudeResqueSquad
{
    public class CameraFacingBillboard : MonoBehaviour
    {
        private Transform _camera = null;

        private void Start()
        {
            _camera = GameManager.Instance.MinimapCamera.transform;
        }

        //Orient the camera after all movement is completed this frame to avoid jittering
        private void LateUpdate()
        {
            if (_camera == null)
                return;

            transform.LookAt(transform.position + _camera.rotation * Vector3.forward, _camera.rotation * Vector3.up);
        }
    }
}