﻿using System.Reflection;

namespace LinkIt.Conventions.Interfaces
{
    public interface ILoadLinkExpressionConvention{
        string Name { get; }
        bool DoesApply(PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty);
    }
}