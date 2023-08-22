using System.Collections.Generic;
using System.Linq;
using _Scripts.MechaParts;
using UnityEngine;

namespace _Scripts
{
    [CreateAssetMenu(menuName = "Inventory", order = 0)]
    public class Inventory : ScriptableObject
    {
        [Header("Current Inventory")]
        public List<Arm> arms;
        public List<BonusPart> bonusParts;
        public List<Legs> legs;
        public List<Torso> torsos;

        [Header("Default Inventory")] 
        public Arm defaultLeftArm;
        public Arm defaultRightArm;
        public BonusPart defaultBonusPart;
        public Legs defaultLegs;
        public Torso defaultTorso;

        public void InitiateInventory()
        {
            arms.Clear();
            arms.Add(defaultLeftArm);
            arms.Add(defaultRightArm);
            bonusParts.Clear();
            if (defaultBonusPart != null) bonusParts.Add(defaultBonusPart);
            legs.Clear();
            legs.Add(defaultLegs);
            torsos.Clear();
            torsos.Add(defaultTorso);
        }
    }
}