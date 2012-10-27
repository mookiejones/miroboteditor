using System.ComponentModel;

namespace miRobotEditor.Core.EventArgs
{
    /// <summary>
    /// 
    /// </summary>
    public class SolutionEventArgs : System.EventArgs
    {
        Solution solution;

        /// <summary>
        /// 
        /// </summary>
        public Solution Solution
        {
            get
            {
                return solution;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="solution"></param>
        public SolutionEventArgs(Solution solution)
        {
            this.solution = solution;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class SolutionCancelEventArgs : CancelEventArgs
    {
        Solution solution;

        /// <summary>
        /// 
        /// </summary>
        public Solution Solution
        {
            get
            {
                return solution;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="solution"></param>
        public SolutionCancelEventArgs(Solution solution)
        {
            this.solution = solution;
        }
    }
}
