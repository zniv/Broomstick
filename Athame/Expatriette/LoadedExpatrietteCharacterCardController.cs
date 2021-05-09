using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace Athame.Expatriette
{
    public class LoadedExpatrietteCharacterCardController : HeroCharacterCardController
    {
        public static readonly string TotalPowerCount = "ExpatExtraLoadedTotalPowerCount";
        public static readonly string ThisTurnPowerCount = "ExpatExtraLoadedThisTurnPowerCount";

        public LoadedExpatrietteCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SetCardProperty(TotalPowerCount, 0);
            SetCardProperty(ThisTurnPowerCount, 0);
            this.SpecialStringMaker.ShowSpecialString(() => $"{this.Card.Title} is reloading ! x{this.GetTotalPowerCount()}", () => true).Condition = () => this.GetTotalPowerCount() > 0;
        }

        private int GetTotalPowerCount()
        {
            var count = GetCardPropertyJournalEntryInteger(TotalPowerCount);
            return count.HasValue ? count.Value : 0;
        }

        private void IncrementTotalPowerCount()
        {
            SetCardProperty(TotalPowerCount, this.GetTotalPowerCount() + 1);
        }

        private void DecrementTotalPowerCount()
        {
            SetCardProperty(TotalPowerCount, this.GetTotalPowerCount() - 1);
        }

        private int GetThisTurnPowerCount()
        {
            var count = GetCardPropertyJournalEntryInteger(ThisTurnPowerCount);
            return count.HasValue ? count.Value : 0;
        }

        private void IncrementThisTurnPowerCount()
        {
            SetCardProperty(ThisTurnPowerCount, this.GetThisTurnPowerCount() + 1);
        }

        private void ResetThisTurnPowerCount()
        {
            SetCardProperty(ThisTurnPowerCount, 0);
        }

        public override void AddTriggers()
        {
            AddTrigger<PlayCardAction>(action => action.Origin.OwnerTurnTaker == HeroTurnTakerController.TurnTaker && this.GetTotalPowerCount() > 0 && action.CardToPlay.DoKeywordsContain("gun"), PlayAmmoResponse, TriggerType.PlayCard, TriggerTiming.After);
            AddTrigger<PlayCardAction>(action => action.Origin.OwnerTurnTaker == HeroTurnTakerController.TurnTaker && this.GetTotalPowerCount() > 0 && action.CardToPlay.DoKeywordsContain("ammo"), DrawCardResponse, TriggerType.PlayCard, TriggerTiming.After);
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, EndOfTurnResponse, TriggerType.Hidden);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction arg)
        {
            IEnumerator coroutine = null;
            if (this.GetTotalPowerCount() > this.GetThisTurnPowerCount())
            {
                this.DecrementTotalPowerCount();
                coroutine = GameController.SendMessageAction("a reloaded effect expired", Priority.High, GetCardSource(), showCardSource: true);
            }
            this.ResetThisTurnPowerCount();
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    // One player may play a card now.
                    var e0 = SelectHeroToPlayCard(this.DecisionMaker);
                    if (UseUnityCoroutines)
                    {
                        yield return this.GameController.StartCoroutine(e0);
                    }
                    else
                    {
                        this.GameController.ExhaustCoroutine(e0);

                    }
                    break;
                case 1:
                    // One hero may use a power now.
                    var e1 = this.GameController.SelectHeroToUsePower(this.DecisionMaker, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return this.GameController.StartCoroutine(e1);
                    }
                    else
                    {
                        this.GameController.ExhaustCoroutine(e1);

                    }
                    break;
                case 2:
                    // One player may draw a card now
                    var e2 = this.GameController.SelectHeroToDrawCard(this.DecisionMaker, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return this.GameController.StartCoroutine(e2);
                    }
                    else
                    {
                        this.GameController.ExhaustCoroutine(e2);

                    }
                    break;
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Until the end of your next turn, whenever you play a gun card you may play an ammo card and whenever you play an ammo card you may draw a card. You may play a gun or ammo card now.
            this.IncrementThisTurnPowerCount();
            this.IncrementTotalPowerCount();

            var coroutine3 = this.GameController.SelectAndPlayCardFromHand(HeroTurnTakerController, true, null, new LinqCardCriteria(card => card.DoKeywordsContain("gun") || card.DoKeywordsContain("ammo")));
            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine3);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine3);
            }
        }

        private IEnumerator PlayAmmoResponse(PlayCardAction action)
        {
            for (int i = 0; i < this.GetTotalPowerCount(); i++)
            {
                var coroutine = SelectAndPlayCardFromHand(HeroTurnTakerController, true, null, new LinqCardCriteria(card => card.DoKeywordsContain("ammo")));
                if (UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine);
                }
            }
        }

        private IEnumerator DrawCardResponse(PlayCardAction action)
        {
            for (int i = 0; i < this.GetTotalPowerCount(); i++)
            {
                var coroutine = DrawCard(HeroTurnTaker, true);
                if (UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine);
                }
            }
        }
    }
}
