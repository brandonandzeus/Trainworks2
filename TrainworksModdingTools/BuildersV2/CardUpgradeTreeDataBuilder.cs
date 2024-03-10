using System.Collections.Generic;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class CardUpgradeTreeDataBuilder
    {
        // UNUSED.
        // Note not useful and unused in the codebase. Default clans for some reason have this set to The Sentient;
        // Wurmkin doesn't set it at all.
        //public CharacterData Champion { get; set; }

        /// <summary>
        /// An already built UpgradeTree can be used here. If set overrides UpgradeTrees.
        /// </summary>
        public List<CardUpgradeTreeData.UpgradeTree> UpgradeTreesInternal { get; set; }

        /// <summary>
        /// This is a list of lists of CardUpgradeDataBuilders. Base game clans have a 3x3 list.
        /// Note that the way these Upgrades are applied.
        /// For instance If I have Primordium and I have Superfood II, only the upgrade corresponding to Superfood II is applied.
        /// That is when you select an upgraded path, the previous version is removed and the upgraded version is applied.
        /// If you mix paths then the upgrade corresponding to each split path is applied. That is if I am Superfood II and Aggressive Edible I
        /// Then the Superfood II upgrade is applied and then the Aggressive Edible I upgrade is applied.
        /// </summary>
        public List<List<CardUpgradeDataBuilder>> UpgradeTrees { get; set; } = new List<List<CardUpgradeDataBuilder>>();

        public CardUpgradeTreeData Build()
        {
            CardUpgradeTreeData cardUpgradeTreeData = ScriptableObject.CreateInstance<CardUpgradeTreeData>();

            if (UpgradeTreesInternal == null)
            {
                List<CardUpgradeTreeData.UpgradeTree> upgradeTrees = cardUpgradeTreeData.GetUpgradeTrees();

                foreach (List<CardUpgradeDataBuilder> branch in UpgradeTrees)
                {
                    CardUpgradeTreeData.UpgradeTree newBranch = new CardUpgradeTreeData.UpgradeTree();
                    List<CardUpgradeData> newbranchlist = newBranch.GetCardUpgrades();

                    foreach (CardUpgradeDataBuilder leaf in branch)
                    {
                        newbranchlist.Add(leaf.Build());
                    }

                    upgradeTrees.Add(newBranch);
                }
            }
            else
            {
                cardUpgradeTreeData.GetUpgradeTrees().AddRange(UpgradeTreesInternal);
            }

            return cardUpgradeTreeData;
        }
    }
}
