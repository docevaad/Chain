using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This allows the use of multi-row materializers against a command builder.
    /// </summary>
    public interface IMultipleRowDbCommandBuilder : ISingleRowDbCommandBuilder
    {
        /// <summary>
        /// Indicates the results should be materialized as a list of booleans.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<bool>> AsBooleanList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Materializes the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <returns></returns>
        ILink<List<TObject>> AsCollection<TObject>()
            where TObject : class, new();
        /// <summary>
        /// Materializes the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        ILink<TCollection> AsCollection<TObject, TCollection>()
            where TObject : class, new()
            where TCollection : ICollection<TObject>, new();
        /// <summary>
        /// Indicates the results should be materialized as a DataSet.
        /// </summary>
        ILink<DataTable> AsDataTable();
        /// <summary>
        /// Indicates the results should be materialized as a list of DateTime.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<DateTime>> AsDateTimeList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of DateTimeOffset.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<DateTimeOffset>> AsDateTimeOffsetList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<decimal>> AsDecimalList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<double>> AsDoubleList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of Guids.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<Guid>> AsGuidList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<short>> AsInt16List(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<int>> AsInt32List(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<long>> AsInt64List(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<float>> AsSingleList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of strings.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<string>> AsStringList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a Table.
        /// </summary>
        ILink<Table> AsTable();
        /// <summary>
        /// Indicates the results should be materialized as a list of TimeSpan.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<TimeSpan>> AsTimeSpanList(ListOptions listOptions = ListOptions.None);
    }
}
