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

        [System.Serializable]
        public class SpellData
        {
            public BasicSpell Spell;
            public bool UseMovement;
            public string MovementName;
            public bool UseVoice;
            public string VoiceName;
        }

        public List<SpellData> Spells;

        WeaponGrabInteractable m_WeaponGrabInteractable;
        bool m_IsCasting = false;
        string SaidSpell;

        

        // Start is called before the first frame update
        void Start()
        {
            m_WeaponGrabInteractable = GetComponent<WeaponGrabInteractable>();
            DebugUtility.HandleErrorIfNullGetComponent<WeaponGrabInteractable, MovementRecognizer>(m_WeaponGrabInteractable, this, gameObject);

            VoiceRecognizer.VoiceEvents.OnPartialResponse.AddListener(SetSaidSpell);
            VoiceRecognizer.VoiceEvents.OnFullTranscription.AddListener(CastSpell);
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
            SaidSpell = "";
        }

        public void UpdateCasting()
        {
            MovementRecognizer.UpdateMovement();
        }

        public void StopCasting()
        {
            m_IsCasting = false;
            //CastSpell(SaidSpell);
            MovementRecognizer.EndMovement();
            VoiceRecognizer.Deactivate();
        }

        public void CastSpell(string movementName)
        {
            SpellData spellToCast;

            //if(movementName != null && movementName != "")
            //{
            //    int spellToCastIndex = Spells.FindIndex(x => x.MovementName == movementName && x.UseMovement);

            //    // Found a matching spell
            //    if (spellToCastIndex >= 0)
            //    {
            //        spellToCast = Spells[spellToCastIndex];
            //    }
            //    else
            //    {
            //        spellToCast = Spells[0];
            //    }
            //}
            if(SaidSpell != null && SaidSpell != "")
            {
                int spellToCastIndex = Spells.FindIndex(x => x.VoiceName == SaidSpell && x.UseVoice);
                //int spellToCastIndex = Spells.FindIndex(x => SaidSpell.Contains(x.VoiceName) && x.UseVoice);

                // Found a matching spell
                if (spellToCastIndex >= 0)
                {
                    spellToCast = Spells[spellToCastIndex];
                }
                else
                {
                    spellToCast = Spells[0];
                }
            }
            else
            {
                spellToCast = Spells[0];
            }

            

            BasicSpell spawnedSpell = Instantiate(spellToCast.Spell);
            spawnedSpell.Initialize(WandTip);
        }

        //void SetSaidSpell(string text)
        //{
        //    SaidSpell = text;
        //}

        void SetSaidSpell(WitResponseNode response)
        {
            string intentName = response["intents"][0]["name"].Value.ToString();
            SaidSpell = intentName;
        }

    }
}