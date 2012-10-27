namespace ISTUK.MathLibrary
{
    using System;

    public class NRSolver
    {
        private const int maxIterations = 20;
        private const double stopCondition = 1E-07;

        public NRSolver(int numEquations, int numVariables)
        {
            NumEquations = numEquations;
            NumVariables = numVariables;
        }

        private Matrix CalculateJacobian(ErrorFunction errorFunction, Vector guess)
        {
            var matrix = new Matrix(NumEquations, NumVariables);
            for (var i = 0; i < matrix.Columns; i++)
            {
                Vector vector2;
                int num4;
                Vector vector4;
                int num5;
                const double num2 = 1E-07;
                double num3 = (Math.Abs(guess[i]) >= 1.0) ? (Math.Abs(guess[i]) * num2) : num2;
                var vec = new Vector(guess);
                (vector2 = vec)[num4 = i] = vector2[num4] + num3;
                Vector vector3 = errorFunction(vec);
                (vector4 = vec)[num5 = i] = vector4[num5] - (2.0 * num3);
                Vector vector5 = errorFunction(vec);
                Vector vector6 = vector3 - vector5;
                matrix.SetColumn(i, vector6 / (2.0 * num3));
            }
            return matrix;
        }

        private bool IsDone(Vector delta)
        {
            for (var i = 0; i < delta.Rows; i++)
            {
                if (Math.Abs(delta[i]) > stopCondition)
                {
                    return false;
                }
            }
            return true;
        }

        public Vector Solve(ErrorFunction errorFunction, Vector initialGuess)
        {
            if (initialGuess.Size != NumVariables)
            {
                throw new MatrixException("Size of the initial guess vector is not correct");
            }
            var guess = new Vector(initialGuess);
            NumStepsToConverge = 0;
            for (var i = 0; i < maxIterations; i++)
            {
                Matrix matrix = CalculateJacobian(errorFunction, guess);
                Vector vector3 = errorFunction(guess);
                Matrix matrix2 = matrix.Transpose();
                var matrix3 = new SquareMatrix(matrix2 * matrix);
                var vector4 = matrix2 * vector3;
                Vector delta = matrix3.PseudoInverse() * vector4;
                guess -= delta;
                if (IsDone(delta))
                {
                    NumStepsToConverge = i + 1;
                    return guess;
                }
            }
            return guess;
        }

        public int NumEquations { get; private set; }


        private int NumVariables { get; set; }

        private int NumStepsToConverge { get; set; }
    }
}

