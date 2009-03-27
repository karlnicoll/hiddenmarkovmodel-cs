using System;
using System.Collections.Generic;
using System.Text;

namespace HiddenMarkovModel
{
    /// <summary>
    /// The exception that is thrown when a specified Observation does not exist
    /// </summary>
    public class InvalidObservationException : ApplicationException
    {
        //==================================================================================
        #region Data Structures



        #endregion

        //==================================================================================
        #region Enumerations



        #endregion

        //==================================================================================
        #region Constants

        private const string MESSAGE_DEFAULT = "The Observation does not apply here";

        #endregion

        //==================================================================================
        #region Events



        #endregion

        //==================================================================================
        #region Private Variables

        private object observation;

        #endregion

        //==================================================================================
        #region Constructors/Destructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public InvalidObservationException()
            : this(MESSAGE_DEFAULT)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Message">The message to explain why the exception has occurred</param>
        public InvalidObservationException(string Message)
            : this(Message, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Message">The message to explain why the exception has occurred</param>
        /// <param name="InnerException">Any exception that led to this exception</param>
        public InvalidObservationException(string Message, Exception InnerException)
            : this(Message, null, InnerException)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="BadObservation">The Observation that caused the exception to be raised</param>
        public InvalidObservationException(object BadObservation)
            : this(MESSAGE_DEFAULT, BadObservation)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Message">The message to explain why the exception has occurred</param>
        /// <param name="BadObservation">The Observation that caused the exception to be raised</param>
        public InvalidObservationException(string Message, object BadObservation)
            : this(Message, BadObservation, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Message">The message to explain why the exception has occurred</param>
        /// <param name="BadObservation">The Observation that caused the exception to be raised</param>
        /// <param name="InnerException">Any exception that led to this exception</param>
        public InvalidObservationException(string Message, object BadObservation, Exception InnerException)
            : base(Message, InnerException)
        {
            observation = BadObservation;
        }

        #endregion

        //==================================================================================
        #region Public Properties

        /// <summary>
        /// Gets the Observation that was mistakenly received
        /// </summary>
        public object Observation
        {
            get { return observation; }
        }

        #endregion

        //==================================================================================
        #region Public Methods


        #endregion

        //==================================================================================
        #region Private/Protected Methods



        #endregion
    }
}
