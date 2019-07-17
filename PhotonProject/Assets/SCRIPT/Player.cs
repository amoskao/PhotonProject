using UnityEngine;

namespace amos
{
    public class Player : MonoBehaviour
    {
        public Rigidbody2D rig;
        public float speed = 10;
        private void FixedUpdate()
        {
            Move();
        }
        private void Move()
        {
            rig.AddForce((
                transform.right * Input.GetAxisRaw("Horizontal") + 
                transform.up * Input.GetAxisRaw("Vertical"))*speed);
        }
    }
}
