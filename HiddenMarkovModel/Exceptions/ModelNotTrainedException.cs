using System;
using System.Collections.Generic;
using System.Text;

namespace HiddenMarkovModel
{
    /// <summary>
    /// An exception class that is thrown when the probability of an untrained model is requested
    /// </summary>
    public class ModelNotTrainedException : ApplicationException
    {
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
        /// Constructor
        /// </summary>
        public ModelNotTrainedException()
            : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Message">The message to describe what has happened</param>
        public ModelNotTrainedException(string Message)
            : base(Message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Message">The message to describe what has happened</param>
        /// <param name="InnerException">The exception that led to this exception</param>
        public ModelNotTrainedException(string Message, Exception InnerException)
            : base(Message, InnerException)
        {
        }

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
