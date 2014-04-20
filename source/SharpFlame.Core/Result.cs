

using System;
using System.Collections.Generic;
using NLog;
using SharpFlame.Core.Interfaces;



namespace SharpFlame.Core
{
    public class Result : IResultItem
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public readonly List<IResultItem> Items = new List<IResultItem>();
        public string Text;
        private bool bad;
        private bool warnings;

        public Result(string text, bool log = true)
        {
            if (log)
            {
                logger.Debug(text);
            }

            Text = text;
        }

        public override string GetText
        {
            get { return Text; }
        }

        public bool HasWarnings
        {
            get { return warnings; }
        }

        public bool HasProblems
        {
            get { return bad; }
        }

        public void AddBypass(Result resultToAdd)
        {
            if (resultToAdd.HasWarnings)
            {
                Items.Add(resultToAdd);
            }
        }

        public void Add(Result resultToAdd)
        {
            if (resultToAdd.HasProblems)
            {
                bad = true;
            }
            if (resultToAdd.HasWarnings)
            {
                warnings = true;
            }
            Items.Add(resultToAdd);
        }

        public void Take(Result resultToMerge)
        {
            if (resultToMerge.HasProblems)
            {
                bad = true;
            }
            Items.AddRange(resultToMerge.Items);
        }

        public void ProblemAdd(string text, bool log = true)
        {
            if (log)
            {
                logger.Error(text);
            }
            var problem = new Problem();
            problem.Text = text;
            ItemAdd(problem);
        }

        public void WarningAdd(string text, bool log = true)
        {
            if (log)
            {
                logger.Warn(text);
            }
            var warning = new Warning();
            warning.Text = text;
            warnings = true;
            ItemAdd(warning);
        }

        public void ItemAdd(IResultItem item)
        {
            if (item is Problem)
            {
                bad = true;
            }
            Items.Add(item);
        }

        public override void DoubleClicked()
        {
        }

        public class Problem : IResultItem
        {
            public string Text;

            public override string GetText
            {
                get { return Text; }
            }

            public override void DoubleClicked()
            {
            }
        }

        public class Warning : IResultItem
        {
            public string Text;

            public override string GetText
            {
                get { return Text; }
            }

            public override void DoubleClicked()
            {
            }
        }

        public override string ToString() {
            return toStringHelper (this, 0);
        }

        private string toStringHelper (object item, int intending) {
            string result = "";
            result += String.Format ("{0}TASK: {1}\n", new string (' ', intending), ((Result)item).GetText);
            foreach (var lItem in ((Result)item).Items)
            {
                if (lItem is Result.Problem)
                {
                    result += String.Format ("{0}PROBLEM: {1}\n", new string (' ', intending+2), ((Result.Problem)lItem).GetText);
                } else if (lItem is Result.Warning)
                {
                    result += String.Format ("{0}WARNING: {1}\n", new string (' ', intending+2), ((Result.Warning)lItem).GetText);
                } else if (lItem is Core.Result)
                {
                    result += toStringHelper (lItem, intending + 4);
                }
            }

            return result;
        }
    }
}