using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class MagicSquare : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;

   public KMSelectable[] Buttons;
   public Material[] ButtonColors;

   public TextMesh[] MagicNumbersText;

   int[] MagicList = new int[9];
   int[] ColorIndices = new int[9];
   int OldIndex;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   void Awake () {
      ModuleId = ModuleIdCounter++;

      foreach (KMSelectable Button in Buttons) {
         Button.OnInteract += delegate () { ButtonPress(Button); return false; };
      }


      //button.OnInteract += delegate () { buttonPress(); return false; };

   }

   void Start () {
      MagicList = MagicSquareGenerator.generateSquare();
      for (int i = 0; i < 9; i++) {
         MagicNumbersText[i].text = MagicList[i].ToString("00");
      }
   }

   void ButtonPress (KMSelectable Button) {
      for (int i = 0; i < 9; i++) {
         if (Button == Buttons[i]) {
            ColorIndices[i]++;
            if (ColorIndices.Sum() % 2 == 0) {
               
               foreach (KMSelectable Butt in Buttons) {
                  Butt.GetComponent<MeshRenderer>().material = ButtonColors[0];
               }
            }
            else {
               OldIndex = i;
               Button.GetComponent<MeshRenderer>().material = ButtonColors[1];
            }
         }
      }
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} to do something.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      yield return null;
   }

   IEnumerator TwitchHandleForcedSolve () {
      yield return null;
   }
}
