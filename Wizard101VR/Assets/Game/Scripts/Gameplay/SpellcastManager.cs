using Meta.WitAi.Json;
using Oculus.Voice;
using Unity.Game.Interaction;
using Unity.Game.Shared;
using UnityEngine;

namespace Unity.Game.Gameplay
{
    [RequireComponent(typeof(WeaponGrabInteractable))]
    public class SpellcastManager : MonoBehaviour
    {
        public MovementRecognizer MovementRecognizer;
        public AppVoiceExperience VoiceRecognizer;

        public Transform WandTip;

        //[Range(0f, 1f)]
        //public float MinimumRecognitionThreshold = 0.85f;

        public Material DefaultLineMaterial;


        DeckManager m_DeckManager;
        SpellData m_SpellToCast;
        WeaponGrabInteractable m_WeaponGrabInteractable;
        bool m_IsCasting = false;
        string m_SaidSpell;

        // Start is called before the first frame update
        void Start()
        {
            MovementRecognizer.LineMaterial = DefaultLineMaterial;

            m_WeaponGrabInteractable = GetComponent<WeaponGrabInteractable>();
            DebugUtility.HandleErrorIfNullGetComponent<WeaponGrabInteractable, SpellcastManager>(m_WeaponGrabInteractable, this, gameObject);

            // TODO: Change FindObjectOfType to GetComponent
            m_DeckManager = FindObjectOfType<DeckManager>();
            DebugUtility.HandleErrorIfNullFindObject<SpellcastManager, SpellcastManager>(this, m_DeckManager);


            m_WeaponGrabInteractable.onWeaponEnable += WeaponEnabledHandler;
            m_WeaponGrabInteractable.onWeaponDisable += WeaponDisabledHandler;
            VoiceRecognizer.VoiceEvents.OnPartialResponse.AddListener(SetSaidSpell);
        }

        // Update is called once per frame
        void Update()
        {
            // If wand is being grabbed
            if(m_WeaponGrabInteractable.IsWeaponEnabled)
            {
                bool isActivated = m_WeaponGrabInteractable.InteractorManager.IsActivated;

                // Start Casting
                if(!m_IsCasting && isActivated)
                {
                    StartCasting();
                }
                // Update Casting
                else if(m_IsCasting && isActivated)
                {
                    UpdateCasting();
                }

                // End Casting
                else if(m_IsCasting && !isActivated)
                {
                    StopCasting();
                }
            }
        }

        void WeaponEnabledHandler()
        {
            m_DeckManager.DeckEnabled = true;
        }

        void WeaponDisabledHandler()
        {
            m_DeckManager.ClearSelectedCard();
            m_DeckManager.DeckEnabled = false;
            m_DeckManager.DeckDisplay.Display.SetActive(false);
        }

        public void StartCasting()
        {
            m_IsCasting = true;
            MovementRecognizer.StartMovement();
            VoiceRecognizer.Activate();
            m_SaidSpell = "";
        }

        public void UpdateCasting()
        {
            MovementRecognizer.UpdateMovement();
        }

        public void StopCasting()
        {
            m_IsCasting = false;
            if (m_SpellToCast != null && m_SpellToCast.UseMovement)
            {
                // compare against a given gesture
                MovementRecognizer.EndMovement(m_SpellToCast.MovementName.ToString());
            }
            else
            {
                // Let movement recognizer figure out the movement
                MovementRecognizer.EndMovement();
            }

            VoiceRecognizer.Deactivate();
        }

        public void CastSpell(string name, float score)
        {
            // Check if we have a spell to cast
            if (m_SpellToCast == null)
                return;

            // Voice Recognition Handling
            bool voiceValid = true;
            if (m_SpellToCast.UseVoice)
            {
                // Said spell doesn't match spell name
                if (m_SaidSpell.ToLower() != m_SpellToCast.VoiceName.ToLower())
                {
                    voiceValid = false; // fail
                }
            }

            // Movement Recognition Handling
            bool movementValid = true;
            if (m_SpellToCast.UseMovement)
            {
                //// calculate target score based on the spell accuracy
                //float targetScore = (1f - MinimumRecognitionThreshold) * (1f - (m_SpellToCast.Accuracy * 0.01f)) + MinimumRecognitionThreshold;

                //// Movement is not accurate enough
                //if (score < targetScore)
                //{
                //    movementValid = false;
                //}
                if(name.ToLower() != m_SpellToCast.MovementName.ToLower())
                {
                    movementValid = false;
                }
            }

            if(voiceValid && movementValid)
            {
                BasicSpell spawnedSpell = Instantiate(m_SpellToCast.SpellPrefab);
                spawnedSpell.Initialize(WandTip);
            }
        }

        public void SetSpellToCast(SpellData spell)
        {
            if (!m_IsCasting)
            {
                MovementRecognizer.LineMaterial = (spell && spell.UseMovement) ? spell.LineMaterial : DefaultLineMaterial;
                m_SpellToCast = spell;
            }
        }

        void SetSaidSpell(WitResponseNode response)
        {
            string intentName = response["intents"][0]["name"].Value.ToString();
            m_SaidSpell = intentName;
        }

    }
}