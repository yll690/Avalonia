﻿// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Data;
using Portable.Xaml;
using Portable.Xaml.ComponentModel;
using Portable.Xaml.Markup;

namespace Avalonia.Markup.Xaml.MarkupExtensions
{
    public class DynamicResourceExtension : MarkupExtension, IBinding
    {
        private IResourceProvider _anchor;

        public DynamicResourceExtension()
        {
        }

        public DynamicResourceExtension(string resourceKey)
        {
            ResourceKey = resourceKey;
        }

        public string ResourceKey { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var context = (ITypeDescriptorContext)serviceProvider;
            var provideTarget = context.GetService<IProvideValueTarget>();

            if (!(provideTarget.TargetObject is IResourceProvider))
            {
                _anchor = GetAnchor<IResourceProvider>(context);
            }

            return this;
        }

        InstancedBinding IBinding.Initiate(
            IAvaloniaObject target,
            AvaloniaProperty targetProperty,
            object anchor,
            bool enableDataValidation)
        {
            var control = target as IResourceProvider ?? _anchor;

            if (control != null)
            {
                return new InstancedBinding(control.GetResourceObservable(ResourceKey));
            }

            return null;
        }

        private T GetAnchor<T>(ITypeDescriptorContext context) where T : class
        {
            var schemaContext = context.GetService<IXamlSchemaContextProvider>()?.SchemaContext;
            var ambientProvider = context.GetService<IAmbientProvider>();
            var xamlType = schemaContext.GetXamlType(typeof(T));
            return ambientProvider.GetFirstAmbientValue(xamlType) as T;
        }
    }
}
