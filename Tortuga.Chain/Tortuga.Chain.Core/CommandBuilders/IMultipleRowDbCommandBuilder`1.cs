﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This allows the use of multi-row materializers against a command builder.
    /// </summary>
    /// <remarks>Warning: This interface is meant to simulate multiple inheritance and work-around some issues with exposing generic types. Do not implement it in client code, as new method will be added over time.</remarks>
    public interface IMultipleRowDbCommandBuilder<TObject> : IMultipleRowDbCommandBuilder, ISingleRowDbCommandBuilder<TObject>
            where TObject : class
    {
        /// <summary>
        /// Materializes the result as a list of objects.
        /// </summary>
        /// <param name="collectionOptions">The collection options.</param>
        /// <returns></returns>
        IConstructibleMaterializer<List<TObject>> ToCollection(CollectionOptions collectionOptions = CollectionOptions.None);

        /// <summary>
        /// Materializes the result as a dictionary of objects.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="keyColumn">The key column.</param>
        /// <param name="dictionaryOptions">The dictionary options.</param>
        /// <returns></returns>
        IConstructibleMaterializer<Dictionary<TKey, TObject>> ToDictionary<TKey>(string keyColumn, DictionaryOptions dictionaryOptions = DictionaryOptions.None);

        /// <summary>
        /// Materializes the result as a dictionary of objects.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="keyFunction">The key function.</param>
        /// <param name="dictionaryOptions">The dictionary options.</param>
        /// <returns></returns>
        IConstructibleMaterializer<Dictionary<TKey, TObject>> ToDictionary<TKey>(Func<TObject, TKey> keyFunction, DictionaryOptions dictionaryOptions = DictionaryOptions.None);

        /// <summary>
        /// Materializes the result as an immutable array of objects.
        /// </summary>
        /// <param name="collectionOptions">The collection options.</param>
        /// <returns>Tortuga.Chain.IConstructibleMaterializer&lt;System.Collections.Immutable.ImmutableArray&lt;TObject&gt;&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        /// <remarks>In theory this will offer better performance than ToImmutableList if you only intend to read the result.</remarks>
        IConstructibleMaterializer<ImmutableArray<TObject>> ToImmutableArray(CollectionOptions collectionOptions = CollectionOptions.None);

        /// <summary>
        /// Materializes the result as an immutable list of objects.
        /// </summary>
        /// <param name="collectionOptions">The collection options.</param>
        /// <returns>Tortuga.Chain.IConstructibleMaterializer&lt;System.Collections.Immutable.ImmutableList&lt;TObject&gt;&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        /// <remarks>In theory this will offer better performance than ToImmutableArray if you intend to further modify the result.</remarks>
        IConstructibleMaterializer<ImmutableList<TObject>> ToImmutableList(CollectionOptions collectionOptions = CollectionOptions.None);

        /// <summary>
        /// Materializes the result as an instance of the indicated type
        /// </summary>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        IConstructibleMaterializer<TObject> ToObject(RowOptions rowOptions = RowOptions.None);

        /// <summary>
        /// Materializes the result as an instance of the indicated type
        /// </summary>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        IConstructibleMaterializer<TObject?> ToObjectOrNull(RowOptions rowOptions = RowOptions.None);

        /// <summary>
        /// Materializes the result as a immutable dictionary of objects.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="keyFunction">The key function.</param>
        /// <param name="dictionaryOptions">The dictionary options.</param>
        /// <returns></returns>
        IConstructibleMaterializer<ImmutableDictionary<TKey, TObject>> ToImmutableDictionary<TKey>(Func<TObject, TKey> keyFunction, DictionaryOptions dictionaryOptions = DictionaryOptions.None);

        /// <summary>
        /// Materializes the result as a immutable dictionary of objects.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="keyColumn">The key column.</param>
        /// <param name="dictionaryOptions">The dictionary options.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IConstructibleMaterializer<ImmutableDictionary<TKey, TObject>> ToImmutableDictionary<TKey>(string keyColumn, DictionaryOptions dictionaryOptions = DictionaryOptions.None);
    }
}
