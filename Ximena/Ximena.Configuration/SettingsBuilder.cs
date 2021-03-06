﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public static class SettingsBuilder
    {
        public static RenderSettings BuildSettings(string path)
        {
            var settingsPath = GetSettingsPath(path);
            var settingsDir = Path.GetDirectoryName(settingsPath);

            var settingsData = GetSettingsData(settingsPath);
            var settings = LoadSettings(settingsData);

            EstablishCriticalSettings(settings, settingsDir);
            EstablishGlobalDefaults(settings);

            ConfigureNamespaces(settings);

            return settings;
        }

        public static void IncludeViewModelSettings(RenderSettings settings, ViewModelSettings vm)
        {
            // get target namespace
            var ns = settings.namespaces[vm.EntityNamespace()];
            
            // merge existing view model settings, if there are any
            if(ns.viewModels.ContainsKey(vm.EntityType()))
            {
                var xVM = ns.viewModels[vm.EntityType()];
                vm.type = vm.type ?? xVM.type;
                vm.emitTo = vm.emitTo ?? xVM.emitTo;
                InheritViewModelSettingsBase(vm, xVM);

                // replace existing model with one merged with attribute settings
                ns.viewModels[vm.EntityType()] = vm;
            }
            // if no settings exist, merge with view model defaults for namespace and add it in
            else
            {
                ConfigureViewModelSettings(vm, ns, settings, vm.EntityType(), vm.EntityNamespace());
                ns.viewModels.Add(vm.EntityType(), vm);
            }
        }

        public static void IncludeMemberDefinition(MemberDefinition md, ViewModelSettings vm)
        {
            MemberDefinition xMember = null;
            var isProperty = md is PropertyDefinition;
            // merge existing member settings, if any
            if(isProperty && vm.properties.ContainsKey(md.Name()))
            {
                xMember = vm.properties[md.Name()];
            }
            else if(!isProperty && vm.collections.ContainsKey(md.Name()))
            {
                xMember = vm.collections[md.Name()];
            }

            if(xMember != null)
            {
                // read only setting on existing member only relevant if not already true on new one
                md.readOnly = md.readOnly ? md.readOnly : xMember.readOnly;
                md.summary = md.summary ?? xMember.summary;
                // replace existing member
                if (isProperty)
                {
                    vm.properties[md.Name()] = md as PropertyDefinition;
                }
                else
                {
                    vm.collections[md.Name()] = md as CollectionDefinition;
                }
            }
            else
            {
                // add new member
                if (isProperty)
                {
                    vm.properties.Add(md.Name(), md as PropertyDefinition);
                }
                else
                {
                    vm.collections.Add(md.Name(), md as CollectionDefinition);
                }
            }
        }

        private static void ConfigureNamespaces(RenderSettings settings)
        {
            foreach(var ns in settings.namespaces)
            {
                var name = ns.Key;
                var nsSettings = ns.Value;

                AssertNamespaceSettingsValid(name, nsSettings);
                ConfigureNamespaceSettings(name, nsSettings, settings);
            }
        }
        private static void ConfigureNamespaceSettings(string entityNamespace, NamespaceSettings ns,
            RenderSettings settings)
        {
            var gblNS = settings.globalDefaults.nameSpace;
            // map to namespace can not be null or empty; so we'll come up with a name if not given
            if(string.IsNullOrWhiteSpace(ns.mapToNamespace))
            {
                ns.mapToNamespace = 
                    $"{gblNS.vmNamespacePrefix}{entityNamespace}{gblNS.vmNamespaceSuffix}";
            }
            ns.emitAllEntities = ns.emitAllEntities ?? gblNS.emitAllEntities;

            EstablishNamespaceViewModelDefaults(ns.vmDefaults, settings);

            ConfigureViewModels(entityNamespace, ns, settings);
            ConfigureAdjunctViewModels(ns, settings);
        }
        private static void EstablishNamespaceViewModelDefaults(ViewModelNamespaceSettingDefaults nsVM,
            RenderSettings settings)
        {
            var gblVM = settings.globalDefaults.viewModel;
            nsVM.emitTo = EstablishSettingsDirectory(settings.emitDir, nsVM.emitTo,
                "Default view model namespace directory ('emitTo')");
            InheritViewModelSettingDefaults(nsVM, gblVM);
        }
        private static void AssertNamespaceSettingsValid(string entityNamespace,
            NamespaceSettings ns)
        {
            // entity namespace must be present for namespace settings
            if (string.IsNullOrWhiteSpace(entityNamespace))
            {
                throw new InvalidRenderSettingsException
                    ("Unnamed namespace settings entry encountered");
            }
            if (ns == null)
            {
                throw new InvalidRenderSettingsException
                    ($"Null namespace settings encountered for entity namespace '{entityNamespace}'");
            }
        }

        private static void ConfigureAdjunctViewModels(NamespaceSettings ns, RenderSettings settings)
        {
            foreach (var vm in ns.adjunctViewModels)
            {
                // assert that model definition is not null
                if (vm == null)
                {
                    throw new InvalidRenderSettingsException
                        ($"Null adjunct view model encountered in namespace '{ns.mapToNamespace}'");
                }
                // adjunct view models have no entity counterpart
                vm.HasPublicEntity(false);

                ConfigureViewModelSettings(vm, ns, settings);
            }
        }
        private static void ConfigureViewModels(string entityNamespace, NamespaceSettings ns, RenderSettings settings)
        {
            foreach(var vmEntry in ns.viewModels)
            {
                var entityType = vmEntry.Key;
                var vm = vmEntry.Value;
                AssertViewModelSettingsValid(entityType, ns.mapToNamespace, vm);
                ConfigureViewModelSettings(vm, ns, settings, entityType, entityNamespace);
            }
        }
        private static void ConfigureViewModelSettings(ViewModelSettings vm, NamespaceSettings ns,
            RenderSettings settings, string entity = null, string entityNamespace = null)
        {
            // view model type can not be null or empty; so if possible we'll come up with one 
            // using the entity name if not given, otherwise we fail
            if (string.IsNullOrWhiteSpace(vm.type))
            {
                if(!string.IsNullOrWhiteSpace(entity))
                {
                    vm.type =
                        $"{ns.vmDefaults.typePrefix}{entity}{ns.vmDefaults.typeSuffix}";
                }
                else
                {
                    throw new InvalidRenderSettingsException("Parameter 'type' is required for adjunct view model definitions");
                }
            }

            // inherit emit to directory from namespace view model setting defaults if not
            // specified
            vm.emitTo = vm.emitTo ?? ns.vmDefaults.emitTo;
            vm.emitTo = EstablishSettingsDirectory(settings.emitDir, vm.emitTo,
                "View model source file directory ('emitTo')");
            vm.EntityNamespace(entityNamespace);
            vm.EntityType(entity);
            vm.ViewModelNamespace(ns.mapToNamespace);

            InheritViewModelSettingsBase(vm, ns.vmDefaults);

            AssertPropertyDefinitionsValid(vm);
            AssertCollectionDefinitionsValid(vm);
        }
        private static void AssertViewModelSettingsValid(string entityType, string nameSpace, 
            ViewModelSettings vm)
        {
            // entity type must be present for view model settings
            if (string.IsNullOrWhiteSpace(entityType))
            {
                throw new InvalidRenderSettingsException($"Entity object type not specified for a view model in namespace '{nameSpace}'");
            }
            if (vm == null)
            {
                throw new InvalidRenderSettingsException($"Null view model settings encountered for entity '{entityType}'");
            }
        }
        private static void AssertPropertyDefinitionsValid(ViewModelSettings vm)
        {
            AssertMemberDefinitionsValid(true, vm.type, vm.properties);
        }
        private static void AssertCollectionDefinitionsValid(ViewModelSettings vm)
        {
            AssertMemberDefinitionsValid(false, vm.type, vm.properties);
        }
        private static void AssertMemberDefinitionsValid<T>(bool isProperties, string vmType,
            Dictionary<string, T> members) where T : MemberDefinition
        {
            string pc = isProperties ? "property" : "collection";
            foreach (var m in members)
            {
                if (string.IsNullOrWhiteSpace(m.Key))
                {
                    throw new InvalidRenderSettingsException($"Unnamed {pc} definition encountered in view model '{vmType}'");
                }
                if (m.Value == null)
                {
                    throw new InvalidRenderSettingsException($"Null {pc} settings encountered for {pc} '{vmType}.{m.Key}'");
                }
                if (string.IsNullOrWhiteSpace(m.Value.PropertyType()))
                {
                    throw new InvalidRenderSettingsException($"Type not specified for {pc} '{vmType}.{m.Key}'");
                }
                // ensure an access modifier is present
                if (string.IsNullOrWhiteSpace(m.Value.access))
                {
                    m.Value.access = "public";
                }
                // note property name within object
                m.Value.Name(m.Key);
            }
        }

        private static void EstablishGlobalDefaults(RenderSettings settings)
        {
            EstablishGlobalNamespaceDefaults(settings);
            EstablishGlobalViewModelDefaults(settings);
        }
        private static void EstablishGlobalNamespaceDefaults(RenderSettings settings)
        {
            // any default not explicitly set is overriden with system default setting
            var ns = settings.globalDefaults.nameSpace;
            ns.emitAllEntities = ns.emitAllEntities ?? true;
            ns.vmNamespacePrefix = ns.vmNamespacePrefix ?? string.Empty;
            ns.vmNamespaceSuffix = ns.vmNamespaceSuffix ?? ".ViewModels";
        }
        private static void EstablishGlobalViewModelDefaults(RenderSettings settings)
        {
            var vm = settings.globalDefaults.viewModel;
            var sysDefaults = new ViewModelSettingDefaults
            {
                access = "public",
                emitEntityPropertiesAsViewModels = false,
                inheritUsings = true,
                emitCollectionsAsObservable = false,
                emitAllProperties = true,
                partialClass = true,
                stubForCustomCode = true,
                typePrefix = string.Empty,
                typeSuffix = "VM",
                usings = new HashSet<string>()
            };
            // any default not explicitly set is overriden with system default setting
            InheritViewModelSettingDefaults(vm, sysDefaults);
        }

        private static void InheritViewModelSettingDefaults
            (ViewModelSettingDefaults target, ViewModelSettingDefaults parent)
        {
            target.typePrefix = target.typePrefix ?? parent.typePrefix;
            target.typeSuffix = target.typeSuffix ?? parent.typeSuffix;
            InheritViewModelSettingsBase(target, parent);
        }
        private static void InheritViewModelSettingsBase
            (ViewModelSettingsBase target, ViewModelSettingsBase parent)
        {
            target.access = target.access ?? parent.access;
            target.emitEntityPropertiesAsViewModels =
                target.emitEntityPropertiesAsViewModels ?? parent.emitEntityPropertiesAsViewModels;
            target.inheritUsings = target.inheritUsings ?? parent.inheritUsings;
            if(target.inheritUsings == true)
            {
                foreach(var u in parent.usings)
                {
                    target.usings.Add(u);
                }
            }
            target.emitCollectionsAsObservable =
                target.emitCollectionsAsObservable ?? parent.emitCollectionsAsObservable;
            target.emitAllProperties = target.emitAllProperties ?? parent.emitAllProperties;
            // stubForCustomCode being set true causes partialClass to be true, regardless of its actual value.
            // if stubForCustomCode is set to any other value, partialClass returns its true state
            // we'll preserve stubForCustomCode's set value, set it to null, then restore its original
            // set value later
            var sfcc = target.stubForCustomCode;
            target.stubForCustomCode = null;
            target.partialClass = target.partialClass ?? parent.partialClass;
            target.stubForCustomCode = sfcc ?? parent.stubForCustomCode;
        }

        private static void EstablishCriticalSettings(RenderSettings settings, string settingsDir)
        {
            // make sure source assembly file was specified and exists
            AssertSourceAssembly(settingsDir, settings);
            // establish root directory view model source will be written to
            settings.emitDir = EstablishSettingsDirectory(settingsDir, settings.emitDir,
                pathDescription: "Source code output directory ('emitDir')");
        }
        private static void AssertSourceAssembly(string settingsDir, RenderSettings settings)
        {
            //assembly reference must exist
            if (string.IsNullOrWhiteSpace(settings.sourceAssembly))
            {
                throw new InvalidRenderSettingsException("Required setting 'sourceAssembly' is missing or empty");
            }
            var assmPath = CombinePath(settingsDir, settings.sourceAssembly);
            if (!File.Exists(assmPath))
            {
                throw new DllNotFoundException($"Source assembly '{assmPath}' is missing or otherwise inaccessible");
            }
        }
        private static string EstablishSettingsDirectory(string root, string path, string pathDescription)
        {
            var resolvedPath = CombinePath(root, path);
            var checkPath = IsFileReference(resolvedPath);
            if (checkPath.HasValue && checkPath.Value)
            {
                throw new InvalidRenderSettingsException($"{pathDescription} may not refer to a file");
            }
            Directory.CreateDirectory(resolvedPath);

            return resolvedPath;
        }
        
        private static RenderSettings LoadSettings(string data)
        {
            try
            {
                var settings = JsonConvert.DeserializeObject<RenderSettings>(data);
                return settings ?? new RenderSettings();
            }
            catch (Exception e)
            {
                throw new InvalidRenderSettingsException("Settings file is invalid", e);
            }
        }
        private static string GetSettingsData(string path)
        {
            try
            {
                string data = null;
                using (var r = new StreamReader(path))
                {
                    data = r.ReadToEnd();
                }

                return data;
            }
            catch (Exception e)
            {
                throw new InvalidRenderSettingsException($"Failed to load settings file '{path}': {e.Message}", e);
            }
        }
        private static string GetSettingsPath(string path)
        {
            path = path ?? string.Empty;
            AssertValidPath(path);

            //combine path given with current directory if not a full path
            path = CombinePath(Directory.GetCurrentDirectory(), path);
            var isFileRef = IsFileReference(path);
            if(isFileRef.HasValue)
            {
                // if the path given is a directory, infer default file name 'settings.json'
                if (!isFileRef.Value)
                {
                    path = CombinePath(path, "settings.json");
                }

                //return path for existant file reference
                if(File.Exists(path))
                {
                    return path;
                }
            }

            // throw exception: path not valid
            throw new FileNotFoundException($"Settings file or directory '{path}' is inaccessible or does not exist");
        }

        private static void AssertValidPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) { return; }
            
            if(path.IndexOfAny(Path.GetInvalidPathChars()) > -1)
            {
                throw new InvalidRenderSettingsException($"File or directory reference '{path}' contains one or more invalid characters");
            }
        }
        private static bool? IsFileReference(string path)
        {
            if (File.Exists(path)) { return true; }
            if (Directory.Exists(path)) { return false; }

            // neither file nor directory exists
            return null;
        }
        private static string CombinePath(string a, string b)
        {
            if (string.IsNullOrWhiteSpace(b)) { return a; }

            try
            {
                return Path.GetFullPath(Path.Combine(a, b));
            }
            catch (Exception e)
            {
                var ex = $"Use of path '{b}' will result in an invalid file or directory reference";
                throw new InvalidRenderSettingsException(ex, e);
            }
        }
    }
}
