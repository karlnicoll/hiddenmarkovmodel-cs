using System;
using System.Collections.Generic;
using System.Text;

namespace HiddenMarkovModel
{
    /// <summary>
    /// A statistical model in which the system being modeled is assumed to be a Markov process with unknown parameters
    /// </summary>
    /// <typeparam name="StateType">The states that are used in this model</typeparam>
    /// <typeparam name="ObservationType">The observations that can be emitted from this model</typeparam>
    [Serializable]
    public class HiddenMarkovModel<StateType, ObservationType> : System.ICloneable
        where StateType : IComparable
        where ObservationType : IComparable
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
        /// The lowest possible probability for a emission or state transition
        /// </summary>
        private const double MIN_PROBABILITY_VALUE = 0.0000004;

        /// <summary>
        /// The default number of times the model will be trained with a specified
        /// observation sequence. May be less if the convergence criteria are met before
        /// this point
        /// </summary>
        public const int DEFAULT_TRAINING_ITERATIONS = 250;

        /// <summary>
        /// The default value that states that this instance has not got a specified
        /// functionality ID
        /// </summary>
        private const int MODEL_VALUE_UNSPECIFIED = -1;

        /// <summary>
        /// The default name for a model
        /// </summary>
        private const string MODEL_DEFAULT_NAME = "Unnamed HMM";

        #endregion

        //==================================================================================
        #region Events



        #endregion

        //==================================================================================
        #region Private Variables

        /// <summary>
        /// The state transition matrix for the model
        /// </summary>
        private StateTransitionMatrix<StateType> stm;

        /// <summary>
        /// The confusion matrix for the model
        /// </summary>
        private ConfusionMatrix<StateType, ObservationType> cm;

        /// <summary>
        /// The initial state probabilities for this model;
        /// </summary>
        private Dictionary<StateType, double> isp;

        /// <summary>
        /// The value that indicates what should be performed when this gesture is recognized
        /// </summary>
        private int modelValue;

        /// <summary>
        /// The string name used to identify this model
        /// </summary>
        private string modelName;

        /// <summary>
        /// Boolean to know whether or not the model has been trained using the Baum-Welch algorithm.
        /// </summary>
        private bool isTrained;

        #endregion

        //==================================================================================
        #region Constructors/Destructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public HiddenMarkovModel()
            : this(null, null, MODEL_DEFAULT_NAME, MODEL_VALUE_UNSPECIFIED)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ModelName">The name to give to this model</param>
        public HiddenMarkovModel(string ModelName)
            : this(null, null, ModelName, MODEL_VALUE_UNSPECIFIED)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ModelValue">An integer value to assign to this model to help identify what functionality to execute when this model
        /// is found to be the most probable model, given the observations</param>
        public HiddenMarkovModel(int ModelValue)
            : this(null, null, MODEL_DEFAULT_NAME, ModelValue)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ModelName">The name of this model</param>
        /// <param name="ModelValue">An integer ID to assign to this model to identify what functionality to execute
        /// should this model be identified as the model that emitted a set of observations</param>
        public HiddenMarkovModel(string ModelName, int ModelValue)
            : this(null, null, ModelName, ModelValue)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="States">The states that exist in the model</param>
        /// <param name="Observations">The observations that can be emitted by the states in the model</param>
        public HiddenMarkovModel(StateType[] States, ObservationType[] Observations)
            : this(States, Observations, MODEL_DEFAULT_NAME, MODEL_VALUE_UNSPECIFIED)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="States">The states used in this model</param>
        /// <param name="Observations">The possible observations used by this model</param>
        /// <param name="ModelName">The name of the model</param>
        /// <param name="ModelValue">The functionality value of the model</param>
        public HiddenMarkovModel(StateType[] States, ObservationType[] Observations, string ModelName, int ModelValue)
        {
            //Initialise the confusion matrix
            cm = new ConfusionMatrix<StateType, ObservationType>(States, Observations);

            //Initialise the state transition matrix
            stm = new StateTransitionMatrix<StateType>(States);

            //Initialise the initial state probabilities
            isp = new Dictionary<StateType, double>();
            if (States != null)
            {
                foreach (StateType curState in States)
                {
                    isp.Add(curState, 1D / (double)States.Length);
                }
            }

            //set the name of the model
            modelName = ModelName;

            //Set what value this model holds to identify what functionality to execute
            modelValue = ModelValue;
        }

        #endregion

        //==================================================================================
        #region Public Properties

        /// <summary>
        /// Gets the state transition matrix
        /// </summary>
        public StateTransitionMatrix<StateType> StateTransitionsMatrix      
        {
            get { return stm; }
            set { stm = value; }
        }

        /// <summary>
        /// Gets the confusion matrix for this model
        /// </summary>
        public ConfusionMatrix<StateType, ObservationType> ConfusionMatrix  
        {
            get { return cm; }
            set { cm = value; }
        }

        /// <summary>
        /// Gets the initial state probabilities for the model
        /// </summary>
        public Dictionary<StateType, double> InitialStateProbabilities      
        {
            get { return isp; }
            set { isp = value; }
        }

        /// <summary>
        /// Gets or sets the string name of this model
        /// </summary>
        public string Name
        {
            get { return modelName; }
            set { modelName = value; }
        }

        /// <summary>
        /// Gets or sets the "Functionality Identifier". The functionality identifier can be used by your application
        /// to identify what functionality to execute when this model is found to be the most probable model to emit
        /// a set of observations.
        /// </summary>
        public int FunctionalityID
        {
            get { return modelValue; }
            set { modelValue = value; }
        }

        /// <summary>
        /// Gets whether or not the model has been trained using the "Train(IList&lt;ObservationType&gt;)" Method.
        /// Models that have not been trained will throw an exception if their probability is requested.
        /// </summary>
        public bool IsTrained
        {
            get { return isTrained; }
        }

        #endregion

        //==================================================================================
        #region Public Methods

        #region ICloneable Members

        /// <summary>
        /// Clones this instance
        /// </summary>
        /// <returns>An object representing this class. The object can be casted into an HMM of the same type.</returns>
        /// <remarks> Code derived from "Deep Copy in C# (Cloning for a user defined class) by Surajit Datta (http://www.c-sharpcorner.com/UploadFile/sd_surajit/cloning05032007012620AM/cloning.aspx)</remarks>
        public object Clone()
        {
            //Declare the memory stream and formatter used to serialize
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            //Serialize this HMM into the memory stream
            bf.Serialize(ms, this);

            //Reset the memory stream to the start and deserialize the serialized model into an object
            ms.Position = 0;
            object retVal = bf.Deserialize(ms);
            ms.Close();

            //Return the cloned model
            return retVal;
        }

        #endregion

        #region Training

        /// <summary>
        /// Trains the model to better reflect state transitions and state observations
        /// </summary>
        /// <param name="TrainingInput">The observation set that will be used to identify the patterns.</param>
        public void Train(IList<ObservationType> TrainingInput)
        {
            Train_BaumWelchLog(TrainingInput, DEFAULT_TRAINING_ITERATIONS);
            isTrained = true;
        }

        /// <summary>
        /// Trains the model to better reflect state transitions and state observations
        /// </summary>
        /// <param name="TrainingInput">The observation set that will be used to identify the patterns.</param>
        /// <param name="MaxIterations">The amount of times the model should be trained using this training input.</param>
        public void Train(IList<ObservationType> TrainingInput, uint MaxIterations)
        {
            if (MaxIterations > 0)
            {
                Train_BaumWelchLog(TrainingInput, (int)MaxIterations);
                isTrained = true;
            }
        }

        #endregion

        #region Observation Probability

        /// <summary>
        /// Fetches the likelihood (between 0.0 and 1.0) of the observations seen being emitted by this model
        /// </summary>
        /// <param name="ObservationSequence">The sequence of observations seen</param>
        /// <returns>The probability that this model best explains the observations</returns>
        public double GetProbabilityOfObservations(IList<ObservationType> ObservationSequence)
        {
            //Using the forward variable, get the probability
            double retVal = 0;

            //If the model is trained, get the probability, if the model is not trained,
            //return a probability of zero to avoid errors
            if (isTrained)
            {
                retVal = GetProbabilityOfObservationsFromModel(ObservationSequence);
            }
            else
            {
                throw new ModelNotTrainedException("Observation probability requested for an untrained model");
            }

            return retVal;
        }

        #endregion

        #endregion

        //==================================================================================
        #region Private/Protected Methods

        #region Internal Probability Fetching Methods

        /// <summary>
        /// Gets the probability of a given observation sequence being emitted from the model
        /// </summary>
        /// <param name="ObservationSequence">The Observation Sequence</param>
        /// <returns>The probability of the observation sequence being emitted from the model</returns>
        /// <exception cref="ModelNotTrainedException">Thrown when this method is called before the model has been trained</exception>
        private double GetProbabilityOfObservationsFromModel(IList<ObservationType> ObservationSequence)
        {
            //Fetch the probability from the Forward Variable
            double[][] alpha = GetLogForwardVariable(ObservationSequence);

            //Using the forward variable, get the probability
            double retVal = 0;

            foreach (double curProb in alpha[alpha.Length - 1])
            {
                retVal += Math.Exp(curProb);
            }

            return retVal;
        }

        #endregion
        
        #region "Forward-Backward Procedure" methods

        /// <summary>
        /// Calculates the "lnAlpha" variable that contains the log of the probabilities of seeing any state at any point given it's previous observations
        /// </summary>
        /// <param name="ObservationSequence">The observation sequence used to calculate the log(P)</param>
        /// <returns>An array of lnAlpha[T][I] where T is the point in the observation sequence, and I is each state</returns>
        private double[][] GetLogForwardVariable(IList<ObservationType> ObservationSequence)
        {
            //Get the states used in this HMM and set N (N = number of states in the model)
            StateType[] states = stm.GetRowLabels();
            int N = states.Length;
            int T = ObservationSequence.Count;

            //Declare the forward variable
            double[][] lnAlpha = new double[T][];     //Holds the returned forward variable


            //1. Initialize the log forward variable
            //======================================

            //Initialize the first time slot
            lnAlpha[0] = new double[N];

            //Get the initial values for lnAlpha[0]
            for (int i = 0; i < N; i++)
            {
                //Get the log probability for state "i"
                lnAlpha[0][i] = ExtendedLnMultiply(ExtendedLn(isp[states[i]]), ExtendedLn(cm[states[i], ObservationSequence[0]]));
            }


            //2. Inductively calculate alpha (the forward variable)
            //=====================================================

            //Declare "LogAlpha". This variable is used to hold the log of the alpha calculation
            //for a specified state while it is still being calculated
            double logAlpha;

            /* NOTE:    t = 1 is used here because time 0 was calculated in the initialization
             *          part of the algorithm.
             */
            for (int t = 1; t < ObservationSequence.Count; t++)
            {
                lnAlpha[t] = new double[N];

                //Into lnAlpha[t][j], insert the log of the  probability of seeing each state based on which state we may have seen last time
                for (int j = 0; j < N; j++)
                {

                    //Initialize "logAlpha"
                    logAlpha = double.NaN;

                    //Calculate the log of the probability of reaching each state j from each of the previous states (state i)
                    for (int i = 0; i < N; i++)
                    {
                        logAlpha = ExtendedLnSum(logAlpha, ExtendedLnMultiply(lnAlpha[t - 1][i], ExtendedLn(stm[states[i], states[j]])));
                    }

                    //multiply the resulting log probability for seeing j by the probability of seeing state j from observation at time "t"
                    lnAlpha[t][j] = ExtendedLnMultiply(logAlpha, ExtendedLn(cm[states[j], ObservationSequence[t]]));
                }
            }

            //Return Alpha
            return lnAlpha;

        }

        /// <summary>
        /// Calculates the "lnBeta" variable that contains the log of the probabilities of seeing any state at any point given it's successive observations
        /// </summary>
        /// <param name="ObservationSequence">The observation sequence used to calculate the log(P)</param>
        /// <returns>An array of lnBeta[T][I] where T is the point in the observation sequence, and I is each state</returns>
        private double[][] GetLogBackwardVariable(IList<ObservationType> ObservationSequence)
        {
            //Get the states used in this HMM and set N (N = number of states in the model)
            StateType[] states = stm.GetRowLabels();
            int N = states.Length;
            int T = ObservationSequence.Count;

            //Declare the ln backward variable
            double[][] lnBeta = new double[T][];     //Holds the returned forward variable


            //1. Initialize the forward variable
            //==================================

            //Initialize the first part of the forward variable
            lnBeta[T - 1] = new double[N];

            //Get the initial values for beta[T - 1]
            for (int i = 0; i < states.Length; i++)
            {
                //Get the probability for state "i"
                lnBeta[T - 1][i] = 0;
            }


            //2. Inductively calculate lnBeta
            //===============================

            //Declare "LogBeta". This variable is used to hold the log of the Beta calculation
            //for a specified state while it is still being calculated
            double logBeta;

            /* NOTE:    t = 1 is used here because time 0 was used in the initialization
             *          part of the algorithm.
             */
            for (int t = T - 2; t >= 0; t--)
            {
                lnBeta[t] = new double[N];

                //Into beta, insert the log of the  probability of seeing each state based on which state succeeds it (this is known
                //because we are going backwards through the observation sequence).
                for (int i = 0; i < N; i++)
                {
                    //Initialize logBeta
                    logBeta = double.NaN;

                    //Calculate the probability of reaching each state j from each of the previous states (state i)
                    for (int j = 0; j < N; j++)
                    {
                        logBeta = ExtendedLnSum(logBeta, ExtendedLnMultiply(stm[states[i], states[j]], ExtendedLnMultiply(cm[states[j], ObservationSequence[t + 1]], lnBeta[t + 1][j])));
                    }

                    //Set the log Probability
                    lnBeta[t][i] = logBeta;
                }
            }

            //Return beta
            return lnBeta;
        }

        #endregion

        #region Training Algorithms

        #region Log Gamma and Log Xi

        /// <summary>
        /// Fetches the Log version of the gamma variable
        /// </summary>
        /// <param name="ObservationSequence">The observation sequence from which to calculate gamma</param>
        /// <param name="States">The states in the confusion and state transition matrices. If this parameter is null, it is calculated.</param>
        /// <param name="LnAlpha">The ln forward variable. If this parameter is null, it is calculated.</param>
        /// <param name="LnBeta">The ln backward variable. If this parameter is null, it is calculated.</param>
        /// <returns>The log gamma variable</returns>
        private double[][] LogGamma(IList<ObservationType> ObservationSequence, ref StateType[] States, ref double[][] LnAlpha, ref double[][] LnBeta)
        {
            //Get the forward and backward variables and state array if necessary
            if (LnAlpha == null) LnAlpha = GetLogForwardVariable(ObservationSequence);
            if (LnBeta == null) LnBeta = GetLogBackwardVariable(ObservationSequence);
            if (States == null) States = stm.GetRowLabels();

            //define T (number of observations) and N (the number of unique states)
            int T = ObservationSequence.Count;
            int N = States.Length;

            //Initialize gamma
            double[][] lnGamma = new double[T][];

            //Declare the "Normalizer" variable for holding the denominator of the lnGamma calculation
            double normalizer;

            //Populate lnGamma
            for (int t = 0; t < T; t++)
            {

                //For the probability of each state at time "t", create a new array
                lnGamma[t] = new double[N];

                //Initialize the denominator
                normalizer = double.NaN;


                //Get the probability of the state "i" at time "t"
                //================================================

                //Loop through the states, getting the gamma value for each one
                for (int i = 0; i < N; i++)
                {
                    lnGamma[t][i] = ExtendedLnMultiply(LnAlpha[t][i], LnBeta[t][i]);
                    normalizer = ExtendedLnSum(normalizer, lnGamma[t][i]);
                }

                //Generate the log(gamma) values as the log(P) of the state "i"
                //being the state represented by the observation, independent of
                //any previous values. We're simply making the gamma value
                //a percentage of the observation probability of seeing any state
                for (int i = 0; i < N; i++)
                {
                    lnGamma[t][i] = ExtendedLnMultiply(lnGamma[t][i], -normalizer);
                }

            }

            //Return the lnGamma variable
            return lnGamma;
        }

        private double[][,] LogXi(IList<ObservationType> ObservationSequence, ref StateType[] States, ref double[][] LnAlpha, ref double[][] LnBeta)
        {
            //define T (number of observations) and N (the number of unique states)
            int T = ObservationSequence.Count;
            int N = States.Length;

            //Initialize gamma
            double[][,] lnXi = new double[T][,];

            //Get the forward and backward variables and state array if necessary
            if (LnAlpha == null) LnAlpha = GetLogForwardVariable(ObservationSequence);
            if (LnBeta == null) LnBeta = GetLogBackwardVariable(ObservationSequence);
            if (States == null) States = stm.GetRowLabels();

            double normalizer;

            //Loop through the observation sequence
            for (int t = 0; t < T - 1; t++)
            {
                normalizer = double.NaN;

                lnXi[t] = new double[N, N];

                //Loop through the state transitions
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        lnXi[t][i, j] = ExtendedLnMultiply(LnAlpha[t][i], ExtendedLnMultiply(ExtendedLn(stm[States[i], States[j]]), ExtendedLnMultiply(ExtendedLn(cm[States[j], ObservationSequence[t + 1]]), LnBeta[t + 1][j])));
                        normalizer = ExtendedLnSum(normalizer, lnXi[t][i, j]);
                    }
                }

                //Now that we have the denominator (normalizer) and numerators (lnXi[t][i, j]), divide them to get the true lnXi value
                //Loop through the state transitions
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        lnXi[t][i, j] = ExtendedLnMultiply(lnXi[t][i, j], -normalizer);
                    }
                }
            }

            //return the xi variable
            return lnXi;

        }

        #endregion

        /// <summary>
        /// Trains the model using the Baum-Welch algorithm using logarithms instead of raw values.
        /// This method is preferred for large training sequences as it prevents underflow, which is
        /// common with large training sequences
        /// </summary>
        /// <param name="ObservationSequence">The training sequence</param>
        /// <param name="Iterations">The number of iterations to do on each training sequence</param>
        private void Train_BaumWelchLog(IList<ObservationType> ObservationSequence, int Iterations)
        {

            //Debug stuff
            #if DEBUG
                System.Diagnostics.Debug.WriteLine("Initial Probability: " + this.GetProbabilityOfObservationsFromModel(ObservationSequence));
            #endif


            //Convergance criteria. These variables help to determine whether or not to exit the loop
            bool convergenceAchieved = false;   //Whether or not to exit the loop
            bool firstIteration = true;         //Used to identify whether or not we need to do special stuff on the first iteration
            double averageGain = 0;             //The average improvement made by the training over the course of iteration
            double thisGain = 0;                //The improvement made by the training in the current iteration
            double lastProbValue = 0;           //The output probability produced by the last output

            //Declare the new HMM
            HiddenMarkovModel<StateType, ObservationType> newHMM;

            //Get the forward and backward variables
            double[][] lnAlpha;
            double[][] lnBeta;

            //Get the states, T, and N
            StateType[] states = stm.GetRowLabels();    //An array containing the states in this model
            int T = ObservationSequence.Count;          //The number of observations in the sequence
            int N = states.Length;                      //The number of states in the model

            //declare variable to hold gamma probabilities (the probability of being in a given state at a specified time)
            double[][] lnGamma;
            double[][,] lnXi;

            //variables to store the top and bottom of a division
            double denominator;
            double numerator;


            //Repeatedly train using the input sequence
            int iteration = 0;
            while (!convergenceAchieved && iteration < Iterations)
            {
                //Initialize the new model
                newHMM = (HiddenMarkovModel<StateType, ObservationType>)Clone();

                //Compute the new log alpha and log beta value
                lnAlpha = GetLogForwardVariable(ObservationSequence);
                lnBeta = GetLogBackwardVariable(ObservationSequence);

                //calculate ln gamma / ln xi
                lnGamma = LogGamma(ObservationSequence, ref states, ref lnAlpha, ref lnBeta);
                lnXi = LogXi(ObservationSequence, ref states, ref lnAlpha, ref lnBeta);


                //1. Train the initial state probabilities to the new probability of being at any given state at time 1
                //=====================================================================================================

                for (int i = 0; i < N; i++)
                {
                    newHMM.isp[states[i]] = ExtendedExp(lnGamma[0][i]);
                }


                //2. Train the state transition matrix
                //====================================

                //Go through state trans. matrix setting the values
                for (int i = 0; i < N; i++)
                {
                    denominator = double.NaN;
                    //Calculate the denominator
                    for (int t = 0; t < T - 1; t++)
                    {
                        denominator = ExtendedLnSum(denominator, lnGamma[t][i]);
                    }

                    for (int j = 0; j < N; j++)
                    {
                        numerator = double.NaN;
                        //Calculate the numerator
                        for (int t = 0; t < T - 1; t++)
                        {
                            numerator = ExtendedLnSum(numerator, lnXi[t][i, j]);
                        }

                        //Set the state transition probability
                        newHMM.stm[states[i], states[j]] = ExtendedExp(ExtendedLnMultiply(numerator, -denominator));
                    }
                }

                //3. Train the confusion matrix
                //=============================
                ObservationType[] Observations = cm.GetColumnLabels();
                int K = Observations.Length;

                //Go through each possible state/observation combo, generating the emission probability
                for (int j = 0; j < N; j++)
                {
                    //Reset the denominator so that it can be used in this section
                    denominator = double.NaN;

                    //Calculate the probability of  hitting state "j"
                    for (int t = 0; t < T; t++)
                    {
                        denominator = ExtendedLnSum(denominator, lnGamma[t][j]);
                    }

                    //Calculate the probability of hitting state "j" and observing K
                    for (int k = 0; k < K; k++)
                    {
                        
                        //Reset the numerator
                        numerator = double.NaN;

                        //The probability of observing observation k when in state j
                        for (int t1 = 0; t1 < T; t1++)
                        {
                            if (ObservationSequence[t1].CompareTo(Observations[k]) == 0)
                            {
                                numerator = ExtendedLnSum(numerator, lnGamma[t1][j]);
                            }

                            
                        }

                        newHMM.cm[states[j], Observations[k]] = ExtendedExp(ExtendedLnMultiply(numerator, -denominator));
                    }
                }



                //4. Adjust the new HMMs for zero probability correction
                //======================================================

                //if (newHMM.cm.ContainsValue(0D) || newHMM.stm.ContainsValue(0D))
                //{
                    //convergenceAchieved = true;
                //}
                //else
                //{

                //Confusion/ST Matrix normalization
                newHMM.cm.NormalizeLowValues(MIN_PROBABILITY_VALUE);
                newHMM.stm.NormalizeLowValues(MIN_PROBABILITY_VALUE);


                //Normalize the low values manually for the isp
                //foreach (StateType state in states)
                //{
                //    if (newHMM.isp[state].CompareTo(MIN_PROBABILITY_VALUE) < 0)
                //    {
                //        newHMM.isp[state] = MIN_PROBABILITY_VALUE;
                //    }
                //}

                ////Adjust the ISP matrix to be 1.0 again
                //double subtractAmount = 0D;
                //foreach (StateType state in states)
                //{
                //    subtractAmount += newHMM.isp[state];
                //}
                //subtractAmount = (subtractAmount - 1D) / states.Length;
                //foreach (StateType state in states)
                //{
                //    newHMM.isp[state] -= subtractAmount;
                //}



                //If any of the data is NaN, then end training NOW!
                //=================================================
                if (newHMM.cm.ContainsValue(double.NaN) || newHMM.stm.ContainsValue(double.NaN) || newHMM.isp.ContainsValue(double.NaN))
                {
                    convergenceAchieved = true;
                }
                else
                {


                    //5. Check for Convergence
                    //========================

                    //Get the probability
                    double resultantProbability;

                    resultantProbability = newHMM.GetProbabilityOfObservationsFromModel(ObservationSequence);



                    //Determine if convergence has been acheived (it is never achieved on the first iteration)
                    if (firstIteration)
                    {
                        averageGain = resultantProbability / 2;
                        lastProbValue = resultantProbability;
                        firstIteration = false;
                        iteration++;

                    }
                    else
                    {
                        thisGain = resultantProbability - lastProbValue;

                        //If this is less than 75% of the average gain, convergance has been achieved
                        if (thisGain < averageGain * 0.25)
                        {
                            convergenceAchieved = true;
                        }
                        else
                        {
                            //Set the convergence variables and continue
                            averageGain = (averageGain + thisGain) / 2;
                            lastProbValue = resultantProbability;
                            iteration++;
                        }
                    }

                    this.cm = newHMM.cm;
                    this.stm = newHMM.stm;
                    this.isp = newHMM.isp;
                    //}
                }

            }

            //Debug stuff
            #if DEBUG
                System.Diagnostics.Debug.WriteLine("Exited on Iteration " + iteration.ToString(), "Training");
                System.Diagnostics.Debug.WriteLine("Final Probability: " + this.GetProbabilityOfObservationsFromModel(ObservationSequence));
            #endif



        }

        #endregion

        #region Logarithmic Methods

        /// <summary>
        /// Gets the exponential of a given number, or 0 if the number is NaN
        /// </summary>
        /// <param name="x">the value to get the exponential of</param>
        /// <returns>The exp(x) or 0</returns>
        private double ExtendedExp(double x)
        {
            double retVal;
            if (double.IsNaN(x))
            {
                retVal = 0;
            }
            else
            {
                retVal = Math.Exp(x);
            }

            return retVal;
        }

        /// <summary>
        /// Gets the natural log of a number, or NaN if the number is 0
        /// </summary>
        /// <param name="x">The number to get the ln of</param>
        private double ExtendedLn(double x)
        {
            double retVal;
            if (x == 0)
            {
                retVal = double.NaN;
            }
            else if (x > 0)
            {
                retVal = Math.Log(x);
            }
            else
            {
                throw new Exception("Cannot determine ln of a negative number");
            }

            return retVal;
        }

        /// <summary>
        /// Provides the sum of two log numbers
        /// </summary>
        /// <param name="lnX">The Natural Log of the first number in the sum</param>
        /// <param name="lnY">The Natural Log of the second number in the sum</param>
        /// <returns>The sum of the two input parameters, or the opposide parameter if one parameter is NaN</returns>
        private double ExtendedLnSum(double lnX, double lnY)
        {
            double retVal;

            //If one parameter is NaN, then return the opposite parameter
            if (double.IsNaN(lnX) || double.IsNaN(lnY))
            {
                if (double.IsNaN(lnX))
                {
                    retVal = lnY;
                }
                else
                {
                    retVal = lnX;
                }
            }
            else
            {
                if (lnX > lnY)
                {
                    retVal = lnX + ExtendedLn(1D + Math.Exp(lnY - lnX));
                }
                else
                {
                    retVal = lnY + ExtendedLn(1D + Math.Exp(lnX - lnY));
                }
            }

            return retVal;
        }

        /// <summary>
        /// Multiplies together two logarithms
        /// </summary>
        /// <param name="lnX">The natural log of the first item to multiply</param>
        /// <param name="lnY">The natural log of the second item to multiply</param>
        /// <returns>The natural log of the product of the two numbers</returns>
        private double ExtendedLnMultiply(double lnX, double lnY)
        {
            double retVal;
            if (double.IsNaN(lnX) || double.IsNaN(lnY))
            {
                retVal = double.NaN;
            }
            else
            {
                retVal = lnX + lnY;
            }

            return retVal;

        }

        #endregion

        #endregion

    }
}
