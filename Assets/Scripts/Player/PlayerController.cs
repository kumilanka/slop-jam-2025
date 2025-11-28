using SlopJam.Combat;
using SlopJam.Core;
using UnityEngine;

namespace SlopJam.Player
{
    [RequireComponent(typeof(PlayerRuntime))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float rotationSmoothTime = 0.05f;

        private PlayerRuntime runtime;
        private InputService inputService;
        private Vector3 aimDirection = Vector3.up;

        private void Awake()
        {
            runtime = GetComponent<PlayerRuntime>();
        }

        private void Start()
        {
            if (ServiceLocator.TryResolve(out InputService resolved))
            {
                inputService = resolved;
                inputService.SetAimOrigin(transform);
            }
        }

        private void Update()
        {
            if (inputService == null || runtime.Config == null)
            {
                return;
            }

            HandleMovement();
            HandleShooting();
        }

        private void HandleMovement()
        {
            var moveInput = inputService.Move;
            var movement = new Vector3(moveInput.x, moveInput.y, 0f);
            var displacement = movement * runtime.Config.moveSpeed * Time.deltaTime;
            transform.position += displacement;

            var aim = inputService.Aim;
            if (aim.sqrMagnitude > 0.0001f)
            {
                aimDirection = new Vector3(aim.x, aim.y, 0f).normalized;
                var targetRotation = Quaternion.LookRotation(Vector3.forward, aimDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSmoothTime * 360f * Time.deltaTime);
            }
        }

        private void HandleShooting()
        {
            if (!inputService.IsShooting || runtime.Weapon == null)
            {
                return;
            }

            var direction = aimDirection.sqrMagnitude > 0.0001f ? aimDirection : transform.up;
            runtime.Weapon.TryShoot(direction);
        }
    }
}

