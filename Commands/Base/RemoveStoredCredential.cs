﻿using System.Management.Automation;


namespace PnP.PowerShell.Commands.Base
{
    [Cmdlet(VerbsCommon.Remove, "PnPStoredCredential")]
    public class RemoveStoredCredential : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Name;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ProcessRecord()
        {
            var cred = Utilities.CredentialManager.GetCredential(Name);
            if (cred != null)
            {
                if (Force || ShouldContinue($"Remove credential {Name}?", "Confirm"))
                {
                    if (!Utilities.CredentialManager.RemoveCredential(Name))
                    {
                        WriteError(new ErrorRecord(new System.Exception($"Credential {Name} not removed"), "CREDENTIALNOTREMOVED", ErrorCategory.WriteError, Name));
                    }
                }
            }
            else
            {
                WriteError(new ErrorRecord(new System.Exception($"Credential {Name} not found"), "CREDENTIALNOTFOUND", ErrorCategory.ObjectNotFound, Name));
            }
        }
    }
}
