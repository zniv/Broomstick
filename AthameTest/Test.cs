using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Collections.Generic;

namespace AthameTest
{
    [TestFixture()]
    public class Test : BaseTest
    {

        [Test()]
        public void TestBunkerVariant()
        {
            SetupGameController("BaronBlade", "Bunker/Athame.WaywardBunkerCharacter", "Megalopolis");

            StartGame();

            Assert.IsTrue(bunker.CharacterCard.IsPromoCard);
            Assert.AreEqual("WaywardBunkerCharacter", bunker.CharacterCard.PromoIdentifierOrIdentifier);
            Assert.AreEqual(30, bunker.CharacterCard.MaximumHitPoints);

            GoToUsePowerPhase(bunker);

            // Use the power, it draws 2 cards not 1!
            QuickHandStorage(bunker);
            UsePower(bunker);
            QuickHandCheck(2);
        }

        [Test()]
        public void TestExpatrietteVariant()
        {
            SetupGameController("BaronBlade", "Expatriette/Athame.LoadedExpatrietteCharacter", "Megalopolis");

            StartGame();

            Assert.IsTrue(expatriette.CharacterCard.IsPromoCard);
            Assert.AreEqual("LoadedExpatrietteCharacter", expatriette.CharacterCard.PromoIdentifierOrIdentifier);
            Assert.AreEqual(28, expatriette.CharacterCard.MaximumHitPoints);

            Assert.AreEqual(null, expatriette.CharacterCardController.GetCardPropertyJournalEntryInteger("ExpatExtraLoadedTotalPowerCount"));

            GoToUsePowerPhase(expatriette);

            QuickHandStorage(expatriette);
            UsePower(expatriette);
            QuickHandCheck(-1);

            Assert.AreEqual(1, expatriette.CharacterCardController.GetCardPropertyJournalEntryInteger("ExpatExtraLoadedTotalPowerCount"));
        }
    }
}
