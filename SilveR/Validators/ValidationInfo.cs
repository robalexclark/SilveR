using System;
using System.Collections.Generic;
using System.Linq;

namespace SilveR.Validators
{
    public class ValidationInfo
    {
        public bool ValidatedOK
        {
            get
            {
                return !errorMessages.Any();
            }
        }

        private readonly List<string> errorMessages = new List<string>();
        public IEnumerable<string> ErrorMessages
        {
            get { return errorMessages; }
        }

        private readonly List<string> warningMessages = new List<string>();
        public IEnumerable<string> WarningMessages
        {
            get { return warningMessages; }
        }


        public void AddErrorMessage(string message)
        {
            if (!ValidatedOK)
                throw new InvalidOperationException("Validation already failed");

            errorMessages.Add(message);
        }

        public void AddWarningMessage(string message)
        {
            if (!warningMessages.Contains(message) && !String.IsNullOrEmpty(message))
                warningMessages.Add(message);
        }
    }
}