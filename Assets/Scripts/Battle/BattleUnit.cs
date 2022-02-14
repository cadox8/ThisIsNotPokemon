using System;
using System.Collections;
using pokemon;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Battle {
    public class BattleUnit : MonoBehaviour {

        [SerializeField] public bool isPlayerUnit;
        [SerializeField] public BattleHud hud;
        
        public Pokemon Pokemon { get; set; }

        private Image image;
        private Vector3 originalPos;
        private Color originalColor;

        private void Awake()
        {
            image = GetComponent<Image>();
            originalPos = image.transform.localPosition;
            originalColor = image.color;
        }

        public void Setup(Pokemon pokemon)
        {
            Pokemon = pokemon;
            if (this.isPlayerUnit)
            { 
                image.sprite = Pokemon.Base.BackSprite;
            }
            else
            {
                image.sprite = Pokemon.Base.FrontSprite;
            }

            hud.SetData(pokemon);

            transform.localScale = new Vector3(1, 1, 1);
            image.color = originalColor;
            PlayEnterAnimation();
        }

        public void PlayEnterAnimation()
        {
            if (isPlayerUnit)
            {
                image.transform.localPosition = new Vector3(-500, originalPos.y);
            }
            else
            {
                image.transform.localPosition = new Vector3(500, originalPos.y);
            }

            image.transform.DOLocalMoveX(originalPos.x, 1.5f);
        }
        
        public void PlayExitAnimation()
        {
            Sequence sequence = DOTween.Sequence();
            image.transform.localPosition = new Vector3(originalPos.x, originalPos.y);
            
            if (isPlayerUnit)
            {
                sequence.Append(image.transform.DOLocalMoveX(-500, 1.5f));
            }
            else
            {
                sequence.Append(image.transform.DOLocalMoveX(500, 1.5f));
            }
            sequence.Join(image.DOFade(0f, 0.5f));
        }

        public void PlayAttackAnimation()
        {
            Sequence sequence = DOTween.Sequence();

            if (isPlayerUnit)
            {
                sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50, 0.2f));
            }
            else
            {
                sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50, 0.2f));
            }

            sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.2f));
        }

        public void PlayHitAnimation()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(image.DOColor(Color.gray, 0.1f));
            sequence.Append(image.DOColor(originalColor, 0.1f));
        }

        public void PlayFaintAnimation()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
            sequence.Join(image.DOFade(0f, 0.5f));
        }

        public IEnumerator PlayCatchAnimation()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(image.DOFade(0, .5f));
            sequence.Join(image.transform.DOLocalMoveY(originalPos.y + 50, .5f));
            sequence.Join(transform.DOScale(new Vector3(.3f, .3f, 1), .5f));
            yield return sequence.WaitForCompletion();
        }
        
        public IEnumerator PlayBreakAnimation()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(image.DOFade(1, .5f));
            sequence.Join(image.transform.DOLocalMoveY(originalPos.y, .5f));
            sequence.Join(transform.DOScale(new Vector3(1, 1, 1), .5f));
            yield return sequence.WaitForCompletion();
        }
    }
}