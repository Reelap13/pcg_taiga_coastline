using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float _horizontal_movement_speed = 5f;
        [SerializeField] private float _speed_mulitiplier = 2f;
        [SerializeField] private float _vertical_movement_speed;
        [SerializeField] private float _rotation_speed = 2f;
        [SerializeField] private Vector2 _x_angle_boarders = new(-60, 60);

        private Rigidbody _rb;
        private Vector3 _rotation;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rotation = new();
        }

        private void Update()
        {
            Vector3 movement = GetMovement();
            _rb.MovePosition(transform.position + movement);

            GetRotation();
        }

        private Vector3 GetMovement()
        {
            float forward_movement = Input.GetAxisRaw("Vertical");
            float right_movement = Input.GetAxisRaw("Horizontal");

            Vector3 movement = transform.forward * forward_movement + transform.right * right_movement;
            movement = movement.normalized;
            movement *= _horizontal_movement_speed * (Input.GetKey(KeyCode.LeftShift) ? _speed_mulitiplier : 1);

            if (Input.GetKey(KeyCode.Space))
                movement += transform.up * _vertical_movement_speed;
            if (Input.GetKey(KeyCode.LeftControl))
                movement -= transform.up * _vertical_movement_speed;

            return movement * Time.deltaTime;
        }

        private void GetRotation()
        {
            float mouse_x = Input.GetAxis("Mouse X") * _rotation_speed * Time.deltaTime;
            float mouse_y = Input.GetAxis("Mouse Y") * _rotation_speed * Time.deltaTime;
            
            _rotation.y += mouse_x;
            _rotation.x -= mouse_y;

            _rotation.x = Mathf.Clamp(_rotation.x, _x_angle_boarders.x, _x_angle_boarders.y);

            transform.rotation = Quaternion.Euler(_rotation);
        }
    }
}