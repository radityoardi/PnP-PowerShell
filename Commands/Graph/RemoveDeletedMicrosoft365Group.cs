﻿using System.Management.Automation;
using PnP.Framework.Graph;
using PnP.PowerShell.Commands.Attributes;
using PnP.PowerShell.Commands.Base;
using PnP.PowerShell.Commands.Base.PipeBinds;

namespace PnP.PowerShell.Commands.Graph
{
    [Cmdlet(VerbsCommon.Remove, "PnPDeletedMicrosoft365Group")]
    [Alias("Remove-PnPDeletedUnifiedGroup")]
    [CmdletMicrosoftGraphApiPermission(MicrosoftGraphApiPermission.Group_ReadWrite_All)]
    public class RemoveDeletedMicrosoft365Group : PnPGraphCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public Microsoft365GroupPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            var group = Identity?.GetDeletedGroup(AccessToken);

            if (group != null)
            {
                UnifiedGroupsUtility.PermanentlyDeleteUnifiedGroup(group.GroupId, AccessToken);
            }
        }
    }
}