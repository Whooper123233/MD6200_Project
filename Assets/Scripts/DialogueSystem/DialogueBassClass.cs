using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueBaseClass : MonoBehaviour
    {
        public bool finished { get; private set; }

        protected IEnumerator WriteText(string input, Text textHolder, Color textColor, Font textFont, float delay, AudioClip sound, float delayBetweenLinesstring, string charName , Text charNameHolder)
        {
            textHolder.color = textColor;
            textHolder.font = textFont;

            if (charNameHolder != null)
            {
                charNameHolder.text = charName;
            }

            for (int i = 0; i < input.Length; i++)
            {
                textHolder.text += input[i];
                yield return new WaitForSeconds(delay);
            }
            if (textHolder == null)
                yield break;

            if (textFont != null)
                textHolder.font = textFont;

            if (textColor != default)
                textHolder.color = textColor;

            if (charNameHolder != null && !string.IsNullOrEmpty(charName))
            {
                charNameHolder.text = charName;
            }

            //yield return new WaitForSeconds(delayBetweenLines);
            yield return new WaitUntil(() => Input.GetMouseButton(0));
            finished = true;
        }
    }
}

