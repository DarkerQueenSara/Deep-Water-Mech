using System;
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

        public void AddToInventory(MechPart part)
        {
            switch (part)
            {
                case Head head:
                    heads.Add(head);
                    break;
                case Torso torso:
                    torsos.Add(torso);
                    break;
                case Arm arm:
                    arms.Add(arm);
                    break;
                case Legs leg:
                    legs.Add(leg);
                    break;
                case BonusPart bonus:
                    bonusParts.Add(bonus);
                    break;
            }
        }

        public bool Contains(MechPart part)
        {
            return heads.Contains(part) || arms.Contains(part) || torsos.Contains(part) || legs.Contains(part) ||
                   bonusParts.Contains(part);
        }

        public List<string> GetListHeads()
        {
            List<string> toReturn = new List<string> { equippedHead.name.ToUpper() + " (EQUIPPED)"};
            foreach (Head head in heads.Where(head => !toReturn.Contains(head.name.ToUpper()) && !toReturn.Contains(head.name.ToUpper() + " (EQUIPPED)")))
            {
                toReturn.Add(head.name.ToUpper());
            }
            return toReturn;
        }
        
        public List<string> GetListTorsos()
        {
            List<string> toReturn = new List<string> { equippedTorso.name.ToUpper() + " (EQUIPPED)"};
            foreach (Torso torso in torsos.Where(torso => !toReturn.Contains(torso.name.ToUpper()) && !toReturn.Contains(torso.name.ToUpper() + " (EQUIPPED)")))
            {
                toReturn.Add(torso.name.ToUpper());
            }
            return toReturn;
        }
        
        public List<string> GetListLeftArms()
        {
            List<string> toReturn = new List<string> { equippedLeftArm.name.ToUpper() + " (EQUIPPED)"};
            foreach (Arm arm in arms.Where(arm => !toReturn.Contains(arm.name.ToUpper()) && !toReturn.Contains(arm.name.ToUpper() + " (EQUIPPED)") && arm != equippedRightArm))
            {
                toReturn.Add(arm.name.ToUpper());
            }
            return toReturn;
        }
        
        public List<string> GetListRightArms()
        {
            List<string> toReturn = new List<string> { equippedRightArm.name.ToUpper() + " (EQUIPPED)"};
            foreach (Arm arm in arms.Where(arm => !toReturn.Contains(arm.name.ToUpper()) && !toReturn.Contains(arm.name.ToUpper() + " (EQUIPPED)") && arm != equippedLeftArm))
            {
                toReturn.Add(arm.name.ToUpper());
            }
            return toReturn;
        }
        
        public List<string> GetListLegs()
        {
            List<string> toReturn = new List<string> { equippedLegs.name.ToUpper() + " (EQUIPPED)"};
            foreach (Legs leg in legs.Where(leg => !toReturn.Contains(leg.name.ToUpper()) && !toReturn.Contains(leg.name.ToUpper() + " (EQUIPPED)") ))
            {
                toReturn.Add(leg.name.ToUpper());
            }
            return toReturn;
        }
        
        public List<string> GetListBonusParts()
        {
            List<string> toReturn = new List<string> ();
            
            if (equippedBonusPart != null) toReturn.Add(equippedBonusPart.name.ToUpper() + " (EQUIPPED)");
            
            toReturn.Add("NO EQUIPMENT");
            
            foreach (BonusPart part in bonusParts.Where(part => !toReturn.Contains(part.name.ToUpper()) && !toReturn.Contains(part.name.ToUpper() + " (EQUIPPED)") ))
            {
                toReturn.Add(part.name.ToUpper());
            }
            return toReturn;
        }
        
    }
}