using System.Collections;
using Lean.Transition;
using UnityEngine;
using UnityEngine.UI;

namespace Ieedo
{
    public class UIScreen : UIInteractable
    {
        public Image BlockerBG;

        public virtual ScreenID ID => ScreenID.None;
        public virtual bool AutoAnimate => true;

        public Vector3 DefaultOutEnterPosition = new(-2500, 0, 0);
        public Vector3 DefaultOutExitPosition = new(2500, 0, 0);

        public bool IsOpen => gameObject.activeSelf;

        public void OpenImmediate()
        {
            SoundManager.I.PlaySfx(SfxEnum.open);
            gameObject.SetActive(true);
        }

        public void CloseImmediate(bool reset = false)
        {
            if (!reset) SoundManager.I.PlaySfx(SfxEnum.close);
            gameObject.SetActive(false);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            StartCoroutine(OpenCO());
        }

        public void Close()
        {
            StartCoroutine(CloseCO());
        }

        private float animPeriod = 0.25f;
        private Ref<Vector3> startPos;
        public IEnumerator OpenCO()
        {
            yield return OnPreOpen();
            SoundManager.I.PlaySfx(SfxEnum.open);
            if (BlockerBG != null)
            {
                var col = BlockerBG.color;
                col.a = 1f;
                BlockerBG.colorTransition(col,animPeriod);
            }

            gameObject.SetActive(true);
            if (AutoAnimate)
            {
                if (startPos == null)
                {
                    startPos = new Ref<Vector3>();
                    startPos.Value = transform.localPosition;
                }
                transform.localPosition = DefaultOutEnterPosition;
                transform.localPositionTransition(startPos.Value, animPeriod);
                yield return new WaitForSeconds(animPeriod);
            }
            yield return OnOpen();
        }

        public IEnumerator CloseCO()
        {
            SoundManager.I.PlaySfx(SfxEnum.close);
            if (BlockerBG != null)
            {
                var col = BlockerBG.color;
                col.a = 0.0f;
                BlockerBG.colorTransition(col, animPeriod);
            }

            if (AutoAnimate)
            {
                transform.localPositionTransition(DefaultOutExitPosition, animPeriod);
                yield return new WaitForSeconds(animPeriod);
            }
            yield return OnClose();
            gameObject.SetActive(false);
        }

        protected virtual IEnumerator OnPreOpen() { yield break; }
        protected virtual IEnumerator OnOpen() { yield break; }
        protected virtual IEnumerator OnClose() { yield break; }

    }
}
