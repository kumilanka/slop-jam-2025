using SlopJam.Combat;
using SlopJam.Core;
using UnityEngine;

namespace SlopJam.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerRuntime))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float rotationSmoothTime = 0.05f;

        private CharacterController characterController;
        private PlayerRuntime runtime;
        private InputService inputService;
        private Vector3 rotationVelocity;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
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
            var movement = new Vector3(moveInput.x, 0f, moveInput.y);
            characterController.SimpleMove(movement * runtime.Config.moveSpeed);

            var aim = inputService.Aim;
            if (aim.sqrMagnitude > 0.01f)
            {
                var targetRotation = Mathf.Atan2(aim.x, aim.y) * Mathf.Rad2Deg;
                var yRot = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity.y, rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0f, yRot, 0f);
            }
        }

        private void HandleShooting()
        {
            if (inputService.IsShooting && runtime.Weapon != null)
            {
                runtime.Weapon.TryShoot(transform.forward);
            }
        }
    }
}

