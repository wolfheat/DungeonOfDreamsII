using UnityEngine;
using UnityEngine.InputSystem;
using Wolfheat.StartMenu;

namespace Wolfheat.Inputs
{
    public class StartMenuInputs : MonoBehaviour
    {
        public Controls Controls { get; set; }
        public InputAction Actions { get; set; }

        public static StartMenuInputs Instance { get; private set; }
        // Start is called before the first frame update
        void Awake()
        {
            Debug.Log("Created Inputs Controller");
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Controls = new Controls();
            Controls.Enable();

        }
        private void OnDisable()
        {
            Controls.Disable();
        }
    }
}