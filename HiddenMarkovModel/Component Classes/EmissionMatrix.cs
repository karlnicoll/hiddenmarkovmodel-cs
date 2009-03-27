using System;
using System.Collections.Generic;
using System.Text;

namespace HiddenMarkovModel
{
    /// <summary>
    /// Holds the observation-state relationships in an HMM
    /// </summary>
    class EmissionMatrix<T, U>
    {
        //==================================================================================
        #region *NOTES*
        
        // NOTES
        /* NOTE 1:  In this implementation of the state transition matrix, the "current"
         *          state is the row, and the next state is the column. This is as per:
         *          "Rabiner, L. R. (1989). A tutorial on hidden markov models and selected
         *          applications in speech recognition. Paper presented at the Proceedings
         *          of the IEEE, 257-286."
         */

        #endregion

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

        /// <summary>
        /// The matrix
        /// </summary>
        private Dictionary<T, Dictionary<U, decimal>> emissionMatrix;

        #endregion

        //==================================================================================
        #region Constructors/Destructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public EmissionMatrix()
        {
            emissionMatrix = new Dictionary<T, Dictionary<U, decimal>>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="States">The array of states that exist that the observations can represent</param>
        /// <param name="Observations">The array of observations that may represent any given state</param>
        /// <remarks>Default probabilities will be given for the probability of emission of
        /// any observation for a given state. The probability will be 1/n, where "n" is
        /// the number of observations given by the <see cref="Observations"/> parameter.
        /// It is expected that these transitions will be trained so that more appropriate
        /// probabilities will be found.</remarks>
        public EmissionMatrix(T[] States, U[] Observations)
        {
            GenerateMatrix(States, Observations);
        }

        #endregion

        //==================================================================================
        #region Public Properties

        /// <summary>
        /// Gets or sets a specified probability in the matrix
        /// </summary>
        /// <param name="State">The state to get the emission probability of</param>
        /// <param name="Observation">The observation that will be emitted</param>
        /// <returns>A Decimal representing the probability of the specified emission</returns>
        public decimal this[T State, U Observation]
        {
            get { return GetProbability(State, Observation); }
            set { SetProbability(State, Observation, value); }
        }

        #endregion

        //==================================================================================
        #region Public Methods

        #region Matrix Validation

        /// <summary>
        /// Checks to make sure that a specified Row (specified by the state) adds up to 1,
        /// and therefore obeys the laws of a Right Stochastic matrix
        /// </summary>
        /// <param name="State">The state that specifies the row to examine</param>
        /// <seealso cref="http://en.wikipedia.org/wiki/Stochastic_matrix"/>
        /// <returns>TRUE if the sum of the probabilities on the specified row add up to 1, FALSE otherwise</returns>
        public bool RowIsValid(T State)
        {
            return (GetRowProbabilitySum(State) == 1M);
        }

        public MatrixValidity MatrixIsValid()
        {
            MatrixValidity result = MatrixValidity.OK;

            //Check to see if the matrix is too small
            if (MatrixIsTooSmall())
            {
                result |= MatrixValidity.MatrixEmpty;
            }
            
            //Check to see if any of the rows do not add up to 1.0
            foreach (T curKey in emissionMatrix.Keys)
            {
                if (!RowIsValid(curKey))
                {
                    result |= MatrixValidity.RowSumInvalid;
                }
            }

            return result;

        }

        #endregion

        #region Matrix Creation

        /// <summary>
        /// Generates a transition matrix that contains the specified states
        /// </summary>
        /// <param name="States">The states that can be represented by an observation</param>
        /// <param name="Observations">The possible observations for each state</param>
        public void GenerateMatrix(T[] States, U[] Observations)
        {
            if (States == null || States.Length < MATRIX_MIN_SIZE)
            {
                throw new EmptyMatrixException("The list of states is empty.");
            }
            else if (Observations == null || Observations.Length < MATRIX_MIN_SIZE)
            {
                throw new EmptyMatrixException("The list of observations is empty.");
            }
            else
            {
                //Initialize the matrix (and initialize it so that it is the same size as the
                //amount of states)
                emissionMatrix = new Dictionary<T, Dictionary<U, decimal>>(States.Length);

                //Add the rows to the matrix
                /* e.g.         |           
                 *      --------------------
                 *      state1  |
                 *      state2  |
                 *      state3  |
                 *      stateN  |
                 */
                foreach (T curState in States)
                {
                    emissionMatrix.Add(curState, new Dictionary<U, decimal>(States.Length));
                }


                //Add the columns to the matrix, with the initial probabilities
                /* e.g.         | Ob1       | Ob2       | Ob3       | ObN
                 *      ----------------------------------------------------
                 *      state1  | 1/n       | 1/n       | 1/n       | 1/n
                 *      state2  | 1/n       | 1/n       | 1/n       | 1/n
                 *      state3  | 1/n       | 1/n       | 1/n       | 1/n
                 *      stateN  | 1/n       | 1/n       | 1/n       | 1/n
                 */
                foreach (KeyValuePair<T, Dictionary<U, decimal>> curRow in emissionMatrix)
                {
                    foreach (U curObservation in Observations)
                    {
                        curRow.Value.Add(curObservation, (decimal)(1M / (decimal)States.Length));
                    }
                }
            }
        }

        /// <summary>
        /// Resets all the probabilities in the matrix to their default values, which for a State transition matrix would be 1/Number_Of_States
        /// </summary>
        public void ResetToDefaultProbabilities()
        {
            foreach (KeyValuePair<T, Dictionary<U, decimal>> curRow in emissionMatrix)
            {
                foreach (U curKey in curRow.Value.Keys)
                {
                    curRow.Value[curKey] = 1M / (decimal)emissionMatrix.Count;
                }
            }
        }

        #endregion

        #endregion

        //==================================================================================
        #region Private/Protected Methods

        #region Row Manipulation

        /// <summary>
        /// Gets the sum of all the probabilities on a specified Row
        /// </summary>
        /// <param name="State">The state that specifies the row to sum</param>
        /// <returns>The sum of the probabilities on the row</returns>
        private decimal GetRowProbabilitySum(T State)
        {
            decimal sum = 0M;     //The sum of the values for the row

            //Get the sum
            foreach (KeyValuePair<U, decimal> curProbability in emissionMatrix[State])
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
        bool MatrixIsTooSmall()
        {
            return (emissionMatrix.Count < MATRIX_MIN_SIZE);
        }

        #endregion

        #region Probability Get/Set

        /// <summary>
        /// 
        /// </summary>
        /// <param name="State">The State that the observation represents</param>
        /// <param name="Observation">The observation</param>
        /// <returns>The probability of the observation referring to the state</returns>
        /// <exception cref="InvalidStateException">Raised when either the Current state or Next state is a state that is not contained within the matrix.</exception>
        /// <exception cref="EmptyMatrixException">Thrown when the matrix isn't big enough, or contains no rows</exception>
        /// <exception cref="StochasticMatrixException">Thrown when the row that you are getting the probability from does not add up to 1.00</exception>
        private decimal GetProbabilityWithValidation(T State, U Observation)
        {
            decimal retVal = 0M;

            //Check the matrix size
            if (emissionMatrix.Count < MATRIX_MIN_SIZE)
            {
                throw new EmptyMatrixException("Transition Matrix is empty");
            }

            //Check the matrix contains the state/Observation being sought
            else if (!emissionMatrix.ContainsKey(State))
            {
                throw new InvalidStateException("'State' is not a state contained within this matrix", State);
            }
            else if (!emissionMatrix[State].ContainsKey(Observation))
            {
                throw new InvalidStateException("'Observation' is not a valid observation contained within this matrix", Observation);
            }
            
            //Make sure that Row is valid
            else if (!RowIsValid(State))
            {
                throw new StochasticMatrixException("The row of the matrix that the probability is being fetched from does not add up to 1 exactly, and breaks the rules of a Right Stochastic Matrix.");
            }

            //Provided all the validation succeeds, get the probability
            else
            {
                retVal = GetProbability(State, Observation);
            }

            //Return the value after the validation
            return retVal;
        }

        /// <summary>
        /// Fetches the probability of a transition between two states
        /// </summary>
        /// <param name="CurrentState">The Current State that will be transitioned FROM</param>
        /// <param name="NextState">The next state that will be transitioned TO</param>
        /// <returns>The probability of the transition, as per the matrix</returns>
        private decimal GetProbability(T State, U Observation)
        {
            return emissionMatrix[State][Observation];
        }

        /// <summary>
        /// Sets a transition probability to the value specified
        /// </summary>
        /// <param name="State">The state that is being observed</param>
        /// <param name="Observation">The Observation</param>
        /// <param name="Probability">The probability of this transition occuring</param>
        private void SetProbability(T State, U Observation, decimal Probability)
        {
            emissionMatrix[State][Observation] = Probability;
        }

        #endregion

        #endregion
    }
}
