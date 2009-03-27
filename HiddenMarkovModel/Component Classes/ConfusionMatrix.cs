using System;
using System.Collections.Generic;
using System.Text;
using Matrix;

namespace HiddenMarkovModel
{
    /// <summary>
    /// Holds the observation-state relationships in an HMM
    /// </summary>
    /// <typeparam name="StateType">The states that emit the observations</typeparam>
    /// <typeparam name="ObservationType">The observations that can represent a given state</typeparam>
    [Serializable]
    public class ConfusionMatrix<StateType, ObservationType> : RightStochasticMatrix<StateType, ObservationType>
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
        public ConfusionMatrix()
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
        public ConfusionMatrix(StateType[] States, ObservationType[] Observations)
            : base(States, Observations)
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
