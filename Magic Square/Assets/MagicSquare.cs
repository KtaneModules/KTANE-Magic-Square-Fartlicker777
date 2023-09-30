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
   int OldIndex;
   int[] IntendedSolution = new int[9];

   bool Animating;
   private bool hasSelected;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   void Awake () {
      ModuleId = ModuleIdCounter++;
      for (int i = 0; i < Buttons.Length; i++) {
         int j = i;
         Buttons[i].OnInteract += delegate () {
            ButtonPress(j);
            return false;
         };
      }
   }

   void Start () {
      MagicList = MagicSquareGenerator.generateSquare(ModuleId);
      IntendedSolution = new List<int>(MagicList).ToArray();
      Debug.LogFormat("[Magic Square #{0}] Intended solution is:", ModuleId);
      Debug.LogFormat("[Magic Square #{0}] {1} {2} {3}", ModuleId, IntendedSolution[0].ToString("00"), IntendedSolution[1].ToString("00"), IntendedSolution[2].ToString("00"));
      Debug.LogFormat("[Magic Square #{0}] {1} {2} {3}", ModuleId, IntendedSolution[3].ToString("00"), IntendedSolution[4].ToString("00"), IntendedSolution[5].ToString("00"));
      Debug.LogFormat("[Magic Square #{0}] {1} {2} {3}", ModuleId, IntendedSolution[6].ToString("00"), IntendedSolution[7].ToString("00"), IntendedSolution[8].ToString("00"));
      do {
         MagicList.Shuffle();
      }
      while (CheckIfCorrect());

      Debug.LogFormat("[Magic Square #{0}] Starting configuration is:", ModuleId);
      Debug.LogFormat("[Magic Square #{0}] {1} {2} {3}", ModuleId, MagicList[0].ToString("00"), MagicList[1].ToString("00"), MagicList[2].ToString("00"));
      Debug.LogFormat("[Magic Square #{0}] {1} {2} {3}", ModuleId, MagicList[3].ToString("00"), MagicList[4].ToString("00"), MagicList[5].ToString("00"));
      Debug.LogFormat("[Magic Square #{0}] {1} {2} {3}", ModuleId, MagicList[6].ToString("00"), MagicList[7].ToString("00"), MagicList[8].ToString("00"));

      for (int i = 0; i < 9; i++) {
         MagicNumbersText[i].text = MagicList[i].ToString("00");
      }
   }

   void ButtonPress (int Button) {
      if (Animating || ModuleSolved) {
         return;
      }

      if (hasSelected) {
         if (Button == OldIndex) {
            Audio.PlaySoundAtTransform("Click", transform);
            Buttons[Button].GetComponent<MeshRenderer>().material = ButtonColors[0];
            hasSelected = false;
            return;
         }

         Buttons[Button].GetComponent<MeshRenderer>().material = ButtonColors[1];
         StartCoroutine(SwapTiles(Button, OldIndex));

         int Temp = MagicList[Button];
         MagicList[Button] = MagicList[OldIndex];
         MagicList[OldIndex] = Temp;

         if (CheckIfCorrect()) {
            GetComponent<KMBombModule>().HandlePass();
         }
      }
      else {
         Audio.PlaySoundAtTransform("Click", transform);
         OldIndex = Button;
         Buttons[Button].GetComponent<MeshRenderer>().material = ButtonColors[1];
         hasSelected = true;
      }
   }

   IEnumerator SwapTiles (int a, int b) {
      Vector3 aOldPos = Buttons[a].transform.localPosition;
      Vector3 bOldPos = Buttons[b].transform.localPosition; Audio.PlaySoundAtTransform("Woosh", transform);
      Animating = true;
      var duration = 0.3f;
      var elapsed = 0f;
      while (elapsed < duration) {
         Buttons[a].transform.localPosition = new Vector3(
             Easing.InOutQuad(elapsed, aOldPos.x, bOldPos.x, duration),
             Easing.InOutQuad(elapsed, aOldPos.y, bOldPos.y, duration),
             Easing.InOutQuad(elapsed, aOldPos.z, bOldPos.z, duration)
             );
         Buttons[b].transform.localPosition = new Vector3(
             Easing.InOutQuad(elapsed, bOldPos.x, aOldPos.x, duration),
             Easing.InOutQuad(elapsed, bOldPos.y, aOldPos.y, duration),
             Easing.InOutQuad(elapsed, bOldPos.z, aOldPos.z, duration)
             );
         yield return null;
         elapsed += Time.deltaTime;
      }
      Buttons[a].transform.localPosition = aOldPos;
      Buttons[b].transform.localPosition = bOldPos;
      var oldTxtA = MagicNumbersText[a].text;
      var oldTxtB = MagicNumbersText[b].text;
      MagicNumbersText[a].text = oldTxtB;
      MagicNumbersText[b].text = oldTxtA;
      Buttons[a].GetComponent<MeshRenderer>().material = ButtonColors[0];
      Buttons[b].GetComponent<MeshRenderer>().material = ButtonColors[0];
      Animating = false;
      hasSelected = false;
   }

   bool CheckIfCorrect () {
      int[] Sum = new int[6];
      for (int i = 0; i < 9; i++) {
         Sum[i / 3] += MagicList[i];
      }

      Sum[3] = MagicList[0] + MagicList[3] + MagicList[6];
      Sum[4] = MagicList[1] + MagicList[4] + MagicList[7];
      Sum[5] = MagicList[2] + MagicList[5] + MagicList[8];

      if (Sum[0] == Sum[1] && Sum[1] == Sum[2] && Sum[2] == Sum[3] && Sum[3] == Sum[4] && Sum[4] == Sum[5]) {
         return true;
      }
      return false;
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} swap ## ## to swap the tiles with those two numbers.";
#pragma warning restore 414

   private IEnumerator ProcessTwitchCommand (string command) {
      var m = Regex.Match(command, @"^\s*swap\s+(\d+)\s+(\d+)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
      if (!m.Success)
         yield break;
      var ixs = new int[2];
      if (!int.TryParse(m.Groups[1].Value, out ixs[0]) || !int.TryParse(m.Groups[2].Value, out ixs[1]) || !MagicList.Contains(ixs[0]) || !MagicList.Contains(ixs[1])) {
         yield return "sendtochaterror Invalid command! Use !{0} swap ## ## to swap the two tiles.";
         yield break;
      }
      yield return null;
      Buttons[Array.IndexOf(MagicList, ixs[0])].OnInteract();
      while (Animating)
         yield return null;
      Buttons[Array.IndexOf(MagicList, ixs[1])].OnInteract();
   }

   private IEnumerator TwitchHandleForcedSolve () {
      for (int i = 0; i < 9; i++) {
         if (MagicList[i] != IntendedSolution[i]) {
            Buttons[i].OnInteract();
            while (Animating)
               yield return null;
            Buttons[Array.IndexOf(MagicList, IntendedSolution[i])].OnInteract();
            while (Animating)
               yield return null;
         }
      }
   }
}
