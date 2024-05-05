using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Unity.Game.Interaction
{
    public class UniqueSocketInteractor : XRSocketInteractor
    {
        [Tooltip("List of game objects that the socket allow to be snapped to.")]
        public List<GameObject> UniqueObjects;

        List<int> uniqueIds = new List<int>();

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            foreach (GameObject obj in UniqueObjects)
            {
                uniqueIds.Add(obj.GetInstanceID());
            }
        }

        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            // Make sure we have a list of unique ids
            if (uniqueIds.Count == 0)
                return base.CanSelect(interactable);

            // return true if we can select the interactable and it contains the correct id
            int id = interactable.transform.gameObject.GetInstanceID();
            return base.CanSelect(interactable) && uniqueIds.Contains(id);
        }
    }
}

