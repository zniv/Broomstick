using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Athame.LeBourreau
{
    public class GallowsCardController : CardController
    {
        public GallowsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // When played move a non-character non-hero non-device non-component target next to it
            var coroutine = this.GameController.SelectAndMoveCard(this.DecisionMaker, c => c.IsInPlay && !c.IsHero && c.IsTarget && !c.IsCharacter && !c.IsComponent && !c.IsDevice, this.Card.NextToLocation, optional: true);
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


        public override void AddTriggers()
        {
            // At the start of your turn, this card deals X melee damage to the target next to it where X is the half of that target's current HP (round up).
            AddStartOfTurnTrigger(tt => tt == this.HeroTurnTaker, HangResponse, TriggerType.DealDamage);
        }

        private IEnumerator HangResponse(PhaseChangeAction action)
        {
            Card target = this.Card.NextToLocation.BottomCard;
            int damage = target.HitPoints.Value / 2 + target.HitPoints.Value % 2;
            IEnumerator coroutine = this.DealDamage(this.Card, target, damage, DamageType.Melee);
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

        public override bool AskIfActionCanBePerformed(GameAction gameAction)
        {
            // Card next to this one has no text.
            if (gameAction.CardSource != null && this.Card.NextToLocation != null && gameAction.CardSource.Card == this.Card.NextToLocation.BottomCard)
            {
                return false;
            }

            return base.AskIfActionCanBePerformed(gameAction);
        }
    }
}
