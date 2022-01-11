using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Rnd = UnityEngine.Random;
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
   int[] IntendedSolution = new int[9];
   int[] ButtonIndices = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

   bool Animating;

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
      IntendedSolution = MagicList;
      Debug.LogFormat("[Magic Square #{0}] Intended solution is:", ModuleId);
      Debug.LogFormat("[Magic Square #{0}] {1} {2} {3}", ModuleId, IntendedSolution[0].ToString("00"), IntendedSolution[1].ToString("00"), IntendedSolution[2].ToString("00"));
      Debug.LogFormat("[Magic Square #{0}] {1} {2} {3}", ModuleId, IntendedSolution[3].ToString("00"), IntendedSolution[4].ToString("00"), IntendedSolution[5].ToString("00"));
      Debug.LogFormat("[Magic Square #{0}] {1} {2} {3}", ModuleId, IntendedSolution[6].ToString("00"), IntendedSolution[7].ToString("00"), IntendedSolution[8].ToString("00"));
      do {
         MagicList.Shuffle();
      } while (CheckIfCorrect());

      Debug.LogFormat("[Magic Square #{0}] Starting configuration is:", ModuleId);
      Debug.LogFormat("[Magic Square #{0}] {1} {2} {3}", ModuleId, MagicList[0].ToString("00"), MagicList[1].ToString("00"), MagicList[2].ToString("00"));
      Debug.LogFormat("[Magic Square #{0}] {1} {2} {3}", ModuleId, MagicList[3].ToString("00"), MagicList[4].ToString("00"), MagicList[5].ToString("00"));
      Debug.LogFormat("[Magic Square #{0}] {1} {2} {3}", ModuleId, MagicList[6].ToString("00"), MagicList[7].ToString("00"), MagicList[8].ToString("00"));

      for (int i = 0; i < 9; i++) {
         MagicNumbersText[i].text = MagicList[i].ToString("00");
         //MagicNumbersText[i].text = i.ToString();
      }
   }

   void ButtonPress (KMSelectable Button) {
      if (Animating) {
         return;
      }
      for (int i = 0; i < 9; i++) {
         if (Button == Buttons[ButtonIndices[i]]) {
            ColorIndices[ButtonIndices[i]]++;
            if (ColorIndices.Sum() % 2 == 0) {
               Button.GetComponent<MeshRenderer>().material = ButtonColors[1];

               StartCoroutine(SwapTiles(Button, Buttons[ButtonIndices[OldIndex]]));

               int Temp = ButtonIndices[i];
               ButtonIndices[i] = ButtonIndices[OldIndex];
               ButtonIndices[OldIndex] = Temp;

               /*Debug.Log(ButtonIndices[0] + " " + ButtonIndices[1] + " " + ButtonIndices[2]);
               Debug.Log(ButtonIndices[3] + " " + ButtonIndices[4] + " " + ButtonIndices[5]);
               Debug.Log(ButtonIndices[6] + " " + ButtonIndices[7] + " " + ButtonIndices[8]);


               Debug.Log(MagicList[ButtonIndices[0]] + " " + MagicList[ButtonIndices[1]] + " " + MagicList[ButtonIndices[2]]);
               Debug.Log(MagicList[ButtonIndices[3]] + " " + MagicList[ButtonIndices[4]] + " " + MagicList[ButtonIndices[5]]);
               Debug.Log(MagicList[ButtonIndices[6]] + " " + MagicList[ButtonIndices[7]] + " " + MagicList[ButtonIndices[8]]);*/

               if (CheckIfCorrect()) {
                  GetComponent<KMBombModule>().HandlePass();
               }
            }
            else {
               Audio.PlaySoundAtTransform("Click", transform);
               OldIndex = i;
               Button.GetComponent<MeshRenderer>().material = ButtonColors[1];
            }
            break;
         }
      }
   }

   IEnumerator SwapTiles (KMSelectable a, KMSelectable b) {
      Vector3 aOldPos = a.transform.localPosition;
      Vector3 bOldPos = b.transform.localPosition;
      if (a == b) {
         Audio.PlaySoundAtTransform("Click", transform);
      }
      else {
         Audio.PlaySoundAtTransform("Woosh", transform);
      }
      Animating = true;
      float i = 0;
      while (i < 1 && a.transform.localPosition != bOldPos) {
         i += Time.deltaTime / 1f;
         a.transform.localPosition = Vector3.Lerp(a.transform.localPosition, bOldPos, i);
         b.transform.localPosition = Vector3.Lerp(b.transform.localPosition, aOldPos, i);
         yield return new WaitForSeconds(.01f);
      }
      a.GetComponent<MeshRenderer>().material = ButtonColors[0];
      b.GetComponent<MeshRenderer>().material = ButtonColors[0];
      Animating = false;
   }

   bool CheckIfCorrect () {
      int[] Sum = new int[6];
      for (int i = 0; i < 9; i++) {
         Sum[i / 3] += MagicList[ButtonIndices[i]];
      }

      Sum[3] = MagicList[ButtonIndices[0]] + MagicList[ButtonIndices[3]] + MagicList[ButtonIndices[6]];
      Sum[4] = MagicList[ButtonIndices[1]] + MagicList[ButtonIndices[4]] + MagicList[ButtonIndices[7]];
      Sum[5] = MagicList[ButtonIndices[2]] + MagicList[ButtonIndices[5]] + MagicList[ButtonIndices[8]];

      //Debug.Log(Sum[0].ToString() + " " + Sum[1].ToString() + " " + Sum[2].ToString() + " " + Sum[3].ToString() + " " + Sum[4].ToString() + " " + Sum[5].ToString());

      if (Sum[0] == Sum[1] && Sum[1] == Sum[2] && Sum[2] == Sum[3] && Sum[3] == Sum[4] && Sum[4] == Sum[5]) {
         return true;
      }
      else {
         return false;
      }
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} ## to select that number. I will not improve this command.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      Command = Command.Trim();
      yield return null;
      for (int i = 0; i < 9; i++) {
         if (Command == MagicNumbersText[i].text) {
            Buttons[i].OnInteract();
            yield return new WaitForSeconds(.1f);
         }
      }
   }

   /*IEnumerator TwitchHandleForcedSolve () {
      Debug.Log(1);
      if (ColorIndices.Sum() % 2 != 0) {
         Debug.Log(2);
         Buttons[Rnd.Range(0, 9)].OnInteract();
         Debug.Log(3);
         while (Animating) {
            Debug.Log(4);
            yield return true;
         }
      }
      Debug.Log(5);
      for (int i = 0; i < 9; i++) {
         Debug.Log(6);
         if (IntendedSolution[i].ToString("00") != MagicNumbersText[ButtonIndices[i]].text) {
            Debug.Log(7);
            Buttons[ButtonIndices[i]].OnInteract();
            for (int j = 0; j < 9; j++) {
               Debug.Log(8);
               if (IntendedSolution[i] == MagicList[ButtonIndices[j]]) {
                  Debug.Log(9);
                  Buttons[ButtonIndices[j]].OnInteract();
                  while (Animating) {
                     Debug.Log(10);
                     yield return true;
                  }
               }
            }
         }
      }
   }*/
}
