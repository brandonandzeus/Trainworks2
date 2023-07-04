using System.Collections.Generic;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class CardUpgradeTreeDataBuilder
    {
        /// <summary>
        /// Character Data for a champion.
        /// </summary>
        public CharacterData Champion { get; set; }

        /// <summary>
        /// An already built UpgradeTree can be used here. If set overrides UpgradeTrees.
        /// </summary>
        public List<CardUpgradeTreeData.UpgradeTree> UpgradeTreesInternal { get; set; }

        /// <summary>
        /// This is a list of lists of CardUpgradeDataBuilders. Base game clans have a 3x3 list.
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
