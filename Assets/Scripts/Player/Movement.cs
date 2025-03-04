using UnityEngine;


namespace Hydra.Player
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _jumpPower;

        private Rigidbody rb;

        void Start ()
        {
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate ()
        {
            float axisX = Input.GetAxisRaw("Horizontal");
            float axisY = Input.GetAxisRaw("Vertical");

            Vector2 direction = new Vector2(axisX, axisY).normalized * Time.deltaTime;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, transform.position.y, direction.y));
            print(" " +  targetRotation + " " + direction);
            rb.MovePosition(transform.position + new Vector3(direction.x * targetRotation.x, 0, direction.y * targetRotation.z) * _speed);
            RotatePlayer(direction);
        }

        void RotatePlayer(Vector2 dir)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(dir.x, transform.position.y, dir.y));

            float targetYRotation = targetRotation.eulerAngles.y;

            Quaternion finalRotation = Quaternion.Euler(0, targetYRotation, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, 5 * Time.deltaTime);
        }



    }
}