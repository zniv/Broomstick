using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace Athame.LeBourreau
{
    public class ATasteOfAfterlifeCardController : CardController
    {
        private CardController KilledHero = null;
        public ATasteOfAfterlifeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // When played kill another hero.
            List<DestroyCardAction>  storedResultsAction = new List<DestroyCardAction>();
            var coroutine = this.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria(c => c.IsHeroCharacterCard && c != this.CharacterCard), true, storedResultsAction);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            System.Console.WriteLine(">>>>>>>>>>>>>>>< storedResultsAction " + storedResultsAction);


            if (DidDestroyCard(storedResultsAction))
            {
                this.KilledHero = storedResultsAction.Find(a => a.CardToDestroy != null).CardToDestroy;
                System.Console.WriteLine(">>>>>>>>>>>>>>>< this.KilledHero " + this.KilledHero);
            }

            yield break;
        }


        public override void AddTriggers()
        {
            // When this card leaves play, flip that killed hero, set her hitpoints to half her maximum hitpoints (round up) and that hero's player draws 6 cards.
            AddTrigger<MoveCardAction>(a => this.KilledHero != null && a.CardToMove == this.Card && !a.Destination.IsInPlay, ResurrectResponse, TriggerType.FlipCard, TriggerTiming.After);

            // At the start of your turn, this card is removed from the game.
            AddStartOfTurnTrigger(tt => tt == this.HeroTurnTaker, RemoveFromGameResponse, TriggerType.RemoveFromGame);
        }

        private IEnumerator ResurrectResponse(MoveCardAction action)
        {
            IEnumerator coroutine = this.GameController.FlipCard(this.KilledHero);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var halfHP = this.KilledHero.Card.MaximumHitPoints.Value / 2 + this.KilledHero.Card.MaximumHitPoints.Value % 2;
            coroutine = this.GameController.SetHP(this.KilledHero.Card, halfHP, this.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = this.GameController.DrawCards(this.KilledHero.HeroTurnTakerController, 6);
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
