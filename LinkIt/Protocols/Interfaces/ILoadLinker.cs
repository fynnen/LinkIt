using System;
using System.Collections.Generic;

namespace LinkIt.Protocols.Interfaces
{
    public interface ILoadLinker<TRootLinkedSource>
    {
        TRootLinkedSource FromModel<TRootLinkedSourceModel>(TRootLinkedSourceModel model);
        List<TRootLinkedSource> FromModels<TRootLinkedSourceModel>(params TRootLinkedSourceModel[] models);
        TRootLinkedSource ById<TRootLinkedSourceModelId>(TRootLinkedSourceModelId modelId);
        List<TRootLinkedSource> ByIds<TRootLinkedSourceModelId>(params TRootLinkedSourceModelId[] modelIds);
        List<TRootLinkedSource> FromQuery<TRootLinkedSourceModel>(Func<List<TRootLinkedSourceModel>> executeQuery);
        List<TRootLinkedSource> FromQuery<TRootLinkedSourceModel>(Func<IReferenceLoader, List<TRootLinkedSourceModel>> executeQuery);
    }
}