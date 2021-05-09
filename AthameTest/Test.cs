using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;

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

        [Test()]
        public void TestLeBourreau()
        {
            SetupGameController("BaronBlade", "Athame.LeBourreau", "Megalopolis", "Legacy");

            StartGame();

            var lebourreau = this.FindHero("LeBourreau");

            // test character
            Assert.IsTrue(lebourreau.CharacterCard.IsHero);
            Assert.AreEqual("LeBourreauCharacter", lebourreau.CharacterCard.Identifier);
            Assert.AreEqual(33, lebourreau.CharacterCard.MaximumHitPoints);

            // test power
            var card = FindCard(c => c.IsTarget && !c.IsCharacter);
            PlayCard(card, true);
            Assert.IsTrue(card.IsInPlay);
            SetHitPoints(card, 2);
            GoToUsePowerPhase(lebourreau);
            UsePower(lebourreau);
            Assert.IsTrue(card.IsInTrash);

            // test Gallows card
            var target = FindCard(c => c.IsTarget && c.IsVillain && !c.IsCharacter && !c.IsDevice && !c.IsComponent);
            PlayCard(target, true);
            Assert.NotNull(target);
            var hp = target.HitPoints.Value;
            GoToPlayCardPhase(lebourreau);
            var gallows = FindCard(c => c.Identifier == "Gallows");
            Assert.NotNull(gallows);
            PlayCard(gallows);
            GoToPlayCardPhase(lebourreau);
            Assert.AreEqual(hp / 2, target.HitPoints.Value);

            // test LifeTakerBurden card
            var lifeTakerBurden = PutIntoPlay("LifeTakerBurden");
            var pool = lifeTakerBurden.FindTokenPool(Athame.LeBourreau.LifeTakerBurdenCardController.PoolIdentifier);
            AssertTokenPoolCount(pool, 0);
            var bladeBattalion = PutIntoPlay("BladeBattalion");
            DestroyCard(bladeBattalion, lebourreau.CharacterCard);
            AssertTokenPoolCount(pool, 1);
            DealDamage(lebourreau, c => c == lebourreau.CharacterCard, 5, Handelabra.Sentinels.Engine.Model.DamageType.Melee);
            AssertTokenPoolCount(pool, 1);
            bladeBattalion = PutIntoPlay("BladeBattalion");
            DestroyCard(bladeBattalion, lebourreau.CharacterCard);
            AssertTokenPoolCount(pool, 2);
            bladeBattalion = PutIntoPlay("BladeBattalion");
            DestroyCard(bladeBattalion, lebourreau.CharacterCard);
            AssertTokenPoolCount(pool, 3);
            bladeBattalion = PutIntoPlay("BladeBattalion");
            DestroyCard(bladeBattalion, lebourreau.CharacterCard);
            AssertTokenPoolCount(pool, 4);
            hp = lebourreau.CharacterCard.HitPoints.Value;
            DecisionsYesNo = new bool[] { true };
            DealDamage(lebourreau, c => c == lebourreau.CharacterCard, 5, Handelabra.Sentinels.Engine.Model.DamageType.Melee);
            Assert.AreEqual(hp, lebourreau.CharacterCard.HitPoints.Value);
            AssertTokenPoolCount(pool, 0);

            // test ATasteOfAfterlife card
            var legacy = FindHero("Legacy");
            Assert.NotNull(legacy);
            Assert.IsFalse(legacy.IsIncapacitated);
            DecisionSelectCard = legacy.CharacterCard;
            var aTasteOfAfterlife = PutIntoPlay("ATasteOfAfterlife");
            Assert.IsTrue(legacy.IsIncapacitated);
            GoToPlayCardPhase(lebourreau);
            Assert.IsFalse(legacy.IsIncapacitated);
        }
    }
}
