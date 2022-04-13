using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapShot.Model
{
    public enum EditOperationKind : byte
    {
        None,
        Add,
        Edit,
        Remove
    };

    public class Levenshtein
    {
        #region Properties
        public char ValueFrom { get; }
        public char ValueTo { get; }
        public EditOperationKind Operation { get; }

        #endregion

        #region Constructor

        public Levenshtein(char valueFrom, char valueTo, EditOperationKind operation)
        {
            ValueFrom = valueFrom;
            ValueTo = valueTo;

            Operation = valueFrom == valueTo ? EditOperationKind.None : operation;
        }

        #endregion

        #region Methods

        public static string FindDifferences(
            string source, string target, int insertCost = 1, int removeCost = 1, int editCost = 2)
        {
            EditOperationKind[][] M = Enumerable
              .Range(0, source.Length + 1)
              .Select(line => new EditOperationKind[target.Length + 1])
              .ToArray();

            int[][] D = Enumerable
              .Range(0, source.Length + 1)
              .Select(line => new int[target.Length + 1])
              .ToArray();

            for (int i = 1; i <= source.Length; ++i)
            {
                M[i][0] = EditOperationKind.Remove;
                D[i][0] = removeCost * i;
            }

            for (int i = 1; i <= target.Length; ++i)
            {
                M[0][i] = EditOperationKind.Add;
                D[0][i] = insertCost * i;
            }

            for (int i = 1; i <= source.Length; ++i)
                for (int j = 1; j <= target.Length; ++j)
                {
                    int insert = D[i][j - 1] + insertCost;
                    int delete = D[i - 1][j] + removeCost;
                    int edit = D[i - 1][j - 1] + (source[i - 1] == target[j - 1] ? 0 : editCost);

                    int min = Math.Min(Math.Min(insert, delete), edit);

                    if (min == insert)
                        M[i][j] = EditOperationKind.Add;
                    else if (min == delete)
                        M[i][j] = EditOperationKind.Remove;
                    else if (min == edit)
                        M[i][j] = EditOperationKind.Edit;

                    D[i][j] = min;
                }

            List<Levenshtein> result =
              new List<Levenshtein>(source.Length + target.Length);

            for (int x = target.Length, y = source.Length; (x > 0) || (y > 0);)
            {
                EditOperationKind op = M[y][x];

                if (op == EditOperationKind.Add)
                {
                    
                    x -= 1;
                    result.Add(new Levenshtein('\0', target[x], op));
                }
                else if (op == EditOperationKind.Remove)
                {
                    y -= 1;
                    result.Add(new Levenshtein(source[y], '\0', op));
                }
                else if (op == EditOperationKind.Edit)
                {
                    
                    x -= 1;
                    y -= 1;
                    result.Add(new Levenshtein(source[y], target[x], op));
                }
                else
                    break;
            }

            result.Reverse();

            string res = "";
            foreach (var r in result)
            {
                if (r.Operation == EditOperationKind.Add || r.Operation == EditOperationKind.Edit)
                    res += r.ValueTo;
            }

            return res;
        }

        #endregion
    }
}
