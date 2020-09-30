﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.Serialization;
using PnP.PowerShell.ModuleFilesGenerator.Model;
using CmdletInfo = PnP.PowerShell.ModuleFilesGenerator.Model.CmdletInfo;
using System.ComponentModel;

namespace PnP.PowerShell.ModuleFilesGenerator
{
    internal class CmdletsAnalyzer
    {
        private readonly Assembly _assembly;

        internal CmdletsAnalyzer(Assembly assembly)
        {
            _assembly = assembly;
        }

        internal List<CmdletInfo> Analyze()
        {

            return GetCmdlets();

        }
        private List<CmdletInfo> GetCmdlets()
        {
            List<CmdletInfo> cmdlets = new List<CmdletInfo>();
            var types = _assembly.GetTypes().Where(t => t.BaseType != null &&  (t.BaseType.Name.StartsWith("PnP") || t.BaseType.Name == "PSCmdlet" || (t.BaseType.BaseType != null && (t.BaseType.BaseType.Name.StartsWith("PnP") || t.BaseType.BaseType.Name == "PSCmdlet")))).OrderBy(t => t.Name).ToArray();

            foreach (var type in types)
            {
                var cmdletInfo = new Model.CmdletInfo();
                cmdletInfo.CmdletType = type;

                var attributes = type.GetCustomAttributes();

                foreach (var attribute in attributes)
                {
                    var cmdletAttribute = attribute as CmdletAttribute;
                    if (cmdletAttribute != null)
                    {
                        var customAttributesData = type.GetCustomAttributesData();
                        var customAttributeData = customAttributesData.FirstOrDefault(c => c.AttributeType == typeof(CmdletAttribute));
                        if (customAttributeData != null)
                        {
                            cmdletInfo.Verb = customAttributeData.ConstructorArguments[0].Value.ToString();
                            cmdletInfo.Noun = customAttributeData.ConstructorArguments[1].Value.ToString();
                        }
                    }
                    var aliasAttribute = attribute as AliasAttribute;
                    if (aliasAttribute != null)
                    {
                        var customAttributeData = type.GetCustomAttributesData().FirstOrDefault(c => c.AttributeType == typeof(AliasAttribute));
                        if (customAttributeData != null)
                        {
                            foreach (var name in customAttributeData.ConstructorArguments)
                            {
                                cmdletInfo.Aliases.Add(name.Value as string);
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(cmdletInfo.Verb) && !string.IsNullOrEmpty(cmdletInfo.Noun))
                {
                 //  cmdletInfo.Syntaxes = GetCmdletSyntaxes(cmdletInfo);
                    //cmdletInfo.Parameters = GetCmdletParameters(cmdletInfo);
                    cmdlets.Add(cmdletInfo);
                }
            }

            return cmdlets;
        }

        //private List<CmdletSyntax> GetCmdletSyntaxes(Model.CmdletInfo cmdletInfo)
        //{
        //    List<CmdletSyntax> syntaxes = new List<CmdletSyntax>();
        //    var fields = GetFields(cmdletInfo.CmdletType);
        //    foreach (var field in fields)
        //    {
        //        MemberInfo fieldInfo = field;
        //        var obsolete = fieldInfo.GetCustomAttributes<ObsoleteAttribute>().Any();

        //        if (!obsolete)
        //        {
        //            var parameterAttributes = fieldInfo.GetCustomAttributes<ParameterAttribute>(true).Where(a => a.ParameterSetName != ParameterAttribute.AllParameterSets);
        //            var pnpAttributes = field.GetCustomAttributes<PnPParameterAttribute>(true);
        //            foreach (var parameterAttribute in parameterAttributes)
        //            {
        //                var cmdletSyntax = syntaxes.FirstOrDefault(c => c.ParameterSetName == parameterAttribute.ParameterSetName);
        //                if (cmdletSyntax == null)
        //                {
        //                    cmdletSyntax = new CmdletSyntax();
        //                    cmdletSyntax.ParameterSetName = parameterAttribute.ParameterSetName;
        //                    syntaxes.Add(cmdletSyntax);
        //                }
        //                var typeString = field.FieldType.Name;
        //                if (field.FieldType.IsGenericType)
        //                {
        //                    typeString = field.FieldType.GenericTypeArguments[0].Name;
        //                }
        //                var fieldAttribute = field.FieldType.GetCustomAttributes<CmdletPipelineAttribute>(false).FirstOrDefault();
        //                if (fieldAttribute != null)
        //                {
        //                    if (fieldAttribute.Type != null)
        //                    {
        //                        typeString = string.Format(fieldAttribute.Description, fieldAttribute.Type.Name);
        //                    }
        //                    else
        //                    {
        //                        typeString = fieldAttribute.Description;
        //                    }
        //                }
        //                var order = 0;
        //                if (pnpAttributes != null && pnpAttributes.Any())
        //                {
        //                    order = pnpAttributes.First().Order;
        //                }
        //                cmdletSyntax.Parameters.Add(new CmdletParameterInfo()
        //                {
        //                    Name = field.Name,
        //                    Description = parameterAttribute.HelpMessage,
        //                    Position = parameterAttribute.Position,
        //                    Required = parameterAttribute.Mandatory,
        //                    Type = typeString,
        //                    Order = order
        //                });
        //            }
        //        }
        //    }

        //    // AllParameterSets
        //    foreach (var field in fields)
        //    {
        //        var obsolete = field.GetCustomAttributes<ObsoleteAttribute>().Any();

        //        if (!obsolete)
        //        {
        //            var parameterAttributes = field.GetCustomAttributes<ParameterAttribute>(true).Where(a => a.ParameterSetName == ParameterAttribute.AllParameterSets);
        //            var pnpAttributes = field.GetCustomAttributes<PnPParameterAttribute>(true);
        //            foreach (var parameterAttribute in parameterAttributes)
        //            {
        //                if (!syntaxes.Any())
        //                {
        //                    syntaxes.Add(new CmdletSyntax { ParameterSetName = ParameterAttribute.AllParameterSets });
        //                }

        //                foreach (var syntax in syntaxes)
        //                {
        //                    var typeString = field.FieldType.Name;
        //                    if (field.FieldType.IsGenericType)
        //                    {
        //                        typeString = field.FieldType.GenericTypeArguments[0].Name;
        //                    }
        //                    var fieldAttribute = field.FieldType.GetCustomAttributes<CmdletPipelineAttribute>(false).FirstOrDefault();
        //                    if (fieldAttribute != null)
        //                    {
        //                        if (fieldAttribute.Type != null)
        //                        {
        //                            typeString = string.Format(fieldAttribute.Description, fieldAttribute.Type.Name);
        //                        }
        //                        else
        //                        {
        //                            typeString = fieldAttribute.Description;
        //                        }
        //                    }
        //                    var order = 0;
        //                    if (pnpAttributes != null && pnpAttributes.Any())
        //                    {
        //                        order = pnpAttributes.First().Order;
        //                    }
        //                    syntax.Parameters.Add(new CmdletParameterInfo()
        //                    {
        //                        Name = field.Name,
        //                        Description = parameterAttribute.HelpMessage,
        //                        Position = parameterAttribute.Position,
        //                        Required = parameterAttribute.Mandatory,
        //                        Type = typeString,
        //                        Order = order
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    return syntaxes;
        //}

        //private List<CmdletParameterInfo> GetCmdletParameters(Model.CmdletInfo cmdletInfo)
        //{
        //    List<CmdletParameterInfo> parameters = new List<CmdletParameterInfo>();
        //    var fields = GetFields(cmdletInfo.CmdletType);
        //    foreach (var field in fields)
        //    {
        //        MemberInfo fieldInfo = field;
        //        var obsolete = fieldInfo.GetCustomAttributes<ObsoleteAttribute>().Any();

        //        if (!obsolete)
        //        {
        //            var aliases = field.GetCustomAttributes<AliasAttribute>(true);
        //            var parameterAttributes = field.GetCustomAttributes<ParameterAttribute>(true);
        //            var pnpParameterAttributes = field.GetCustomAttributes<PnPParameterAttribute>(true);
        //            foreach (var parameterAttribute in parameterAttributes)
        //            {
        //                var description = parameterAttribute.HelpMessage;
        //                if (string.IsNullOrEmpty(description))
        //                {
        //                    // Maybe a generic one? Find the one with only a helpmessage set
        //                    var helpParameterAttribute = parameterAttributes.FirstOrDefault(p => !string.IsNullOrEmpty(p.HelpMessage));
        //                    if (helpParameterAttribute != null)
        //                    {
        //                        description = helpParameterAttribute.HelpMessage;
        //                    }
        //                }
                        
        //                var typeString = field.FieldType.Name;
        //                if(field.FieldType.IsGenericType)
        //                {
        //                    typeString = field.FieldType.GenericTypeArguments[0].Name;
        //                }
        //                var fieldAttribute = field.FieldType.GetCustomAttributes<CmdletPipelineAttribute>(false).FirstOrDefault();
        //                if (fieldAttribute != null)
        //                {
        //                    if (fieldAttribute.Type != null)
        //                    {
                               
        //                        typeString = string.Format(fieldAttribute.Description, fieldAttribute.Type.Name);
        //                    }
        //                    else
        //                    {
        //                        typeString = fieldAttribute.Description;
        //                    }
        //                }
        //                var order = 0;
        //                if (pnpParameterAttributes != null && pnpParameterAttributes.Any())
        //                {
        //                    order = pnpParameterAttributes.First().Order;
        //                }
        //                var cmdletParameterInfo = new CmdletParameterInfo()
        //                {
        //                    Description = description,
        //                    Type = typeString,
        //                    Name = field.Name,
        //                    Required = parameterAttribute.Mandatory,
        //                    Position = parameterAttribute.Position,
        //                    ValueFromPipeline = parameterAttribute.ValueFromPipeline,
        //                    ParameterSetName = parameterAttribute.ParameterSetName,
        //                    Order = order
        //                };

        //                if (aliases != null && aliases.Any())
        //                {
        //                    var customAttributesData = fieldInfo.GetCustomAttributesData();
        //                    foreach (var aliasAttribute in customAttributesData.Where(c => c.AttributeType == typeof(AliasAttribute)))
        //                    {
        //                        cmdletParameterInfo.Aliases.AddRange(aliasAttribute.ConstructorArguments.Select(a => a.ToString()));
        //                    }
        //                }
        //                parameters.Add(cmdletParameterInfo);

        //            }
        //        }
        //    }
        //    return parameters;
        //}

        #region Helpers
        private static List<FieldInfo> GetFields(Type t)
        {
            var fieldInfoList = new List<FieldInfo>();
            foreach (var fieldInfo in t.GetFields())
            {
                fieldInfoList.Add(fieldInfo);
            }
            if (t.BaseType != null && t.BaseType.BaseType != null)
            {
                fieldInfoList.AddRange(GetFields(t.BaseType.BaseType));
            }
            return fieldInfoList;
        }

        private static string ToEnumString<T>(T type)
        {
            var enumType = typeof(T);
            var name = Enum.GetName(enumType, type);
            try
            {
                var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
                return enumMemberAttribute.Value;
            }
            catch
            {
                return name;
            }
        }
        #endregion
    }
}
