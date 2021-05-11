using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace Athame.LeBourreau
{
    public class ATasteOfAfterlifeCardController : CardController
    {
        //private CardController KilledHero = null;
        private Card KilledHeroCard = null;
        private int KilledHeroMaxHP = -1;
        public ATasteOfAfterlifeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // When played kill another hero.
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            var coroutine = this.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.CharacterCard, new LinqCardCriteria(c => c.IsHeroCharacterCard && c != this.CharacterCard), storedResults, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidSelectCard(storedResults))
            {
                var selectDecision = storedResults.Find(a => a.SelectedCard != null);
                this.KilledHeroCard = selectDecision?.SelectedCard;
                System.Console.WriteLine(">>>>>>>>>>>>>>>>>>> this.KilledHeroCard " + this.KilledHeroCard);
                this.KilledHeroMaxHP = this.KilledHeroCard.MaximumHitPoints.Value;
                coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.KilledHeroCard);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }


        public override void AddTriggers()
        {
            // When this card leaves play, flip that killed hero, set her hitpoints to half her maximum hitpoints (round up) and that hero's player draws 6 cards.
            //AddTrigger<MoveCardAction>(a => this.KilledHero != null && a.CardToMove == this.Card && !a.Destination.IsInPlay, ResurrectResponse, TriggerType.FlipCard, TriggerTiming.Before);
            AddTrigger<MoveCardAction>(a => this.KilledHeroCard != null && a.CardToMove == this.Card && !a.Destination.IsInPlay, ResurrectResponse, TriggerType.FlipCard, TriggerTiming.Before);

            // At the start of your turn, this card is removed from the game.
            AddStartOfTurnTrigger(tt => tt == this.HeroTurnTaker && this.Card.IsInPlay, RemoveFromGameResponse, TriggerType.RemoveFromGame);
        }

        private IEnumerator ResurrectResponse(MoveCardAction action)
        {
            var killedHero = this.FindCardController(this.KilledHeroCard);
            IEnumerator coroutine = this.GameController.FlipCard(killedHero,true,true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var halfHP = this.KilledHeroMaxHP/ 2 + this.KilledHeroMaxHP % 2;
            System.Console.WriteLine(">>>>>>>>>>>>>>>>>>> this.KilledHeroCard 2 " + this.KilledHeroCard);
            this.KilledHeroCard.SetMaximumHP(this.KilledHeroMaxHP,false);
            coroutine = this.GameController.SetHP(this.KilledHeroCard, halfHP, this.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = this.GameController.DrawCards(killedHero.HeroTurnTakerController, 6);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }

        private IEnumerator RemoveFromGameResponse(GameAction action)
        {
            IEnumerator coroutine = this.GameController.MoveCard(this.DecisionMaker, this.Card, TurnTaker.OutOfGame, cardSource: this.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
