﻿using System.Management.Automation;
using Microsoft.SharePoint.Client;

using System.Linq.Expressions;
using System;
using System.Linq;

namespace PnP.PowerShell.Commands.Site
{
    [Cmdlet(VerbsCommon.Get, "PnPSiteCollectionAdmin")]
    public class GetSiteCollectionAdmin : PnPWebCmdlet
    {
        protected override void ExecuteCmdlet()
        {
            var retrievalExpressions = new Expression<Func<User, object>>[]
           {
                u => u.Id,
                u => u.Title,
                u => u.LoginName,
                u => u.Email,
                u => u.IsShareByEmailGuestUser,
                u => u.IsSiteAdmin,
                u => u.UserId,
                u => u.IsHiddenInUI,
                u => u.PrincipalType,
                u => u.Alerts.Include(
                    a => a.Title,
                    a => a.Status),
                u => u.Groups.Include(
                    g => g.Id,
                    g => g.Title,
                    g => g.LoginName)
           };

            var siteCollectionAdminUsersQuery = SelectedWeb.SiteUsers.Where(u => u.IsSiteAdmin);
            var siteCollectionAdminUsers = ClientContext.LoadQuery(siteCollectionAdminUsersQuery.Include(retrievalExpressions));
            ClientContext.ExecuteQueryRetry();

            WriteObject(siteCollectionAdminUsers, true);
        }
    }
}
