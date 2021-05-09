using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace Athame.LeBourreau
{
    public class LifeTakerBurdenCardController : CardController
    {
        public static readonly string PoolIdentifier = "LifeTakerBurdenPool";
        private ITrigger ReduceDamageTrigger;

        public LifeTakerBurdenCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowTokenPool(this.Card.FindTokenPool(PoolIdentifier));
        }

        public override void AddTriggers()
        {
            // Whenever a {LeBourreau}'s card destroys a target then {LeBourreau} deals himself 1 irrectuctible psychic damage.
            // For each damage taken this way add a token to this card.
            AddTrigger<DestroyCardAction>(a => a.CardSource != null && a.CardSource.CardController.CharacterCard == this.CharacterCard && a.CardToDestroy != null && a.CardToDestroy.Card.IsTarget, DamageAndTokenResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.AddTokensToPool }, TriggerTiming.After);

            // Whenever {LeBourreau} would be dealt damage, you may remove 4 tokens to prevent it.
            this.ReduceDamageTrigger = AddTrigger<DealDamageAction>(a => a.Amount > 0 && a.Target == this.CharacterCard && this.Card.FindTokenPool(PoolIdentifier).CurrentValue >= 4, PreventDamageResponse, TriggerType.ReduceDamage, TriggerTiming.Before);
        }

        private IEnumerator DamageAndTokenResponse(DestroyCardAction action)
        {
            IEnumerator coroutine = this.GameController.DealDamageToSelf(this.DecisionMaker, c => c == this.CharacterCard, 1, DamageType.Psychic, isIrreducible: true, cardSource: GetCardSource(), addStatusEffect: a => { this.Card.FindTokenPool(PoolIdentifier).AddTokens(a.Amount); return DoNothing(); });
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

        private IEnumerator PreventDamageResponse(DealDamageAction action)
        {
            List<RemoveTokensFromPoolAction> storedResults = new List<RemoveTokensFromPoolAction>();
            IEnumerator coroutine = this.GameController.RemoveTokensFromPool(this.Card.FindTokenPool(PoolIdentifier), 4, storedResults, true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidRemoveTokens(storedResults))
            {
                coroutine = this.GameController.ReduceDamage(action, action.Amount, this.ReduceDamageTrigger, GetCardSource());
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
    }
}
