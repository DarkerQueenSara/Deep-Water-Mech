using System.Collections.Generic;
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
        public List<Head> heads;
        public List<Legs> legs;
        public List<Torso> torsos;

        [Header("Default Inventory")] 
        public Arm defaultLeftArm;
        public Arm defaultRightArm;
        public BonusPart defaultBonusPart;
        public Head defaultHead;
        public Legs defaultLegs;
        public Torso defaultTorso;

        public void InitiateInventory()
        {
            arms.Clear();
            arms.Add(defaultLeftArm);
            arms.Add(defaultRightArm);
            bonusParts.Clear();
            if (defaultBonusPart != null) bonusParts.Add(defaultBonusPart);
            heads.Clear();
            heads.Add(defaultHead);
            legs.Clear();
            legs.Add(defaultLegs);
            torsos.Clear();
            torsos.Add(defaultTorso);
        }
    }
}