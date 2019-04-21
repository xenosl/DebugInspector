using System;
using System.Collections.Generic;

namespace ShuHai.DebugInspector.Editor
{
    public sealed class ScheduledActions
    {
        /// <summary>
        ///     Register an action that executes on <see cref="ExecuteScheduledActions"/> call.
        /// </summary>
        /// <param name="action"><see cref="Action" /> instance to schedule.</param>
        /// <remarks>
        ///     This is useful if there is any logic need to execute in of GUI calls, especially calls that may change GUI
        ///     contents.
        /// </remarks>
        public void ScheduleAction(Action action)
        {
            Ensure.Argument.NotNull(action, "action");
            scheduledActions.Add(action);
        }

        public void ExecuteScheduledActions()
        {
            for (int i = 0; i < scheduledActions.Count; ++i)
                scheduledActions[i]();
            scheduledActions.Clear();
        }

        private readonly List<Action> scheduledActions = new List<Action>();
    }
}