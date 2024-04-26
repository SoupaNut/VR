using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Game.Interaction;
using Unity.Game.Shared;
using Oculus.Voice;
using Meta.WitAi.Json;

namespace Unity.Game.Gameplay
{
    [RequireComponent(typeof(WeaponGrabInteractable))]
    public class SpellcastManager : MonoBehaviour
    {
        public MovementRecognizer MovementRecognizer;
        public AppVoiceExperience VoiceRecognizer;

        public Transform WandTip;

        [Range(0f, 1f)]
        public float MinimumRecognitionThreshold = 0.85f;

        CardData m_SpellToCast;
        WeaponGrabInteractable m_WeaponGrabInteractable;
        bool m_IsCasting = false;
        string m_SaidSpell;

        

        // Start is called before the first frame update
        void Start()
        {
            m_WeaponGrabInteractable = GetComponent<WeaponGrabInteractable>();
            DebugUtility.HandleErrorIfNullGetComponent<WeaponGrabInteractable, SpellcastManager>(m_WeaponGrabInteractable, this, gameObject);

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
            if (m_SpellToCast != null)
            {
                // compare against a given gesture
                MovementRecognizer.EndMovement(m_SpellToCast.School.ToString());
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

            
            bool voiceValid = true;
            // Voice Recognition Handling
            {
                if (m_SpellToCast.UseVoice)
                {
                    // Said spell doesn't match spell name
                    if (m_SaidSpell.ToLower() != m_SpellToCast.Name.ToLower())
                    {
                        voiceValid = false; // fail
                    }
                }
            }

            bool movementValid = true;
            // Movement Recognition Handling
            {
                if(m_SpellToCast.UseMovement)
                {
                    // calculate target score based on the spell accuracy
                    float targetScore = (1f - MinimumRecognitionThreshold) * (1f - (m_SpellToCast.Accuracy * 0.01f)) + MinimumRecognitionThreshold;

                    // Movement is not accurate enough
                    if (score < targetScore)
                    {
                        movementValid = false;
                    }
                }
            }

            if(voiceValid && movementValid)
            {
                BasicSpell spawnedSpell = Instantiate(m_SpellToCast.Animation);
                spawnedSpell.Initialize(WandTip);
            }

            // calculate target score based on the spell accuracy
            //float targetScore = (1f - MinimumRecognitionThreshold) * (1f - (m_SpellToCast.Accuracy * 0.01f)) + MinimumRecognitionThreshold;

            //if(score > targetScore && voiceSpellCorrect)
            //{
            //    BasicSpell spawnedSpell = Instantiate(m_SpellToCast.Animation);
            //    spawnedSpell.Initialize(WandTip);
            //}
        }

        public void SetSpellToCast(CardData spell)
        {
            if (!m_IsCasting)
            {
                //if(spell != null)
                //{
                //    MovementRecognizer.LineMaterial = spell.LineMaterial;
                //}

                MovementRecognizer.LineMaterial = spell ? spell.LineMaterial : null;

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