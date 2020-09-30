﻿using System.Management.Automation;
using Microsoft.SharePoint.Client;


namespace PnP.PowerShell.Commands.InformationManagement
{
    [Cmdlet(VerbsCommon.Set, "PnPSiteClosure")]
    
    
    
    public class SetSiteClosure : PnPWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public ClosureState State;

        protected override void ExecuteCmdlet()
        {
            if (State == ClosureState.Open)
            {
                SelectedWeb.SetOpenBySitePolicy();
            } else if (State == ClosureState.Closed)
            {
                if (this.SelectedWeb.GetAppliedSitePolicy() != null)
                {
                    SelectedWeb.SetClosedBySitePolicy();
                }
                else
                {
                    WriteWarning("No site policy applied. Set the Site Policy with Set-PnPSitePolicy and retrieve all available policies with Get-PnPSitePolicy -AllAvailable");
                }
            }
        }
    }
}
