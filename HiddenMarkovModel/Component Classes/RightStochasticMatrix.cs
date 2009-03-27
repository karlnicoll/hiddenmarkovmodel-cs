using System;
using System.Collections.Generic;
using System.Text;

namespace HiddenMarkovModel
{
    /// <summary>
    /// A Right Stochastic matrix where the rows in the matrix must add up to 1.0
    /// </summary>
    /// <typeparam name="TCol">The Data type of the columns of the matrix</typeparam>
    /// <typeparam name="TRow">The Data type of the rows of the matrix</typeparam>
    [Serializable]
    public class RightStochasticMatrix<TRow, TCol> : Matrix.Matrix<TRow, TCol, double>
    {
        //==================================================================================
        #region Data Structures



        #endregion

        //==================================================================================
        #region Enumerations



        #endregion

        //==================================================================================
        #region Constants

        /// <summary>
        /// The smallest capacity of the matrix. A 1x1 matrix is useless because the outcome probability will always be 1. A value of >= 2 is recommended
        /// </summary>
        private const int MATRIX_MIN_SIZE = 2;

        #endregion

        //==================================================================================
        #region Events



        #endregion

        //==================================================================================
        #region Private Variables



        #endregion

        //==================================================================================
        #region Constructors/Destructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public RightStochasticMatrix()
            : base(null, null, 0D)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Rows">The rows in this matrix</param>
        /// <param name="Columns">The columns in this matrix</param>
        public RightStochasticMatrix(TRow[] Rows, TCol[] Columns)
            : base (Rows, Columns, 1D / Columns.Length)
        { }


        #endregion

        //==================================================================================
        #region Public Properties

        /// <summary>
        /// Gets or sets the specified probability
        /// </summary>
        /// <param name="Row">The row in which the probability resides inside the matrix</param>
        /// <param name="Column">The column in which the probability resides inside the matrix</param>
        /// <returns>The probability specified by it's row and column</returns>
        public new double this[TRow Row, TCol Column]
        {
            get { return GetProbability(Row, Column); }
            set { SetProbability(Row, Column, value); }
        }

        #endregion

        //==================================================================================
        #region Public Methods

        #region Counting Methods

        /// <summary>
        /// Searches the matrix to see if it contains any values that are less than the specified value
        /// </summary>
        /// <param name="LowValue">The value that any matching values should be less than</param>
        /// <returns>The number of values less than the value specified</returns>
        public int CountValuesLessThan(double LowValue)
        {
            int retVal = 0;

            //Get the row labels
            TRow[] rows = this.GetRowLabels();
            TCol[] cols = this.GetColumnLabels();

            //Loop through the rows searching for matching values
            foreach (TRow curRow in rows)
            {
                foreach (TCol curCol in cols)
                {
                    if (matrix[curRow][curCol] < LowValue)
                    {
                        retVal++;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Searches the matrix to see if it contains any values that are more than the specified value
        /// </summary>
        /// <param name="HighValue">The value that any matching values should be more than</param>
        /// <returns>The number of values more than the value specified</returns>
        public int CountValuesMoreThan(double HighValue)
        {
            int retVal = 0;

            //Get the row labels
            TRow[] rows = this.GetRowLabels();
            TCol[] cols = this.GetColumnLabels();

            //Loop through the rows searching for matching values
            foreach (TRow curRow in rows)
            {
                foreach (TCol curCol in cols)
                {
                    if (matrix[curRow][curCol] > HighValue)
                    {
                        retVal++;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Searches the matrix to see if it contains any values that are equal to the specified value
        /// </summary>
        /// <param name="LowValue">The value that any matching values should be equal to</param>
        /// <returns>The number of values equal to the value specified</returns>
        public int CountValuesEqualTo(double Value)
        {
            int retVal = 0;

            //Get the row labels
            TRow[] rows = this.GetRowLabels();
            TCol[] cols = this.GetColumnLabels();

            //Loop through the rows searching for matching values
            foreach (TRow curRow in rows)
            {
                foreach (TCol curCol in cols)
                {
                    if (matrix[curRow][curCol] == Value)
                    {
                        retVal++;
                    }
                }
            }

            return retVal;
        }

        #endregion

        #region Matrix Bulk Editing

        /// <summary>
        /// Resets all the probabilities in the matrix to their default values, which for a confusion matrix would be 1/Number_Of_Possible_Observations
        /// </summary>
        public void ResetToDefaultProbabilities()
        {
            //Get the Rows
            TRow[] Rows = new TRow[matrix.Keys.Count];
            int index = 0;
            foreach (TRow curRowHeading in matrix.Keys)
            {
                Rows[index] = curRowHeading;
            }

            //Get the Columns
            TCol[] Columns = new TCol[matrix[Rows[0]].Keys.Count];
            index = 0;
            foreach (TCol curColHeading in matrix[Rows[0]].Keys)
            {
                Columns[index] = curColHeading;
            }


            //Make all values the same
            for (int r = 0; r < Rows.Length; r++)
            {
                for (int c = 0; c < Columns.Length; c++)
                {
                    matrix[Rows[r]][Columns[c]] = 1D / (double)Columns.Length;
                }
            }

        }

        /// <summary>
        /// Changes all the cells containing the value Low Value specified to the specified value, and spreads (RowTotal - 1) difference over the values so that the corrected values do not ill-skew probabilities
        /// </summary>
        /// <param name="ZeroValue">The value to look for</param>
        public void NormalizeLowValues(double LowValue)
        {
            //Continue adjusting until all the zeroes have been disposed of.
            while (this.CountValuesLessThan(LowValue) > 0)
            {
                //Get the Rows
                TRow[] Rows = new TRow[matrix.Keys.Count];
                matrix.Keys.CopyTo(Rows, 0);

                //Get the Columns
                TCol[] Columns = new TCol[matrix.Keys.Count];
                matrix[Rows[0]].Keys.CopyTo(Columns, 0);

                //Holds the difference between the row sum and 1.0
                double rowSum;          //The original sum of all the values on the row we are searching
                double newRowSum;       //The new sum of all the values on the row once adjustments have been made
                double subtractQty;     //The amount to subtract from all the >LowValue values in the row
                int cellsToAdjust;       //The number of cells to subtract the "subtractQty" from

                //Go Row by Row
                foreach (TRow curRow in Rows)
                {
                    //Set diff to zero
                    newRowSum = 0;
                    rowSum = 0;
                    cellsToAdjust = Columns.Length;

                    //Calculate initial difference
                    foreach (TCol curCol in Columns)
                    {
                        rowSum += matrix[curRow][curCol];
                    }

                    foreach (TCol curCol in Columns)
                    {
                        if (matrix[curRow][curCol].CompareTo(LowValue) < 0)
                        {
                            matrix[curRow][curCol] = LowValue;
                            cellsToAdjust--;
                        }
                        else if (matrix[curRow][curCol].CompareTo(LowValue) == 0)
                        {
                            cellsToAdjust--;
                        }


                        //Add the difference
                        newRowSum += matrix[curRow][curCol];
                    }

                    //Adjust the values to cater for the manipulation of the zeroes in this row
                    //=========================================================================

                    //Get the actual difference
                    newRowSum = newRowSum - rowSum;
                    subtractQty = newRowSum / cellsToAdjust;

                    foreach (TCol curCol in Columns)
                    {
                        if (matrix[curRow][curCol].CompareTo(LowValue) > 0)
                        {
                            matrix[curRow][curCol] = matrix[curRow][curCol] - subtractQty;
                        }
                    }

                }
            }
        }

        #endregion

        #region Matrix Validation

        /// <summary>
        /// Checks to make sure that a specified Row (specified by the Row) adds up to 1,
        /// and therefore obeys the laws of a Right Stochastic matrix
        /// </summary>
        /// <param name="Row">The Row that specifies the row to examine</param>
        /// <seealso cref="http://en.wikipedia.org/wiki/Stochastic_matrix"/>
        /// <returns>TRUE if the sum of the probabilities on the specified row add up to 1, FALSE otherwise</returns>
        public bool RowIsValid(TRow Row)
        {
            return (GetRowProbabilitySum(Row) == 1D);
        }

        public MatrixValidity MatrixIsValid()
        {
            MatrixValidity result = 0;

            //Check to see if the matrix is too small
            if (MatrixIsTooSmall())
            {
                result |= MatrixValidity.MatrixEmpty;
            }

            //Check to see if any of the rows do not add up to 1.0
            foreach (TRow curKey in matrix.Keys)
            {
                if (!RowIsValid(curKey))
                {
                    result |= MatrixValidity.RowSumInvalid;
                }
            }

            return result;

        }

        /// <summary>
        /// Gets the smallest value found in the matrix
        /// </summary>
        /// <param name="ExcludeZero">When TRUE will not classify zero as a value. This is useful for determining the smallest non-zero value, When FALSE will return the smallest value while treating zero as a value</param>
        /// <returns>The smallest value found in the matrix according to the parameters, or ZERO if all the values are zero</returns>
        public double GetMinValue(bool ExcludeZero)
        {
            //Get the Rows/Columns
            TRow[] rows = new TRow[matrix.Keys.Count];
            matrix.Keys.CopyTo(rows, 0);
            TCol[] cols = new TCol[matrix[rows[0]].Keys.Count];
            matrix[rows[0]].Keys.CopyTo(cols, 0);

            double retVal = this.GetMaxValue();

            foreach (TRow curState in rows)
            {
                foreach (TCol curOb in cols)
                {
                    if (!ExcludeZero)
                    {
                        if (matrix[curState][curOb] < retVal)
                        {
                            retVal = matrix[curState][curOb];
                        }
                    }
                    else
                    {
                        if (matrix[curState][curOb] < retVal && matrix[curState][curOb] != 0)
                        {
                            retVal = matrix[curState][curOb];
                        }
                    }

                }
            }

            return retVal;
        }

        /// <summary>
        /// Gets the largest value in the matrix
        /// </summary>
        /// <returns>The largest value in the matrix</returns>
        public double GetMaxValue()
        {
            //Get the Rows/Columns
            TRow[] Rows = new TRow[matrix.Keys.Count];
            matrix.Keys.CopyTo(Rows, 0);
            TCol[] Columns = new TCol[matrix[Rows[0]].Keys.Count];
            matrix[Rows[0]].Keys.CopyTo(Columns, 0);

            double retVal = matrix[Rows[0]][Columns[0]];

            foreach (TRow curState in Rows)
            {
                foreach (TCol curOb in Columns)
                {
                    if (matrix[curState][curOb] > retVal)
                    {
                        retVal = matrix[curState][curOb];
                    }
                }
            }

            return retVal;
        }

        #endregion

        #endregion

        //==================================================================================
        #region Private/Protected Methods

        #region Row Manipulation

        /// <summary>
        /// Gets the sum of all the probabilities on a specified Row
        /// </summary>
        /// <param name="Row">The Row that specifies the row to sum</param>
        /// <returns>The sum of the probabilities on the row</returns>
        private double GetRowProbabilitySum(TRow Row)
        {
            double sum = 0D;     //The sum of the values for the row

            //Get the sum
            foreach (KeyValuePair<TCol, double> curProbability in matrix[Row])
            {
                sum += curProbability.Value;
            }

            return sum;

        }

        #endregion

        #region Matrix Checking

        /// <summary>
        /// Compares the amount of rows in the matrix to the threshold amount stated by the constant MATRIX_MIN_SIZE
        /// </summary>
        /// <returns>TRUE if the matrix row count is less than MATRIX_MIN_SIZE, FALSE if the matrix meets the requirement</returns>
        private bool MatrixIsTooSmall()
        {
            return (matrix.Count < MATRIX_MIN_SIZE);
        }

        #endregion

        #region Probability Get/Set

        /// <summary>
        /// Gets the probability of a certain Col being a result of the given Row.
        /// </summary>
        /// <param name="Row">The Row that the Col represents</param>
        /// <param name="Col">The Col</param>
        /// <returns>The probability of the Col referring to the Row</returns>
        /// <exception cref="InvalidStateException">Raised when either the Current Row or Next Row is a Row that is not contained within the matrix.</exception>
        /// <exception cref="EmptyMatrixException">Thrown when the matrix isn't big enough, or contains no rows</exception>
        /// <exception cref="StochasticMatrixException">Thrown when the row that you are getting the probability from does not add up to 1.00</exception>
        private double GetProbabilityWithValidation(TRow Row, TCol Col)
        {
            double retVal = 0D;

            //Check the matrix size
            if (matrix.Count < MATRIX_MIN_SIZE)
            {
                throw new EmptyMatrixException("Transition Matrix is empty");
            }

            //Check the matrix contains the Row/Col being sought
            else if (!matrix.ContainsKey(Row))
            {
                throw new InvalidStateException("'Row' is not a Row contained within this matrix", Row);
            }
            else if (!matrix[Row].ContainsKey(Col))
            {
                throw new InvalidStateException("'Col' is not a valid Col contained within this matrix", Col);
            }

            //Make sure that Row is valid
            else if (!RowIsValid(Row))
            {
                throw new StochasticMatrixException("The row of the matrix that the probability is being fetched from does not add up to 1 exactly, and breaks the rules of a Right Stochastic Matrix.");
            }

            //Provided all the validation succeeds, get the probability
            else
            {
                retVal = GetProbability(Row, Col);
            }

            //Return the value after the validation
            return retVal;
        }

        /// <summary>
        /// Fetches the probability of a transition between two Rows
        /// </summary>
        /// <param name="CurrentState">The Current Row that will be transitioned FROM</param>
        /// <param name="NextState">The next Row that will be transitioned TO</param>
        /// <returns>The probability of the transition, as per the matrix</returns>
        protected double GetProbability(TRow Row, TCol Col)
        {
            return matrix[Row][Col];
        }

        /// <summary>
        /// Sets a transition probability to the value specified
        /// </summary>
        /// <param name="Row">The Row that is being observed</param>
        /// <param name="Col">The Col</param>
        /// <param name="Probability">The probability of this transition occuring</param>
        protected void SetProbability(TRow Row, TCol Col, double Probability)
        {
            matrix[Row][Col] = Probability;
        }

        #endregion

        #endregion
    }
}
