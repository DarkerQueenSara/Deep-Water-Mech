using System.Collections.Generic;
using System.Linq;
using _Scripts.MechaParts;
using _Scripts.MechaParts.SO;
using UnityEngine;

namespace _Scripts
{
    [CreateAssetMenu(menuName = "Inventory", order = 0)]
    public class Inventory : ScriptableObject
    {
        [Header("Current Inventory")] 
        [HideInInspector] public List<Head> heads;
        [HideInInspector] public List<Arm> arms;
        [HideInInspector] public List<BonusPart> bonusParts;
        [HideInInspector] public List<Legs> legs;
        [HideInInspector] public List<Torso> torsos;

        [Header("Equipped Inventory")] 
        [HideInInspector] public Head equippedHead;
        [HideInInspector] public Arm equippedLeftArm;
        [HideInInspector] public Arm equippedRightArm;
        [HideInInspector] public BonusPart equippedBonusPart;
        [HideInInspector] public Legs equippedLegs;
        [HideInInspector] public Torso equippedTorso;

        [Header("Default Inventory")] 
        public Head defaultHead;
        public Arm defaultLeftArm;
        public Arm defaultRightArm;
        public BonusPart defaultBonusPart;
        public Legs defaultLegs;
        public Torso defaultTorso;

        public void InitiateInventory()
        {
            heads.Clear();
            heads.Add(defaultHead);
            equippedHead = defaultHead;
            arms.Clear();
            arms.Add(defaultLeftArm);
            arms.Add(defaultRightArm);
            equippedLeftArm = defaultLeftArm;
            equippedRightArm = defaultRightArm;
            bonusParts.Clear();
            if (defaultBonusPart != null)
            {
                bonusParts.Add(defaultBonusPart);
                equippedBonusPart = defaultBonusPart;
            }
            legs.Clear();
            legs.Add(defaultLegs);
            equippedLegs = defaultLegs;
            torsos.Clear();
            torsos.Add(defaultTorso);
            equippedTorso = defaultTorso;
        }
    }
}