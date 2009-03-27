using System;
using System.Collections.Generic;
using System.Text;

namespace HiddenMarkovModel
{
    /// <summary>
    /// Flags for matrix validity.
    /// </summary>
    [Flags]
    public enum MatrixValidity : byte
    {
        /// <summary>
        /// The Matrix is a valid stochastic State transition matrix
        /// </summary>
        OK = 0x00,
        /// <summary>
        /// The matrix does not have enough rows/columns
        /// </summary>
        MatrixEmpty = 0x01,
        /// <summary>
        /// This means that the sum of one or more rows in the matrix do
        /// not add up to 1.00. The state transition matrix is classified
        /// as a "Right-Stochastic matrix" and this means that all rows
        /// must add up to 1.
        /// </summary>
        RowSumInvalid = 0x02

    }
}
