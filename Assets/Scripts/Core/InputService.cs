using System;
using UnityEngine;

namespace SlopJam.Core
{
    public class InputService : MonoBehaviour
    {
        public event Action<Vector2> OnMove;
        public event Action<Vector2> OnAim;
        public event Action<bool> OnShoot;

        public Vector2 Move { get; private set; }
        public Vector2 Aim { get; private set; }
        public bool IsShooting { get; private set; }

        [SerializeField] private Transform aimOrigin;

        public void SetAimOrigin(Transform origin)
        {
            aimOrigin = origin;
        }

        private void Update()
        {
            ReadMoveInput();
            ReadAimInput();
            ReadShootInput();
        }

        private void ReadMoveInput()
        {
            var move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if ((move - Move).sqrMagnitude > 0.0001f)
            {
                Move = Vector2.ClampMagnitude(move, 1f);
                OnMove?.Invoke(Move);
            }
        }

        private void ReadAimInput()
        {
            if (aimOrigin == null)
            {
                return;
            }

            var camera = Camera.main;
            if (camera == null)
            {
                return;
            }

            var mouse = Input.mousePosition;
            if (!camera.orthographic)
            {
                mouse.z = Mathf.Abs(camera.transform.position.z - aimOrigin.position.z);
            }

            var world = camera.ScreenToWorldPoint(mouse);
            var direction = world - aimOrigin.position;
            direction.z = 0f;

            var aim = new Vector2(direction.x, direction.y);
            if (aim.sqrMagnitude < 0.0001f)
            {
                return;
            }

            aim.Normalize();
            if ((aim - Aim).sqrMagnitude > 0.0001f)
            {
                Aim = aim;
                OnAim?.Invoke(Aim);
            }
        }

        private void ReadShootInput()
        {
            var shooting = Input.GetMouseButton(0);
            if (shooting != IsShooting)
            {
                IsShooting = shooting;
                OnShoot?.Invoke(IsShooting);
            }
        }
    }
}

