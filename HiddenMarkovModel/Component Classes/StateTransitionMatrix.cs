using System;
using System.Collections.Generic;

namespace HiddenMarkovModel
{
    /// <summary>
    /// Stores the State Transition Matrix for the hidden markov model. Keeps a record of
    /// the probability of a state occuring based on the previous state.
    /// </summary>
    /// <typeparam name="StateType">The data type of the states</typeparam>
    [Serializable]
    public class StateTransitionMatrix<StateType> : RightStochasticMatrix<StateType, StateType>
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
        public StateTransitionMatrix()
            : base()
        { }

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
        public StateTransitionMatrix(StateType[] States)
            : base(States, States)
        { }

        #endregion

        //==================================================================================
        #region Public Properties



        #endregion

        //==================================================================================
        #region Public Methods



        #endregion

        //==================================================================================
        #region Private/Protected Methods



        #endregion
    }
}
