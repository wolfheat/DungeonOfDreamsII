using UnityEngine;
using UnityEngine.InputSystem;
using Wolfheat.StartMenu;

namespace Wolfheat.Inputs
{
    public class Inputs : MonoBehaviour
    {
        public Controls Controls { get; set; }
        public InputAction Actions { get; set; }

        public static Inputs Instance { get; private set; }

        private void OnEnable()
        {
            // Enable this when you want to use the loading of a saved file
            //SavingUtility.LoadingComplete += LoadingComplete;
            Controls.Player.T.performed += JumpToNextEntryPoint;
        }
        private void Start()
        {
            LoadingComplete();
            
        }

        private void LoadingComplete()
        {
            Debug.Log("Loading Complete");
            Controls.Player.M.performed += SoundMaster.Instance.ToggleAllAudio;
            Controls.Player.N.performed += SoundMaster.Instance.ToggleMusic;
        }

        private void OnDisable()
        {
            Controls.Player.M.performed -= SoundMaster.Instance.ToggleAllAudio;
            Controls.Player.N.performed -= SoundMaster.Instance.ToggleMusic;
            Controls.Player.T.performed -= SoundMaster.Instance.ToggleMusic;
        }

        public void JumpToNextEntryPoint(InputAction.CallbackContext context)
        {
            Debug.Log("Jumping to Next Exit point");
            PlayerController.Instance.GotoNextStartPosition();
        }

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
    }
}