using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class MagicSquareGenerator : MonoBehaviour {

   public static int[] generateSquare () {
      Restart:
      List<int> UsedNumbers = new List<int>();
      int[] x = new int[9];

      UsedNumbers.Clear();
      for (int i = 0; i < 9; i++) {
         x[i] = 0;
      }

      int Sum = 0;

      for (int i = 0; i < 3; i++) {
         do {
            x[i] = Rnd.Range(1, 100);
         } while (UsedNumbers.Contains(x[i]));
         UsedNumbers.Add(x[i]);
      }

      Sum = x[0] + x[1] + x[2];

      if (Sum > 150) {
         goto Restart;
      }

      int Target = Sum - x[0];

      for (int i = 0; i < 2; i++) {
         Target = Sum - x[i];
         do {
            x[3 + i] = Rnd.Range(1, Target / 2 + 1);
            x[6 + i] = Target - x[3 + i];
         } while (UsedNumbers.Contains(x[3 + i]) || UsedNumbers.Contains(x[6 + i]));
         UsedNumbers.Add(x[3 + i]);
         UsedNumbers.Add(x[6 + i]);
      }

      for (int i = 0; i < 3; i++) {
         x[i * 3 + 2] = Sum - x[i * 3 + 1] - x[i * 3];
      }

      for (int i = 0; i < 9; i++) {
         if (x[i] > 99 || x[i] <= 0) {
            goto Restart;
         }
      }

      return x;
   }
}
