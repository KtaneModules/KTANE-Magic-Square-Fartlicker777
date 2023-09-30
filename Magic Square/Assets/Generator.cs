using System.Collections.Generic;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class MagicSquareGenerator : MonoBehaviour {

   public static int[] generateSquare (int ModID) {
      

      int a = Rnd.Range(2, 30);
      int b = 0;
      int c = 0;


      do {
         b = Rnd.Range(a + 1, a + 30);
      } while (b == 2 * a);

      c = Rnd.Range(b + a + 1, b + a + 31);

      /*
       * General formula for a 3x3 magic square:
       * 
       * c - b | c + (a + b) | c - a
       * 
       * c - (a - b) | c | c + (a - b)
       * 
       * c + a | c - (a + b) | c + b
       */

      Debug.LogFormat("[Magic Square #{0}] A = {1}, B = {2}, C = {3}", ModID, a, b, c);
      return new int[] { c - b, c + a + b, c - a, c - (a - b), c, c + (a - b), c + a, c - (a + b), c + b};
   }
}
