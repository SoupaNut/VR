using Meta.WitAi.Json;
using Oculus.Voice;
using System.Collections.Generic;
using Unity.Game.Interaction;
using Unity.Game.Shared;
using UnityEngine;
using Unity.Game.Gameplay.Spells;

namespace Unity.Game.Gameplay
{
    [RequireComponent(typeof(WeaponGrabInteractable))]
    public class SpellcastManager : MonoBehaviour
    {
        public MovementRecognizer MovementRecognizer;
        public AppVoiceExperience VoiceRecognizer;

        public Transform WandTip;

        public Material DefaultLineMaterial;

        public List<Collider> IgnoredColliders { get; private set; }
        public GameObject Owner { get; private set; }


        DeckManager m_DeckManager;
        SpellData m_SpellToCast;
        WeaponGrabInteractable m_WeaponGrabInteractable;
        bool m_IsCasting = false;
        string m_SaidSpell;
        Collider[] m_SelfColliders;

        // Start is called before the first frame update
        void Start()
        {
            MovementRecognizer.LineMaterial = DefaultLineMaterial;

            m_WeaponGrabInteractable = GetComponent<WeaponGrabInteractable>();
            DebugUtility.HandleErrorIfNullGetComponent<WeaponGrabInteractable, SpellcastManager>(m_WeaponGrabInteractable, this, gameObject);

            // TODO: Change FindObjectOfType to GetComponent
            m_DeckManager = FindObjectOfType<DeckManager>();
            DebugUtility.HandleErrorIfNullFindObject<SpellcastManager, SpellcastManager>(this, m_DeckManager);

            m_SelfColliders = transform.GetComponentsInChildren<Collider>();    


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
                if(m_SpellToCast != null)
                {
                    bool isActivated = m_WeaponGrabInteractable.InteractorManager.IsActivated;

                    // Start Casting
                    if (!m_IsCasting && isActivated)
                    {
                        StartCasting();
                    }
                    // Update Casting
                    else if (m_IsCasting && isActivated)
                    {
                        UpdateCasting();
                    }

                    // End Casting
                    else if (m_IsCasting && !isActivated)
                    {
                        StopCasting();
                    }
                }
                // TODO: add indicator to user that they need to select a spell to start casting
                
            }
        }

        void WeaponEnabledHandler()
        {
            m_DeckManager.DeckEnabled = true;

            GameObject player = m_WeaponGrabInteractable.InteractorManager.Player.gameObject;
            Owner = player;

            IgnoredColliders = new List<Collider>();

            // Add self colliders
            IgnoredColliders.AddRange(m_SelfColliders);
            
            // Add player colliders
            Collider[] playerColliders = player.GetComponentsInChildren<Collider>();
            if (playerColliders.Length > 0)
            {
                IgnoredColliders.AddRange(playerColliders);
            }
        }

        void WeaponDisabledHandler()
        {
            m_DeckManager.ClearSelectedCard();
            m_DeckManager.DeckEnabled = false;
            m_DeckManager.DeckDisplay.Display.SetActive(false);
            IgnoredColliders = null;
            Owner = null;
        }

        public void StartCasting()
        {
            m_IsCasting = true;
            m_SaidSpell = "";

            if(m_SpellToCast.UseMovement)
                MovementRecognizer.StartMovement();
            
            if(m_SpellToCast.UseVoice)
                VoiceRecognizer.Activate();
            
        }

        public void UpdateCasting()
        {
            if(m_SpellToCast.UseMovement)
                MovementRecognizer.UpdateMovement();
        }

        public void StopCasting()
        {
            m_IsCasting = false;
            if(m_SpellToCast.UseVoice)
                VoiceRecognizer.Deactivate();

            if (m_SpellToCast != null && m_SpellToCast.UseMovement)
            {
                // compare against a given gesture
                MovementRecognizer.EndMovement(m_SpellToCast.MovementName.ToString());
            }
            else if (m_SpellToCast.UseMovement)
            {
                // Let movement recognizer figure out the movement
                MovementRecognizer.EndMovement();
            }
            else
            {
                CastSpell();
            }
        }

        public void CastSpell(string name="", float score=0f)
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
                if(name.ToLower() != m_SpellToCast.MovementName.ToLower())
                {
                    movementValid = false;
                }
            }

            if(voiceValid && movementValid)
            {
                BasicSpell spawnedSpell = Instantiate(m_SpellToCast.SpellPrefab);
                spawnedSpell.Cast(this);
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