using UnityEngine;

namespace RPG.Core
{
    public class CameraRotate : MonoBehaviour
    {
        [SerializeField] private float speed = 3.5f;
        private float X;
        private float Y;

        void Update()
        {
            if (Input.GetMouseButton(1))
            {
                transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * speed, -Input.GetAxis("Mouse X") * speed, 0));
                X = transform.rotation.eulerAngles.x;
                
                if(X < 0 || X > 90)
                {
                    X = 0;
                }
                Y = transform.rotation.eulerAngles.y;
                transform.rotation = Quaternion.Euler(X, Y, 0);
            }
        }
    }
}