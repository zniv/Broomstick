using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Collections.Generic;

namespace BroomstickTest
{
    [TestFixture()]
    public class Test : BaseTest
    {
        [Test()]
        public void TestModWorks()
        {

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(env);


            StartGame();

            AssertIsInPlay("SmashBackField");

            // Always deals 5 psychic to non villains!
            PlayCard("FireEverything");

            QuickHPCheck(0, -6); // Nemesis!

            PlayTopCard(env);

            // Deals 1 damage
            QuickHPCheck(-1, -1);

            // Heals 1 at the start of the environment turn
            GoToStartOfTurn(env);
            QuickHPCheck(1, 1);
        }


        [Test()]
        public void TestBunkerVariant()
        {
            SetupGameController("BaronBlade", "Bunker/Broomstick.WaywardBunkerCharacter", "Megalopolis");

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
    }
}
