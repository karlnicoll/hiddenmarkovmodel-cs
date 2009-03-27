using System;
using System.Collections.Generic;
using System.Text;

namespace HiddenMarkovModel
{
    /// <summary>
    /// The exception that is thrown when a specified state does not exist
    /// </summary>
    public class InvalidStateException : ApplicationException
    {
        //==================================================================================
        #region Data Structures



        #endregion

        //==================================================================================
        #region Enumerations



        #endregion

        //==================================================================================
        #region Constants

        private const string MESSAGE_DEFAULT = "The state does not apply here";

        #endregion

        //==================================================================================
        #region Events



        #endregion

        //==================================================================================
        #region Private Variables

        private object state;

        #endregion

        //==================================================================================
        #region Constructors/Destructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public InvalidStateException()
            : this(MESSAGE_DEFAULT)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Message">The message to explain why the exception has occurred</param>
        public InvalidStateException(string Message)
            : this(Message, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Message">The message to explain why the exception has occurred</param>
        /// <param name="InnerException">Any exception that led to this exception</param>
        public InvalidStateException(string Message, Exception InnerException)
            : this(Message, null, InnerException)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="BadState">The state that caused the exception to be raised</param>
        public InvalidStateException(object BadState)
            : this(MESSAGE_DEFAULT, BadState)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Message">The message to explain why the exception has occurred</param>
        /// <param name="BadState">The state that caused the exception to be raised</param>
        public InvalidStateException(string Message, object BadState)
            : this(Message, BadState, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Message">The message to explain why the exception has occurred</param>
        /// <param name="BadState">The state that caused the exception to be raised</param>
        /// <param name="InnerException">Any exception that led to this exception</param>
        public InvalidStateException(string Message, object BadState, Exception InnerException)
            : base(Message, InnerException)
        {
            state = BadState;
        }

        #endregion

        //==================================================================================
        #region Public Properties

        /// <summary>
        /// Gets the state that was mistakenly received
        /// </summary>
        public object State
        {
            get { return state; }
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
