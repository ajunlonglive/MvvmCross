﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml;
using MvvmCross.Base;
using MvvmCross.Converters;
using MvvmCross.Binding;
using MvvmCross.Binding.Binders;
using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.Binding.Combiners;
using MvvmCross.Platforms.WinUi.Binding.MvxBinding;
using MvvmCross.Platforms.WinUi.Binding.MvxBinding.Target;
using MvvmCross.IoC;

namespace MvvmCross.Platforms.WinUi.Binding
{
    public class MvxWindowsBindingBuilder : MvxBindingBuilder
    {
        public enum BindingType
        {
            Windows,
            MvvmCross
        }

        private readonly BindingType _bindingType;

        public MvxWindowsBindingBuilder(
            BindingType bindingType = BindingType.MvvmCross)
        {
            _bindingType = bindingType;
        }

        public override void DoRegistration(IMvxIoCProvider iocProvider)
        {
            base.DoRegistration(iocProvider);
            InitializeBindingCreator();
        }

        protected override void RegisterBindingFactories(IMvxIoCProvider iocProvider)
        {
            switch (_bindingType)
            {
                case BindingType.Windows:
                    // no need for MvvmCross binding factories - so don't create them
                    break;

                case BindingType.MvvmCross:
                    base.RegisterBindingFactories(iocProvider);
                    break;

                default:
                    throw new InvalidOperationException($"Unable to register binding factories for BindingType: {_bindingType}");
            }
        }

        protected override IMvxTargetBindingFactoryRegistry CreateTargetBindingRegistry()
        {
            switch (_bindingType)
            {
                case BindingType.Windows:
                    return base.CreateTargetBindingRegistry();

                case BindingType.MvvmCross:
                    return new MvxWindowsTargetBindingFactoryRegistry();

                default:
                    throw new InvalidOperationException($"Unable to create target binding registry for BindingType: {_bindingType}");
            }
        }

        private void InitializeBindingCreator()
        {
            var creator = CreateBindingCreator();
            Mvx.IoCProvider.RegisterSingleton(creator);
        }

        protected virtual IMvxBindingCreator CreateBindingCreator()
        {
            switch (_bindingType)
            {
                case BindingType.Windows:
                    return new MvxWindowsBindingCreator();

                case BindingType.MvvmCross:
                    return new MvxMvvmCrossBindingCreator();

                default:
                    throw new InvalidOperationException($"Unable to create binding creator for BindingType: {_bindingType}");
            }
        }

        protected override void FillValueConverters(IMvxValueConverterRegistry registry)
        {
            base.FillValueConverters(registry);

            if (MvxSingleton<IMvxWindowsAssemblyCache>.Instance != null)
            {
                foreach (var assembly in MvxSingleton<IMvxWindowsAssemblyCache>.Instance.Assemblies)
                {
                    registry.Fill(assembly);
                }
            }
        }

        protected override void FillValueCombiners(IMvxValueCombinerRegistry registry)
        {
            base.FillValueCombiners(registry);

            if (MvxSingleton<IMvxWindowsAssemblyCache>.Instance != null)
            {
                foreach (var assembly in MvxSingleton<IMvxWindowsAssemblyCache>.Instance.Assemblies)
                {
                    registry.Fill(assembly);
                }
            }
        }

        protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
        {
            registry.RegisterCustomBindingFactory<FrameworkElement>(
                MvxWindowsPropertyBinding.FrameworkElement_Visible,
                view => new MvxVisibleTargetBinding(view));

            registry.RegisterCustomBindingFactory<FrameworkElement>(
                MvxWindowsPropertyBinding.FrameworkElement_Collapsed,
                view => new MvxCollapsedTargetBinding(view));

            registry.RegisterCustomBindingFactory<FrameworkElement>(
                MvxWindowsPropertyBinding.FrameworkElement_Hidden,
                view => new MvxCollapsedTargetBinding(view));

            base.FillTargetFactories(registry);
        }
    }
}
