using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueLine : DialogueBaseClass
    {
        private Text textHolder;

        [Header("Text Options")]
        [SerializeField] private string input;
        [SerializeField] private Color textColor;
        [SerializeField] private Font textFont;

        [Header("Time parameters")]
        [SerializeField] private float delay;
        [SerializeField] private float delayBetweenLines;

        [Header("Sound")]
        [SerializeField] private AudioClip sound;

        [Header("Character Image")]
        [SerializeField] private Sprite characterSprite;
        [SerializeField] private Image imageHolder;

        [Header("Character Name")]
        [SerializeField] private Text charNameHolder;
        [SerializeField] private string charName;

        private IEnumerator lineAppear;

        private void Awake()
        {
    
            if (imageHolder != null && characterSprite != null)
            {
                imageHolder.sprite = characterSprite;
                imageHolder.preserveAspect = true;
            }
            if (sound != null)
            {
                AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position);
            }
        }

        private void OnEnable()
        {
            ResetLine();
            lineAppear = WriteText(input, textHolder, textColor, textFont, delay, sound, delayBetweenLines, charName, charNameHolder);
            StartCoroutine(lineAppear);

        }
        private void ResetLine()
        {
            textHolder = GetComponent<Text>();
            textHolder.text = "";
            finished = false;
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
            {
                if(textHolder.text != input)
                {
                    StopCoroutine(lineAppear);
                    textHolder.text = input;
                    
                }
                else
                {
                    finished = true;
                } 

            }
        }
    }
}