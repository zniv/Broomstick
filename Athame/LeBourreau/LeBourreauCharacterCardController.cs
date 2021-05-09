using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Athame.LeBourreau
{
    public class LeBourreauCharacterCardController : HeroCharacterCardController
    {
        public LeBourreauCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Destroy a target with 2 or less hitpoints.
            var coroutine = GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria(c => c.IsTarget && c.HitPoints <= 2 && c.IsInPlay, "target"), true, cardSource: this.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    // One player may play a card now.
                    var e0 = SelectHeroToPlayCard(DecisionMaker);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(e0);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(e0);

                    }
                    break;
                case 1:
                    // One hero may use a power now.
                    var e1 = GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(e1);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(e1);

                    }
                    break;
                case 2:
                    // One player may draw a card now
                    var e2 = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(e2);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(e2);

                    }
                    break;
            }
        }
    }
}
