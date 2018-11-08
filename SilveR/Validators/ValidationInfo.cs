using System;
using System.Collections.Generic;

namespace SilveR.Validators
{
    public class ValidationInfo
    {
        private bool validatedOK = true;
        public bool ValidatedOK { get { return validatedOK; } }

        private List<string> errorMessages = new List<string>();
        public IEnumerable<string> ErrorMessages
        {
            get { return errorMessages; }
        }

        private List<string> warningMessages = new List<string>();
        public IEnumerable<string> WarningMessages
        {
            get { return warningMessages; }
        }


        public void AddErrorMessage(string message)
        {
            if (!validatedOK) throw new InvalidOperationException("Validation already failed");

            errorMessages.Add(message);
            validatedOK = false;
        }

        public void AddWarningMessage(string message)
        {
            if (!warningMessages.Contains(message) && !String.IsNullOrEmpty(message))
                warningMessages.Add(message);
        }
    }
}